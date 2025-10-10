using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.SmartHomeHubClients
{
    public class ContentNotification
    {
        [JsonProperty("timestamp")] 
        public long timestamp { get; private set; }

        [JsonProperty("json_content")] public Dictionary<string, object> jsonContent { get; private set; }

        public ContentNotification(string entity, string attribute, object newValue)
        {
            var newValueSerialized = SerializeUtils.SerializeAttribute(newValue);
            
            var data = new Dictionary<string, object>
            {
                { "unity_id", entity },
                { "attribute", attribute },
                { "new_value", newValueSerialized }
            };
            
            this.jsonContent = data;
            this.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        
        public ContentNotification(string componentName, Action action)
        {
            var data = new Dictionary<string, object>
            {
                { "unity_id", action.GetSubject().name },
                { "verb", action.GetActionMethod() }
            };
            // object
            if (action.GetObject() != null)
            {
                var obj = action.GetObject();
                if (obj is string)
                {
                    data["variable"] = (string)obj;
                    data["modifier"] = action.GetModifier();
                    data["value"] = action.GetModifierValue().ToString();
                }
                else
                {
                    string objString;
                    
                    if (obj is GameObject)
                    {
                        objString = ((GameObject)obj).name;
                    }
                    else
                    {
                        objString = obj.ToString();
                    }
                    
                    data["obj"] = objString;
                }
                
            }

            this.jsonContent = data;
            this.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, this.jsonContent);
        }
    }
    
    public class NotificationQueue
    {
        private Queue<ContentNotification> queue = new Queue<ContentNotification>();
        
        public event EventHandler<ContentNotification> ItemAdded;
        
        public void Enqueue(ContentNotification item)
        {
            queue.Enqueue(item);
            ItemAdded?.Invoke(this, item);
        }
        
        public ContentNotification Dequeue()
        {
            return queue.Dequeue();
        }
    }
}