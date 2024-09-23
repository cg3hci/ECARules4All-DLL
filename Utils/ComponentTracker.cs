using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Nodes;
using UnityEngine;

namespace ECARules4All_DLL.Utils
{
    public class ComponentTracker
    {
        private static ComponentTracker _instance;
        private Dictionary<string, object> _components = new Dictionary<string, object>();

        private ComponentTracker() { }
    
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
        
        public event EventHandler<TrackedPair> ItemAdded;

        public void AddComponent(TrackedPair trackedPair)
        {
            if (!_components.ContainsKey(trackedPair.GetName()))
            {
                _components[trackedPair.GetName()] = trackedPair.GetSceneObject();
                
                // Lancia un evento per ogni elemento del dizionario [coppia string-object]
                ItemAdded?.Invoke(this, trackedPair);
            }
            else
            {
                // Segnala errore con un Log [messaggio da modificare]
                Debug.Log("Error");
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
    
    // Classe per coppie String - GameObject selezionate attraverso ComponentTracker
    public class TrackedPair
    {
        private string name;
        private object sceneObject;
        
        public TrackedPair (string name, object sceneObject)
        {
            this.name = name;
            this.sceneObject = sceneObject;
        }

        public string GetName() { return this.name; }
        public object GetSceneObject() { return this.sceneObject; }

        public JsonObject GetAttributes()
        {
            JsonObject jsonAttributes = new JsonObject();

            // Ottieni il tipo dell'oggetto sceneObject
            Type objectType = sceneObject.GetType();

            // Itera sui campi e sulle proprietà dell'oggetto
            foreach (var member in objectType.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                // Cerca gli attributi personalizzati di tipo StateVariableAttribute
                var stateVariableAttribute = member.GetCustomAttribute<StateVariableAttribute>();
                if (stateVariableAttribute != null)
                {
                    // Ottieni il valore del campo o della proprietà
                    object value = null;
                    if (member is FieldInfo field)
                    {
                        value = field.GetValue(sceneObject);
                    }
                    else if (member is PropertyInfo property)
                    {
                        value = property.GetValue(sceneObject);
                    }

                    // Aggiungi l'attributo e il suo valore al JsonObject
                    if (value != null)
                    {
                        jsonAttributes.Add(stateVariableAttribute.Name, JsonValue.Create(value));
                    }
                }
            }

            return jsonAttributes;
        }
    }
}