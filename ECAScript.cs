using System.Reflection;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL
{
    public abstract class ECAScript : MonoBehaviour
    {
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
        
        protected virtual void Start()
        {
            string componentName = GetType().Name;
            ComponentTracker.Instance.AddComponent(gameObject.name, componentName, this);
        }

        protected virtual void OnDestroy()
        {
            string componentName = GetType().Name;
            ComponentTracker.Instance.RemoveComponent(gameObject.name, componentName, this);
        }
    }
}
