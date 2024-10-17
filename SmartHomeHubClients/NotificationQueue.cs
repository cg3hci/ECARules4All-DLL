using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using ECARules4All_DLL.Utils;
using UnityEngine;

namespace ECARules4All_DLL.SmartHomeHubClients
{
    public class ContentNotification
    {
        [JsonPropertyName("timestamp")] 
        public long timestamp { get; private set; }

        [JsonPropertyName("json_content")] public Dictionary<string, object> jsonContent { get; private set; }

        public ContentNotification(string entity, string attribute, object newValue)
        {
            //string jsonString = JsonSerializer.Serialize(newValue);
            //Dictionary<string, object> dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
            var newValueSerialized = SerializeAttributeUtils.SerializeAttributes(newValue);
            
            var data = new Dictionary<string, object>
            {
                { "unity_id", entity },
                { "attribute", attribute },
                { "new_value", newValueSerialized }
            };
            
            this.jsonContent = data;//JsonSerializer.Serialize(data);
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
                    
                    data["variable"] = objString;
                }
                
            }

            this.jsonContent = data;//JsonSerializer.Serialize(data);
            this.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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