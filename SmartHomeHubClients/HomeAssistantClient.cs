using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ECARules4All_DLL.SmartHomeHubClients
{
    public class HomeAssistantClient : AbstractClient<HomeAssistantClient>
    {
	    protected override async void sendNotification(object sender, ContentNotification newItem)
        {
            if (string.IsNullOrEmpty(this.token) || string.IsNullOrEmpty(this.url))
            {
                Debug.Log("Token or url is empty");
            }
            

            //var lastContent = this.updates.GetUpdates();
            //var lastContent = new List<Update> {this.updates.Dequeue()};
            var lastContent = this.updates.Dequeue();
            string jsonBody = JsonSerializer.Serialize(new
            {
	            content = lastContent.jsonContent,
	            timestamp = lastContent.timestamp
            }
            );

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var body = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    string urlService = $"{this.url}/api/services/eud4xr/receive_update_from_unity";
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", this.token);
                    HttpResponseMessage response = await client.PostAsync(urlService, body);
                    response.EnsureSuccessStatusCode();
                    
                    Debug.Log($"Sent an update to Home Assistant Client at {this.url} - content: {lastContent.jsonContent} - timestamp: {lastContent.timestamp}");
                    // delete list content
                    //this.updates.Remove(lastContent);
                }
                catch (HttpRequestException e)
                {
                    Debug.Log($"An error occured during an update request - {e.Message}");
                }
            }
        }

        public async void receivedUpdateHandler(object sender, ReceivedUpdate receivedUpdate)
        {
			if (ComponentTracker.Instance.GetAllComponents().ContainsKey(receivedUpdate.subject))
			{
				// GameObject.Tag@ECAScript.Name example => T_Shirt_1@ECAObject
				string[] names = receivedUpdate.subject.Split('@'); 
				Type ecaScript = FindTypeByName(names[1]);
				if (ecaScript != null)
				{
					var gameObject = GameObject.Find(names[0]);
					
					MethodInfo methodInfo = FindMethodWithVerb(ecaScript, receivedUpdate.verb);

					if (methodInfo == null)
					{
						RuleEngine.GetInstance().ExecuteAction(
							new Action(gameObject, receivedUpdate.verb)
						);
					}
					else
					{
						// retrieve parameter
						ParameterInfo methodParameter = methodInfo.GetParameters()[0];
						string parameterName = methodParameter.Name;
						string receivedParameter = receivedUpdate.parameters[parameterName].ToString();
						object parameter = ConvertParameter(methodParameter, receivedParameter);
						
						// run action
						if (String.IsNullOrEmpty(receivedUpdate.variable))
						{
							RuleEngine.GetInstance().ExecuteAction(
								new Action(gameObject, receivedUpdate.verb, parameter)
							);
						}
						else
						{
							RuleEngine.GetInstance().ExecuteAction(
								new Action(gameObject, receivedUpdate.verb, receivedUpdate.variable, receivedUpdate.modifier, parameter)
							);
						}
					}
					Debug.Log($"Action {receivedUpdate.verb} performed");	
				}
			}
        }

        private static object ConvertParameter(ParameterInfo methodParameter, string receivedParameter)
        {
	        var typeParameter = methodParameter.ParameterType;
	        object parameter = null;
	        
	        if (typeParameter == typeof(Position))
	        {
		        parameter = new Position(JsonConvert.DeserializeObject<Vector3>(receivedParameter));
		        //parameter = new Position(JsonSerializer.Deserialize<Vector3>(receivedParameter));
	        }
	        else if (typeParameter == typeof(Rotation))
	        {
		        parameter = new Rotation(JsonConvert.DeserializeObject<Quaternion>(receivedParameter));
		        //parameter = new Rotation(JsonSerializer.Deserialize<Quaternion>(receivedParameter));
	        }
	        else if (typeParameter == typeof(Path))
	        {
		        var matches = Regex.Matches(receivedParameter, @"\{[^{}]+\}");
		        List<Position> positions = new List<Position>();
		        foreach (Match match in matches)
		        {
			        positions.Add(new Position(JsonConvert.DeserializeObject<Vector3>(match.Value)));
			        //positions.Add(new Position(JsonSerializer.Deserialize<Vector3>(match.Value)));
		        }
		        parameter = new Path(positions);
	        }
	        else if (typeParameter == typeof(float) || typeParameter == typeof(int))
	        {
		        parameter = float.Parse(receivedParameter);
	        }

	        return parameter;
        }
        
        // Metodo gestore dell'evento [TrackedPair]
        protected override async void addNewSensor(object sender, TrackedPair component)
        {
	        Debug.Log($"Add sensor - {component.GetName()}");
	        var attribute = component.GetAttributes();

	        // Creazione Payload in formato JSON
	        var payload = new {
                eca_script = component.GetECAScript(),
                game_object = component.GetName(),
                unity_id = component.GetName(),
                attributes = attribute.Count > 0 ? attribute : null
            };

	        /* var options = new JsonSerializerOptions
	        {
		        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	        }; */
	        
	        var settings = new JsonSerializerSettings
	        {
		        NullValueHandling = NullValueHandling.Ignore
	        };


	        // string jsonPayload = JsonSerializer.Serialize(payload, options);
	        string jsonPayload = JsonConvert.SerializeObject(payload, settings);
	        
	        //Debug.Log(payload.eca_script);
	        //Debug.Log(payload.game_object);
	        //Debug.Log(payload.unity_id);
	        //Debug.Log(payload.attributes);
	        Debug.Log(jsonPayload);
	        
			using (HttpClient client = new HttpClient()) {
				try
				{
					// Creazione della richiesta POST
					var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
					
					// Impostazione dell'URL dell'API di HomeAssistant
					string urlService = $"{this.url}/api/services/eud4xr/add_sensor";
					
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
					HttpResponseMessage response = await client.PostAsync(urlService, content);
					response.EnsureSuccessStatusCode();
                    
					Debug.Log($"Registered a new TrackedPair to Home Assistant Client at {this.url}");
				} 
				catch (HttpRequestException e) {
					Debug.Log($"An error occured during an update request - {e.Message}");
				}
			}
        }
    }
}
