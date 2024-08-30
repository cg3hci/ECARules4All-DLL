using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.SmartHomeHubClients
{
    public abstract class AbstractClientBase
    {
        public string url { get; set; }
        public string token { get; set; }
        public UpdateQueue updates { get; } = new UpdateQueue();
    }
    
    public abstract class AbstractClient<T> : AbstractClientBase where T : class, new()
    {
        private static T _instance;

        protected AbstractClient()
        {
            this.updates.ItemAdded += sendRequestHandler;
        }

        public static T GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }

        protected abstract void sendRequestHandler(object sender, Update newItem);
        
        public static Type FindTypeByName(string className)
        {
            /*Type type = Type.GetType(className);
            if (type == null)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                type = assembly.GetType(className);
            }

            return type;*/
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type type = assembly.GetTypes().FirstOrDefault(t => t.Name == className);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        public static MethodInfo FindMethodWithVerb(Type targetType, string verb, string variable = null)
        {
            MethodInfo[] methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            List<MethodInfo> fMethods = new List<MethodInfo>();
            foreach (var method in methods)
            {
                // Check if the method has the ECAActionAttribute
                var attribute = method.GetCustomAttribute<ActionAttribute>();
                if (attribute != null)
                {
                    bool verbComparison = attribute.Verb.Equals(verb, StringComparison.OrdinalIgnoreCase);
                    // Compare the second argument (string) with the given input string
                    if (verbComparison && String.IsNullOrEmpty(variable) || 
                        verbComparison && attribute.variableName.Equals(variable, StringComparison.OrdinalIgnoreCase))
                    {
                        return method;
                    }
                }
            }

            return null;
        }
    }
    
    public static class UpdateValueWrapper
    {
        public static void UpdateValue<T>(string ownerName, string propertyName, T newValue)
        {
            foreach (var client in RuleEngine.GetInstance().clients)
            {
                client.updates.Enqueue(new Update(ownerName, propertyName, newValue));
                Debug.Log($"registered new value for client {client}");
            }
        }
    }
}
