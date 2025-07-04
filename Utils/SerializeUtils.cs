using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using ECARules4All_DLL.Taxonomies.Objects.Interactions.Subcategories;
using Newtonsoft.Json;
using Serilog;
using Scenes_Scene = ECARules4All_DLL.Taxonomies.Objects.Scenes.Scene;
using UnityEngine;


namespace ECARules4All_DLL.Utils
{
    public class SerializeUtils
    {
        public static object SerializeAttribute(object value)
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
        
        public static object ConvertStringToParameter(Type typeParameter, string receivedParameter)
        {
            object parameter = null;
            
            if (typeParameter == typeof(Position))
            {
                parameter = new Position(JsonConvert.DeserializeObject<Vector3>(receivedParameter));
            }
            else if (typeParameter == typeof(Rotation))
            {
                parameter = new Rotation(Quaternion.Euler(JsonConvert.DeserializeObject<Vector3>(receivedParameter)));
            }
            else if (typeParameter == typeof(Path))
            {
                var matches = Regex.Matches(receivedParameter, @"\{[^{}]+\}");
                List<Position> positions = new List<Position>();
                foreach (Match match in matches)
                {
        	        positions.Add(new Position(JsonConvert.DeserializeObject<Vector3>(match.Value)));
                }
                parameter = new Path(positions);
            } else if(typeParameter == typeof(Scale))
            {
                parameter = new Scale(JsonConvert.DeserializeObject<Vector3>(receivedParameter));
            }
            else if (typeParameter == typeof(ECABoolean))
            {
                parameter = ECABoolean.FromString(receivedParameter);
            }
            if (typeParameter == typeof(float) || typeParameter == typeof(Single))
            {
                parameter = float.Parse(receivedParameter);
            }
            else if (typeParameter == typeof(double) || typeParameter == typeof(Double))
            {
                parameter = double.Parse(receivedParameter);
            }
            else if (typeParameter == typeof(int) || typeParameter == typeof(Int32))
            {
                parameter = int.Parse(receivedParameter);
            }

            else if (typeParameter == typeof(string))
            {
                parameter = receivedParameter;
            }
            else
            {
                string message = $"Error on converting {receivedParameter} to {typeParameter}";
                Log.Error(message);
                throw new Exception(message);
            }

            return parameter;
        }
        
        public static object ConvertObjectToParameter(Type typeParameter, object receivedParameter)
        {
            object parameter = null;
            
            if (typeParameter == typeof(Position))
            {
                parameter = new Position(JsonConvert.DeserializeObject<Vector3>(receivedParameter.ToString()));
            }
            else if (typeParameter == typeof(Rotation))
            {
                parameter = new Rotation(Quaternion.Euler(JsonConvert.DeserializeObject<Vector3>(receivedParameter.ToString())));
            }
            else if (typeParameter == typeof(Path))
            {
                var matches = Regex.Matches(receivedParameter.ToString(), @"\{[^{}]+\}");
                List<Position> positions = new List<Position>();
                foreach (Match match in matches)
                {
                    positions.Add(new Position(JsonConvert.DeserializeObject<Vector3>(match.Value)));
                }
                parameter = new Path(positions);
            } else if(typeParameter == typeof(Scale))
            {
                parameter = new Scale(JsonConvert.DeserializeObject<Vector3>(receivedParameter.ToString()));
            }
            else if (typeParameter == typeof(ECABoolean))
            {
                parameter = ECABoolean.FromString(receivedParameter.ToString());
            }
            if (typeParameter == typeof(float) || typeParameter == typeof(Single))
            {
                parameter = float.Parse(receivedParameter.ToString());
            }
            else if (typeParameter == typeof(double) || typeParameter == typeof(Double))
            {
                parameter = double.Parse(receivedParameter.ToString());
            }
            else if (typeParameter == typeof(int) || typeParameter == typeof(Int32))
            {
                parameter = int.Parse(receivedParameter.ToString());
            }
            else if (typeParameter == typeof(string))
            {
                parameter = receivedParameter;
            }
            else
            {
                string message = $"Error on converting {receivedParameter} to {typeParameter}";
                Log.Error(message);
                throw new Exception(message);
            }

            return parameter;
        }
    }
}