using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories;
using UnityEngine;
using Scenes_Scene = ECARules4All_DLL.Taxonomies.Objects.Scenes.Scene;

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

        public TrackedPair(string name, object sceneObject)
        {
            this.name = name;
            this.sceneObject = sceneObject;
        }

        public string GetName()
        {
            return this.name;
        }

        public object GetSceneObject()
        {
            return this.sceneObject;
        }

        public string GetECAScript()
        {
            return this.sceneObject.GetType().Name;
        }

        // Versione vecchia con JsonObject [non funzionante]
        /*
        public JsonObject GetAttributes()
        {
            JsonObject jsonAttributes = new JsonObject();

            // Ottieni il tipo dell'oggetto sceneObject
            Type objectType = sceneObject.GetType();

            // LOG
            Debug.Log($"{this.GetName()} - Dentro GetAttributes");

            // Itera sui campi e sulle proprietà dell'oggetto
            foreach (var member in objectType.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                // LOG
                Debug.Log($"{this.GetName()} - Dentro foreach");

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

                        // LOG
                        Debug.Log($"{this.GetName()} - Dentro if (value != null)");
                    }
                }
            }

            // LOG
            Debug.Log($"{this.GetName()} - Return");
            return jsonAttributes;
        }*/

        // Versione nuova con dizionario [solo coppie <string, string>]
        /*
        public Dictionary<string, string> GetAttributes()
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>();

            // Ottieni il tipo dell'oggetto sceneObject
            Type objectType = sceneObject.GetType();

            // LOG
            Debug.Log($"{this.GetName()} - Dentro GetAttributes");

            // Itera sui campi e sulle proprietà dell'oggetto
            foreach (var member in objectType.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                Debug.Log($"{this.GetName()} - Dentro foreach");

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

                    // Aggiungi l'attributo e il suo valore al dizionario
                    if (value != null)
                    {
                        attributes.Add(stateVariableAttribute.Name, value.ToString());

                        Debug.Log($"{this.GetName()} - Dentro if (value != null)");
                    }
                }
            }

            Debug.Log($"{this.GetName()} - Return");
            return attributes;
        }*/

        // Versione aggiornata con accorgimenti tecnici
        public Dictionary<string, object> GetAttributes()
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            
            Type objectType = sceneObject.GetType();
            var members = objectType.GetMembers()
                .Where(m => m.GetCustomAttributes(typeof(StateVariableAttribute), true).Length > 0);
            
            // Itera sui campi e sulle proprietà dell'oggetto
            foreach (var member in members)
            {
                object value = member.MemberType is MemberTypes.Property
                    ? ((PropertyInfo)member).GetValue(sceneObject)
                    : ((FieldInfo)member).GetValue(sceneObject);
                object processedValue = null;
                
                //Debug.Log($"{member.Name} - {value}");
                
                if (value != null) 
                {
                    switch (value) 
                    {
                        case string stringValue: processedValue = stringValue; break;
                        case int intValue: processedValue = intValue; break;
                        case float floatValue: processedValue = floatValue; break;
                        case double doubleValue: processedValue = doubleValue; break;
                        case bool boolValue: processedValue = boolValue; break;
                        case ECABoolean ecaBooleanValue: processedValue = ecaBooleanValue.ToString(); break;
                        case Position positionValue: processedValue = positionValue; break;
                        case Rotation rotationValue: processedValue = rotationValue; break;
                        case Color colorValue: processedValue = colorValue.ToString(); break;
                        case Mesh meshValue: processedValue = meshValue.ToString(); break; 
                        case Scenes_Scene scene_sceneValue: processedValue = scene_sceneValue; break;
                        case DateTime dateTimeValue: processedValue = dateTimeValue.ToString(); break;
                        case ECACamera.POV povValue: processedValue = povValue.ToString(); break;
                        
                        default:
                            Debug.LogWarning($"Tipo sconosciuto per l'attributo {member.Name}");
                            break;
                    }
                }
                
                var memberStateVariable = member.GetCustomAttribute<StateVariableAttribute>();
                attributes.Add(memberStateVariable.Name.Replace("-", "_"), processedValue);
                Debug.Log($"{this.GetName()} - Aggiunto attributo {member.Name}: {processedValue}");
            }

            // LOG
            Debug.Log($"{this.GetName()} - Return");
            return attributes;
        }
    }
}