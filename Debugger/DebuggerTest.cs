using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.Debugger
{
    public class DebuggerTest
    {
        public struct StatePropertyKey
        {
            public StatePropertyKey(int objID, string objName, string componentName, string propName)
            {
                ObjID = objID;
                ObjName = objName;
                ComponentName = componentName;
                PropName = propName;
            }
            
            public int ObjID { get;  }
            public string ObjName { get; }
            
            public string ComponentName { get; }
            
            public string PropName { get; }

            public override string ToString() => $"Object: {ObjName} (ID: {ObjID}), Component: {ComponentName}, Property: {PropName}";
        }
        
        // a list of dictionaries is a possible way to save and reload the state over time
        // it may be treated as a stack for the purposes of an "undo"
        public static List<Dictionary<StatePropertyKey, object>> StateOfVariables = new List<Dictionary<StatePropertyKey, object>>();
        
        /* integer representing the next index to save a state at. Nominally this would be equal to the last index
         of StateOfVariables + 1, but in case of restoring a state from the middle of the list, it would instead be
         (Last Restored State) + 1 instead. _indexToSaveAt-1 represents the current state. 
         */
        private static int _indexToSaveAt;
        
        /* Two statically assigned variables representing debug options:
         * if _indexChecker is true, the value of all parameters named "light" will be equal to _indexToSaveAt * 10,
         * to test if indexes are being navigated correctly
         * if _debugPrintValues is true, the value of each saved parameter will be printed with Debug.Log
         */
        private static bool _indexChecker = false;
        private static bool _debugPrintValues = false;
        
        public static void SaveState()
        {
            /* key value pair for properties, the string is temporarily a collection of the name of the object,
             the name of the ECA Member and the name of the state variable to identify it
             */
            Dictionary<StatePropertyKey, object> attributesCollection = new Dictionary<StatePropertyKey, object>();
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            
            foreach (var gameObject in allObjects)
                //Iterate each game object, checking if ECAObject - an universal component - appears in their components
            {
                MonoBehaviour[] components;
                if (gameObject.GetComponent<ECAObject>() != null)
                {
                    components = gameObject.GetComponents<MonoBehaviour>();
                    
                    /* If they have ECAObject, iterate through their components to find other ECA Components
                     by checking to see if they have the ECARules4All.
                     */
                    foreach (MonoBehaviour component in components)
                    {
                        if (component.GetType().GetCustomAttributes(typeof(ECARules4AllAttribute), true).Length > 0)
                        {   
                            IEnumerable<MemberInfo> ECAStateVariables = component.GetType().GetMembers().Where(ECAMember =>
                                ECAMember.GetCustomAttributes(typeof(StateVariableAttribute), true).Length > 0);
                            /*
                             * For every state variable get the value and add it to the dictionary
                             * keyed under the ID of the object and name of component and property
                             */
                            foreach (MemberInfo ECAStateVariable in ECAStateVariables)
                            {
                                
                                StatePropertyKey key = new StatePropertyKey(gameObject.GetInstanceID(), gameObject.name, component.GetType().Name, ECAStateVariable.Name);
                                FieldInfo fieldInfo;
                                PropertyInfo propertyInfo;
                                object value = null;
                                //object processedValue = null;
                                if (ECAStateVariable.MemberType.Equals(MemberTypes.Field))
                                {
                                    fieldInfo = (FieldInfo)ECAStateVariable;
                                    value = fieldInfo.GetValue(component);
                                }

                                if (ECAStateVariable.MemberType.Equals(MemberTypes.Property))
                                {
                                    propertyInfo = (PropertyInfo)ECAStateVariable;
                                    value = propertyInfo.GetValue(component);
                                }
                                /*if(value != null){
                                    processedValue = SerializeUtils.SerializeAttribute(value);
                                }*/
                                if (_indexChecker && ECAStateVariable.Name == "intensity")
                                {
                                    value = _indexToSaveAt * 10;
                                }
                                attributesCollection.Add(key, value);
                            }
                        }
                    }
                }
                
            }
            if(_debugPrintValues){
                foreach (var keyValuePair in attributesCollection)
                {
                    Debug.Log(keyValuePair.Key + " - " + keyValuePair.Value);
                }
            }
            /* Finally, add the completed dictionary to the list of states
             First check if we are not saving at the end of the state list;
             in that case we need to wipe the list first
             */
            if (StateOfVariables.Count > _indexToSaveAt)
            {
                WipeStatesPastIndex(_indexToSaveAt);
            }
            StateOfVariables.Add(attributesCollection);
            _indexToSaveAt = StateOfVariables.Count;
            Debug.Log($"Saved state at index: {_indexToSaveAt-1}");
        }

        public static void RestoreLatestState()
        {
            RestoreStateAtIndex(StateOfVariables.Count-1);
        }
        
        /* Undo and redo methods: the idea is that since our current state index is at _indexToSaveAt-1,
         * the previous state is at -2, and the next state is at +0. 
         */
        public static void UndoState()
        {
            RestoreStateAtIndex(_indexToSaveAt-2);
        }

        public static void RedoState()
        {
            RestoreStateAtIndex(_indexToSaveAt);
        }

        public static void RestoreStateAtIndex(int index)
        {
            if(StateOfVariables.Count > index && index >= 0){
                Debug.Log($"Restoring state at index: {index}");
                RestoreState(StateOfVariables[index]);
                _indexToSaveAt = index + 1;
            }
            else
            {
                Debug.LogWarning($"Error restoring state at index: {index} (index must be between 0 and {StateOfVariables.Count - 1})");
            }
        }
        
        // Same general flow as SaveState, but when an attribute is found get the value from the state and set it
        private static void RestoreState(Dictionary<StatePropertyKey, object> state)
        {
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var gameObject in allObjects)
            {
                MonoBehaviour[] components;
                if (gameObject.GetComponent<ECAObject>() != null)
                {
                    components = gameObject.GetComponents<MonoBehaviour>();
                    foreach (MonoBehaviour component in components)
                    {
                        if (component.GetType().GetCustomAttributes(typeof(ECARules4AllAttribute), true).Length > 0)
                        {   
                            IEnumerable<MemberInfo> ECAStateVariables = component.GetType().GetMembers().Where(ECAMember =>
                                ECAMember.GetCustomAttributes(typeof(StateVariableAttribute), true).Length > 0);
                            foreach (MemberInfo ECAStateVariable in ECAStateVariables)
                            {
                                
                                StatePropertyKey key = new StatePropertyKey(gameObject.GetInstanceID(), gameObject.name, component.GetType().Name, ECAStateVariable.Name);
                                object value;
                                if (state.TryGetValue(key, out value)){
                                    FieldInfo fieldInfo;
                                    PropertyInfo propertyInfo;
                                    if (ECAStateVariable.MemberType.Equals(MemberTypes.Field))
                                    {
                                        fieldInfo = (FieldInfo)ECAStateVariable;
                                        fieldInfo.SetValue(component, value);
                                    }

                                    if (ECAStateVariable.MemberType.Equals(MemberTypes.Property))
                                    {
                                        propertyInfo = (PropertyInfo)ECAStateVariable;
                                        propertyInfo.SetValue(component, value);
                                    }

                                }
                                else
                                {
                                    Debug.LogWarning($"Error getting state variable at key: {key}");
                                }
                            }
                        }
                    }
                }
            }
            
        }

        private static void WipeStatesPastIndex(int index)
        {
            StateOfVariables.RemoveRange(index, StateOfVariables.Count - index);
        }
    }
}