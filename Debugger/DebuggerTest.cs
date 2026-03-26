using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using ECARules4All_DLL.Utils;
using UnityEngine;
using Newtonsoft.Json;
using Object = UnityEngine.Object;

namespace ECARules4All_DLL.Debugger
{
    public class DebuggerTest
    {
        static DebuggerTest()
        {
            CleanDebuggerFolder();
        }
        
        /*
         * Represents a "complete" state
         */
        public class FrozenState
        {
            private DateTime timestamp;
            private Rule[] rules;
            private Action action;
            
            /* Structure to keep track of state variables, a list of triple-nested dictionaries,
             * the outer dictionary has gameObject names as keys
             * the middle dictionary has component names as keys
             * the inner dictionary has property names as keys, with the property values as names
             */
            private Dictionary<string, Dictionary<string, Dictionary<string, object>>> properties;

            public DateTime Timestamp
            {
                get => timestamp;
                set => timestamp = value;
            }

            public Rule[] Rules
            {
                get => rules;
                set => rules = value;
            }

            public Action Action
            {
                get => action;
                set => action = value;
            }

            public Dictionary<string, Dictionary<string, Dictionary<string, object>>> Properties
            {
                get => properties;
                set => properties = value;
            }

            public FrozenState(Action action)
            {
                Timestamp = DateTime.Now;
                Rules = RuleEngine.GetInstance().Rules().ToArray();
                Action = action;
            }
            
        }
        
        /* integer representing the next index to save a state at. Nominally this would be equal to the last index
         of JSON files saved in DebuggerTempSaveData + 1, but in case of restoring a state from the middle of it, 
         it would instead be (Last Restored State) + 1 instead. _indexToSaveAt-1 represents the current state. 
         */
        private static int _indexToSaveAt;
        
        /* integer representing the amount of states saved. Used primarily to check whether we're in the middle of
         * the "state list" when saving a new state, in order to wipe any states coming after it
         * E.G: We save 10 states, _stateCount and _indexToSaveAt are both at 10; We restore a state at index 0,
         * so _indexToSaveAt becomes 1, and if we then try to save a state it compares the two variables to see that
         * we need to wipe the states above 0 before saving a new state
         */
        private static int _stateCount;

        
        /* Const representing debug options
         * if DebugPrintValues is true, the value of each saved parameter will be printed with Debug.Log
         * if DebugPrintRestoreValues is true, the value of each parameter restored off a JSON file will be printed
         * if DebugPrintFileOps is true, every operation involving saving and deleting a file will be printed
         * if DebugIndexChecker is true, changes the value of properties representing a light's intensity based on
         * the current state index
         */
        private const bool DebugPrintSaveValues = false;
        private const bool DebugPrintRestoreValues = false;
        private const bool DebugPrintFileOps = false;
        private const bool DebugIndexChecker = false;
        
        // Where JSON data is saved and loaded
        private const string FolderPath = "DebuggerTempSaveData";

        /* Save the completed state to the files
        *  First check if we are saving in the middle of the "state list";
        *  in that case we need to delete the states after the index we're saving the state at
        */
        public static void SaveState(Action action = null)
        {
            FrozenState frozenState = new FrozenState(action);
            frozenState.Properties = SaveProperties();
            
            if (_stateCount > _indexToSaveAt)
            {
                CleanDebuggerFolderPastIndex(_indexToSaveAt);
            }
            SaveStateToFile(frozenState);
            _indexToSaveAt += 1;
            _stateCount = _indexToSaveAt;
        }
        
        // Finds and saves every state variable to JSON
        private static Dictionary<string,Dictionary<string,Dictionary<string,object>>> SaveProperties()
        {
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            Dictionary<string,Dictionary<string,Dictionary<string,object>>> objDict = new  Dictionary<string,Dictionary<string,Dictionary<string,object>>>();
            foreach (var gameObject in allObjects)
                // Iterate each game object, checking if ECAObject - a universal component - appears in their components
            {
                MonoBehaviour[] components;
                if (gameObject.GetComponent<ECAObject>() != null)
                {
                    components = gameObject.GetComponents<MonoBehaviour>();
                    Dictionary<string,Dictionary<string,object>> compDict = new Dictionary<string, Dictionary<string,object>>();
                    /* If they have ECAObject, iterate through their components to find other ECA Components
                     by checking to see if they have the ECARules4All.
                     */
                    foreach (MonoBehaviour component in components)
                    {
                        if (component.GetType().GetCustomAttributes(typeof(ECARules4AllAttribute), true).Length > 0)
                        {   
                            Dictionary<string,object> propDict = new Dictionary<string, object>();
                            IEnumerable<MemberInfo> ECAStateVariables = component.GetType().GetMembers().Where(ECAMember =>
                                ECAMember.GetCustomAttributes(typeof(StateVariableAttribute), true).Length > 0);
                            
                            /* For every state variable get the value and add it to the dictionary
                             * keyed under the name of the property
                             */
                            foreach (MemberInfo ECAStateVariable in ECAStateVariables)
                            {
                                FieldInfo fieldInfo;
                                PropertyInfo propertyInfo;
                                object value = null;
                                object processedValue = null;
                                
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
                                
                                // Debug
                                if (DebugIndexChecker && ECAStateVariable.Name == "intensity")
                                {
                                    value = _indexToSaveAt * 10;
                                }
                                if (value != null)
                                {
                                    processedValue = SerializeUtils.SerializeAttribute(value);
                                }
                                
                                propDict.Add(ECAStateVariable.Name, processedValue);
                            }
                            // Then add every propDict to compDict keyed to the name of the component's type
                            compDict.Add(component.GetType().Name, propDict);
                        }
                    }
                    // Then add every compDict to objDict keyed to the name of the object
                    objDict.Add(gameObject.name, compDict);
                }
            }
            if(DebugPrintSaveValues)
            {
                foreach(KeyValuePair<string,Dictionary<string,Dictionary<string,object>>> objData in objDict)
                {
                    foreach(KeyValuePair<string,Dictionary<string,object>> compData in objData.Value)
                    {
                        foreach(KeyValuePair<string,object> propData in compData.Value)
                        {
                            Debug.Log($"{objData.Key}/{compData.Key}/{propData.Key}: {propData.Value}");
                        }
                    }
                }
            }

            return objDict;
            
        }

        public static void RestoreLatestState()
        {
            RestoreStateAtIndex(_stateCount-1);
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

        // Simply calls RestoreStateFromFile assuming the index is valid
        public static void RestoreStateAtIndex(int index)
        {
            if(_stateCount > index && index >= 0){
                RestoreStateFromFile(index);
                _indexToSaveAt = index + 1;
            }
            else
            {
                Debug.LogError($"Error restoring state at index: {index} (index must be between 0 and {_stateCount - 1})");
            }
        }
         
        // Attempts to restore a state by reading a file and deserializing the JSON
        private static void RestoreStateFromFile(int index)
        {
            try
            {
                Dictionary<string, Dictionary<string, Dictionary<string, object>>> objectsDict =
                    ReadJsonFile(index).Properties;

                GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                foreach (var gameObject in allObjects)
                {
                    Dictionary<string, Dictionary<string, object>> componentsDict =
                        new Dictionary<string, Dictionary<string, object>>();
                    MonoBehaviour[] components;
                    if (objectsDict.TryGetValue(gameObject.name, out componentsDict) &&
                        gameObject.GetComponent<ECAObject>() != null)
                    {
                        components = gameObject.GetComponents<MonoBehaviour>();
                        foreach (MonoBehaviour component in components)
                        {
                            Dictionary<string, object> propsDict = new Dictionary<string, object>();
                            if (componentsDict.TryGetValue(component.GetType().Name, out propsDict)
                                && component.GetType().GetCustomAttributes(typeof(ECARules4AllAttribute), true).Length >
                                0)
                            {
                                IEnumerable<MemberInfo> ECAStateVariables = component.GetType().GetMembers()
                                    .Where(ECAMember =>
                                        ECAMember.GetCustomAttributes(typeof(StateVariableAttribute), true).Length > 0);
                                foreach (MemberInfo ECAStateVariable in ECAStateVariables)
                                {

                                    object json;
                                    object value = null;
                                    if (propsDict.TryGetValue(ECAStateVariable.Name, out json))
                                    {
                                        if (json != null)
                                        {
                                            value = DeserializeProperty(
                                                ECAStateVariable.GetCustomAttribute<StateVariableAttribute>().type,
                                                json.ToString());
                                            if (DebugPrintRestoreValues)
                                            {
                                                Debug.Log(
                                                    $"Restoring value from json for {gameObject.name}/{component.GetType().Name}/{ECAStateVariable.Name}: {json} -> {value}");
                                            }
                                        }

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
                                        Debug.LogError($"Error getting state variable at key: {ECAStateVariable.Name}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
         // Saves the serialized state as a .json file, following an incrementing index for the file name
        public static void SaveStateToFile(FrozenState state)
        {
            
            if (!Directory.Exists(FolderPath)){
                Directory.CreateDirectory(FolderPath);
            }

            string filePath = FolderPath + "\\" + _indexToSaveAt + ".json";
            
            string json = JsonConvert.SerializeObject(state, Formatting.Indented);
            if (DebugPrintFileOps)
            {
                Debug.Log($"Creating file {filePath}...");
            }
            File.WriteAllText(filePath, json);
        }
        
         // Called once at the start to wipe old saved states
        public static void CleanDebuggerFolder()
        {
            if (!Directory.Exists(FolderPath)){
                Directory.CreateDirectory(FolderPath);
            }
            DirectoryInfo directory = new DirectoryInfo(FolderPath);
            foreach (FileInfo file in directory.GetFiles()) {
                if (DebugPrintFileOps)
                {
                    Debug.Log($"Deleting file {file.Name}...");
                }
                file.Delete();
            }
        }
        
        // Cleans the debug folder of every file with the format "{Number}.json", where Number is >= index
        public static void CleanDebuggerFolderPastIndex(int index)
        {
            if (!Directory.Exists(FolderPath)){
                Directory.CreateDirectory(FolderPath);
            }
            DirectoryInfo directory = new DirectoryInfo(FolderPath);
            foreach (FileInfo file in directory.GetFiles())
            {
                if (Int32.TryParse(file.Name.Split('.')[0], out var fileIndex))
                {
                    if (fileIndex >= index)
                    {
                        if (DebugPrintFileOps)
                        {
                            Debug.Log($"Deleting file {file.Name}...");
                        }
                        file.Delete();
                        
                    }
                }
                
            }
        }

        // Attempts to retrieve a state saved as JSON given the index
        public static FrozenState ReadJsonFile(int index)
        {
            if (!Directory.Exists(FolderPath)){
                Directory.CreateDirectory(FolderPath);
            }
            string filePath = FolderPath + "\\" + index + ".json";
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                FrozenState loadedState = JsonConvert.DeserializeObject<FrozenState>(json);
                return loadedState;
            }
            throw new FileNotFoundException($"File not found at path: {filePath}");

        }

        // Attempts to deserialize a serialized property, largely makes use of ConvertStringToParameter except for Color
        public static object DeserializeProperty(ECARules4AllType type, string json)
        {
            object value = null;
            switch (type)
            {
                case ECARules4AllType.Boolean:
                    value = SerializeUtils.ConvertStringToParameter(typeof(ECABoolean), json);
                    break;
                case ECARules4AllType.Color:
                    /*
                     * Adapted from:
                     * https://stackoverflow.com/questions/73587961/how-to-convert-string-to-color-in-unity-c-sharp
                     */
                    try
                    {
                        string[] rgba = json.Substring(5, json.Length - 6)
                            .Split(new[] { ", " }, 4, StringSplitOptions.None);
                        Color color = new Color(float.Parse(rgba[0], CultureInfo.InvariantCulture), 
                            float.Parse(rgba[1], CultureInfo.InvariantCulture), 
                            float.Parse(rgba[2], CultureInfo.InvariantCulture),
                            float.Parse(rgba[3], CultureInfo.InvariantCulture));
                        value = color;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                    break;
                case ECARules4AllType.Float:
                    value = SerializeUtils.ConvertStringToParameter(typeof(float), json);
                    break;
                case ECARules4AllType.Integer:
                    value = SerializeUtils.ConvertStringToParameter(typeof(int), json);
                    break;
                case ECARules4AllType.Position:
                    value = SerializeUtils.ConvertStringToParameter(typeof(Position), json);
                    break;
                case ECARules4AllType.Path:
                    value = SerializeUtils.ConvertStringToParameter(typeof(Path), json);
                    break;
                case ECARules4AllType.Identifier:
                    //Unused?
                    break;
                case ECARules4AllType.Rotation:
                    value = SerializeUtils.ConvertStringToParameter(typeof(Rotation), json);
                    break;
                case ECARules4AllType.Text:
                    value = SerializeUtils.ConvertStringToParameter(typeof(string), json);
                    break;
                case ECARules4AllType.Time:
                    //Unused?
                    break;
                case ECARules4AllType.Scale:
                    SerializeUtils.ConvertStringToParameter(typeof(Scale), json);
                    break;
            }

            return value;
        }

        
        
    }
    
}