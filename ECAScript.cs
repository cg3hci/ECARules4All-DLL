using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL
{
    
    public class ECAScript : MonoBehaviour
    {
        protected string GetGameObjectComponentName()
        {
            return $"{gameObject.name}@{GetType().Name}";
        }

        protected string GetGameObjectName()
        {
            return $"{gameObject.name}";
        }
        
        protected StateVariableAttribute GetStateVariableProperty(string nameProperty)
        {
            var property = GetType().GetProperty(nameProperty);
            // return a StateVariableAttribute if the object contains a property named $"{nameProperty}" 
            if (property != null)
            {
                return property.GetCustomAttribute<StateVariableAttribute>();
            }

            return null;
        }
        
        public void NotifyUpdate(string propertyName, object value)
        {
            var attribute = GetStateVariableProperty(propertyName);
            if (attribute  != null)
            {
                AbstractClient<object>.NotifyAttribute(
                    GetGameObjectComponentName(),
                    attribute.Name,
                    value
                );
            }
        }

        public void NotifyUpdate(Action action)
        {
            try
            {
                AbstractClient<object>.NotifyAction(
                    GetGameObjectName(),
                    action
                );
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
            
        }
    }
    
    [DefaultExecutionOrder(100)]
    public class ECATracker : MonoBehaviour
    {
        private string GetGameObjectComponentName(object component)
        {
            return $"{gameObject.name}@{component.GetType().Name}"; // or GetType()}";
        }
        
        protected virtual void Start()
        {
            List<ComponentTrackerPair> pairs = new List<ComponentTrackerPair>();
            foreach (var component in gameObject.GetComponents<Component>())
            {
                if (Attribute.IsDefined(component.GetType(), typeof(ECARules4AllAttribute)))
                {
                    ComponentTrackerPair componentTrackerPair = new ComponentTrackerPair(GetGameObjectComponentName(component), component);
                    //ComponentTracker.Instance.AddComponent(trackedPair);
                    pairs.Add(componentTrackerPair);
                }
            }
            ComponentTracker.Instance.AddPairs(pairs);
        }

        protected virtual void OnDestroy()
        {
            foreach (var component in gameObject.GetComponents<Component>())
            {
                if (Attribute.IsDefined(component.GetType(), typeof(ECARules4AllAttribute)))
                {
                    ComponentTracker.Instance.RemoveComponent(GetGameObjectComponentName(component), this);
                }
            }
        }
    }
}
