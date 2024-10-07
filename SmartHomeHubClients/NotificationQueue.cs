using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

namespace ECARules4All_DLL.SmartHomeHubClients
{
    public class ContentNotification
    {
        [JsonPropertyName("timestamp")] 
        public long timestamp;

        [JsonPropertyName("json_content")] 
        public Dictionary<string, object> jsonContent;
        
        public ContentNotification(string entity, string attribute, object newValue)
        {
            string jsonString = JsonSerializer.Serialize(newValue);
            Dictionary<string, object> dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
            
            var data = new Dictionary<string, object>
            {
                { "unity_id", entity },
                { "attribute", attribute },
                { "new_value", dictionary }
            };
            
            this.jsonContent = data;//JsonSerializer.Serialize(data);
            this.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        
        public ContentNotification(string componentName, Action action)
        {
            string subject = componentName;//$"{action.GetSubject().name}@{componentName}";
            
            var data = new Dictionary<string, object>
            {
                { "unity_id", subject },
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