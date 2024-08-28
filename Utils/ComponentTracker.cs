using System.Collections.Generic;

namespace ECARules4All_DLL.Utils
{
    public class ComponentTracker
    {
        private static ComponentTracker _instance;
        private Dictionary<string, object> _components = new Dictionary<string, object>();

        private ComponentTracker()
        {
        
        }
    
        public static ComponentTracker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ComponentTracker();
                }
                return _instance;
            }
        }

        public void AddComponent(string pair, object component)
        {
            if (!_components.ContainsKey(pair))
            {
                _components[pair] = component;
            }
        }

        public void RemoveComponent(string pair, object component)
        {
            if (_components.ContainsKey(pair))
            {
                _components.Remove(pair);
            }
        }
    
        public Dictionary<string, object> GetAllComponents()
        {
            return _components;
        }
    }
}