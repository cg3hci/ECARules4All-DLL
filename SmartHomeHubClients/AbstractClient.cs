using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json.Linq;
using Serilog;


namespace ECARules4All_DLL.SmartHomeHubClients
{
    public abstract class AbstractClientBase
    {
        public string url { get; set; }
        public string token { get; set; }
        public NotificationQueue updates { get; } = new NotificationQueue();
        
        public List<object> registeredAutomations { get; set; }
        public List<Expression> registeredExpressions { get; set; }
    }
    
    public abstract class AbstractClient<T> : AbstractClientBase where T : class, new()
    {
        private static T _instance;

        protected AbstractClient()
        {
            this.updates.ItemAdded += SendNotification;
            ComponentTracker.Instance.NewRegisteredComponents += RegisterVirtualObject;
        }

        public static T GetInstance()
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }

        protected abstract void SendNotification(object sender, ContentNotification newItem);
        
        protected abstract void RegisterVirtualObject(object sender, List<ComponentTrackerPair> pairs);

        public abstract void RegisteredAutomations(object sender, List<AutomationDTO> automations);

        public abstract Task<List<object>> GetListAutomations();
        
        public abstract Task<JArray> GetListExpressions();

        
        public static void NotifyAttribute<T>(string ownerName, string propertyName, T newValue)
        {
            var content = new ContentNotification(ownerName, propertyName, newValue);
            Notify(content);
        }
        
        public static void NotifyAction(string componentName, Action action)
        {
            var content = new ContentNotification(componentName, action);
            Notify(content);
        }

        private static void Notify(ContentNotification content)
        {
            foreach (var client in RuleEngine.GetInstance().clients)
            {
                client.updates.Enqueue(content);
                Log.Information($"registered new value for client {client}");
            }
        }
    }
}
