using System;
using System.Reflection;
using ECARules4All_DLL.Utils;
using UnityEngine;


namespace ECARules4All_DLL
{
    public class ECAScript : MonoBehaviour
    {
        protected string GetGameObjectComponentName()
        {
            return $"{gameObject.name}@{GetType().Name}"; // or GetType()}";
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
    }

    public class ECATracker : MonoBehaviour
    {
        private string GetGameObjectComponentName(object component)
        {
            return $"{gameObject.name}@{component.GetType().Name}"; // or GetType()}";
        }
        
        protected virtual void Start()
        {
            //string componentName = GetType().Name;
            //ComponentTracker.Instance.AddComponent(GetGameObjectComponentName(), this);
            foreach (var component in gameObject.GetComponents<Component>())
            {
                if (Attribute.IsDefined(component.GetType(), typeof(ECARules4AllAttribute)))
                {
                    ComponentTracker.Instance.AddComponent(GetGameObjectComponentName(component), this);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            //string componentName = GetType().Name;
            //ComponentTracker.Instance.RemoveComponent(GetGameObjectComponentName(), this);
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
