using System;
using System.Collections.Generic;
using System.Reflection;
using ECARules4All_DLL.SmartHomeHubClients;
using ECARules4All_DLL.Utils;
using UnityEngine;
using Serilog;


namespace ECARules4All_DLL
{
    
    public static class ECAScript
    {
        public static string GetGameObjectComponentName(MonoBehaviour component)
        {
            return $"{component.gameObject.name}@{component.GetType().Name}";
        }

        public static string GetGameObjectName(MonoBehaviour component)
        {
            return $"{component.name}";
        }
        
        public static StateVariableAttribute GetStateVariableProperty(MonoBehaviour component, string nameProperty)
        {
            var property = component.GetType().GetProperty(nameProperty);
            // return a StateVariableAttribute if the object contains a property named $"{nameProperty}" 
            if (property != null)
            {
                return property.GetCustomAttribute<StateVariableAttribute>();
            }

            return null;
        }
        
        public static void NotifyUpdate(MonoBehaviour component, string propertyName, object value)
        {
            var attribute = GetStateVariableProperty(component, propertyName);
            if (attribute != null && RuleEngine.GetInstance().clients.Count > 0)
            {
                AbstractClient<object>.NotifyAttribute(
                    GetGameObjectComponentName(component),
                    attribute.Name,
                    value
                );
            }
        }

        public static void NotifyUpdate(MonoBehaviour component, Action action)
        {
            if (RuleEngine.GetInstance().clients.Count > 0)
            {
                try
                {
                    AbstractClient<object>.NotifyAction(
                        GetGameObjectName(component),
                        action
                    );
                }
                catch (Exception e)
                {
                    Log.Information($"Error on NotifyUpdate - {e}");
                }
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
        
        protected void Start()
        {
            // existing clients
            foreach (var client in RuleEngine.GetInstance().clients)
            {
                SubscribeObject(RuleEngine.GetInstance(), client);                
            }
            
            // new clients
            RuleEngine.GetInstance().NewRegisteredClient += SubscribeObject;
        }

        public void SubscribeObject(object sender, AbstractClientBase client)
        {
            if (client == null)
                return;

            ComponentTracker.Instance.GetAllComponents();
            try
            {
                Log.Information($"[ECATracker - SubscribeObject] {this.gameObject.name} is starting to register its components.");
                List<ComponentTrackerPair> pairs = new List<ComponentTrackerPair>();
                foreach (var component in gameObject.GetComponents<Component>())
                {
                    if (Attribute.IsDefined(component.GetType(), typeof(ECARules4AllAttribute)))
                    {
                        ComponentTrackerPair componentTrackerPair = new ComponentTrackerPair(GetGameObjectComponentName(component), component);
                        pairs.Add(componentTrackerPair);
                        Log.Information($"[ECATracker - SubscribeObject] add {this.gameObject.name}'s component tracker pairs.");
                    }
                }
                ComponentTracker.Instance.AddPairs(pairs, client);
                Log.Information($"[ECATracker - SubscribeObject] {this.gameObject.name} completed registering its components.");
            }
            catch (Exception e)
            {
                Log.Information($"[ECATracker - SubscribeObject] Error: {e}");
            }
            
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
