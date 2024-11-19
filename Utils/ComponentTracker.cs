using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories;
using Serilog;
using UnityEngine;
using Scenes_Scene = ECARules4All_DLL.Taxonomies.Objects.Scenes.Scene;


namespace ECARules4All_DLL.Utils
{
    public class SerializeAttributeUtils
    {
        public static object SerializeAttributes(object value)
        {
            object processedValue = null;
            switch (value) 
            {
                case string stringValue: processedValue = stringValue; break;
                case int intValue: processedValue = intValue; break;
                case float floatValue: processedValue = floatValue; break;
                case double doubleValue: processedValue = doubleValue; break;
                case bool boolValue: processedValue = boolValue; break;
                case ECABoolean ecaBooleanValue: processedValue = ecaBooleanValue.ToString(); break;
                case Position positionValue: processedValue = positionValue.ToDict(); break;
                case Rotation rotationValue: processedValue = rotationValue; break;
                case Path pathValue: processedValue = pathValue.ToDict(); break;
                case Scale scaleValue: processedValue = scaleValue; break;
                case Color colorValue: processedValue = colorValue.ToString(); break;
                case Mesh meshValue: processedValue = meshValue.ToString(); break; 
                case Scenes_Scene scene_sceneValue: processedValue = scene_sceneValue; break;
                case DateTime dateTimeValue: processedValue = dateTimeValue.ToString(); break;
                case ECACamera.POV povValue: processedValue = povValue.ToString(); break;
                            
                default:
                    Log.Warning($"Type {value.GetType().ToString()} does not recognized");
                    break;
            }

            return processedValue;
        }
    }
    
    public class ComponentTrackerPair
    {
        private string name;
        private object sceneObject;

        public ComponentTrackerPair(string name, object sceneObject)
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

        public Dictionary<string, object> GetAttributes()
        {
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            
            Type objectType = sceneObject.GetType();
            var members = objectType.GetMembers()
                .Where(m => m.GetCustomAttributes(typeof(StateVariableAttribute), true).Length > 0);
            
            foreach (var member in members)
            {
                object value = member.MemberType is MemberTypes.Property
                    ? ((PropertyInfo)member).GetValue(sceneObject)
                    : ((FieldInfo)member).GetValue(sceneObject);
                object processedValue = null;
                
                //Log.Information($"{member.Name} - {value}");
                
                if (value != null) 
                {
                    /*switch (value) 
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
                            Log.InformationWarning($"Tipo sconosciuto per l'attributo {member.Name}");
                            break;
                    }*/
                    processedValue = SerializeAttributeUtils.SerializeAttributes(value);
                }
                
                var memberStateVariable = member.GetCustomAttribute<StateVariableAttribute>();
                attributes.Add(memberStateVariable.Name.Replace("-", "_"), processedValue);
                //Log.Information($"{this.GetName()} - Aggiunto attributo {member.Name}: {processedValue}");
            }

            // LOG
            //Log.Information($"{this.GetName()} - Return");
            return attributes;
        }
    }
    
    public class ComponentTracker
    {
        private static ComponentTracker _instance;
        private Dictionary<string, object> _components = new Dictionary<string, object>();
        public event EventHandler<List<ComponentTrackerPair>> NewRegisteredComponents;
        
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

        public void AddPairs(List<ComponentTrackerPair> pairs)
        {
            foreach (var pair in pairs)
            {
                string pairName = pair.GetName();
                if (!_components.ContainsKey(pairName))
                {
                    _components[pairName] = pair.GetSceneObject();
                }
                else
                {
                    Log.Information($"Error {pairName}");
                    pairs.Remove(pair);
                } 
            }
            NewRegisteredComponents?.Invoke(this, pairs);
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