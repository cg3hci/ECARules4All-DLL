using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL.SmartHomeHubClients
{
    public class Update
    {
        [JsonPropertyName("unity_id")]
        public string entity { get; set; }
        [JsonPropertyName("attribute")]
        public string attribute { get; set; }
        [JsonPropertyName("new_value")]
        public object newValue { get; set; }
        /*[JsonIgnore]
        public long ts;
        [JsonIgnore]
        public bool copiedFlag = false;*/

        public Update(string entity, string attribute, object newValue)
        {
            this.entity = entity;
            this.attribute = attribute;
            this.newValue = newValue;
        }
        
        /*public object Clone()
        {
            var copy = new Update(this.entity, this.attribute, this.newValue);
            DateTimeOffset now = DateTimeOffset.Now;
            long unixTimestamp = now.ToUnixTimeSeconds();
            copy.copiedFlag = true;
            copy.ts = unixTimestamp;
            this.ts = unixTimestamp;
            this.copiedFlag = true;
            return copy;
        }*/

        /*public bool Equals(Update other)
        {
            if (other == null)
                return false;
            return entity == other.entity && attribute == other.attribute && newValue == other.newValue && ts == other.ts;
        }*/
    }
    
    public class UpdateQueue
    {
        private Queue<Update> queue = new Queue<Update>();
        //private List<Update> queue = new List<Update>();
        
        public event EventHandler<Update> ItemAdded;
        
        public void Enqueue(Update item)
        {
            queue.Enqueue(item);
            //queue.Add(item);
            OnItemAdded(item);
        }
        
        public Update Dequeue()
        {
            return queue.Dequeue();
        }

        /*public void Remove(Update item)
        {
            if (item.copiedFlag)
            {
                queue.Remove(item);
            }
            else
            {
                throw new Exception("Impossible to remove an item with copiedFlag = false");
            }
        }
        
        public void Remove(IEnumerable<Update> items)
        {
            foreach (var item in items)
            {
                this.Remove(item);
            }
        }

        public void RemoveAll()
        {
            this.queue = new List<Update>();
        }

        public List<Update> GetUpdates()
        {
           var listUpdates = this.queue.Where(item => !item.copiedFlag || item.ts == default(long))
                .Select(item => 
                {
                    item.copiedFlag = true;
                    item.ts = DateTimeOffset.Now.ToUnixTimeSeconds();
                    return (Update)item;
                })
                .ToList();
           return listUpdates;
        }*/

        protected virtual void OnItemAdded(Update item)
        {
            ItemAdded?.Invoke(this, item); //new UpdateAddedEventArgs(item, this));
        }
    }
    
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
