using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Vector3 = UnityEngine.Vector3;

namespace ECARules4All_DLL.SmartHomeHubClients
{
    public class HomeAssistantClient : AbstractClient<HomeAssistantClient>
    {
	    protected override async void SendNotification(object sender, ContentNotification newItem)
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
                    Debug.LogError($"An error occured during an update request - {e.Message}");
                }
            }
        }

        public async void ReceivedUpdateHandler(object sender, ReceivedUpdate receivedUpdate)
        {
	        Debug.Log($"{receivedUpdate.subject} - {receivedUpdate.verb} - {receivedUpdate.parameters}");
			if (ComponentTracker.Instance.GetAllComponents().ContainsKey(receivedUpdate.subject))
			{
				// GameObject.Tag@ECAScript.Name example => T_Shirt_1@ECAObject
				string[] names = receivedUpdate.subject.Split('@'); 
				Type ecaScript = FindTypeByName(names[1]);
				if (ecaScript != null)
				{
					var gameObject = GameObject.Find(names[0]);
					
					MethodInfo methodInfo = FindMethodWithVerb(ecaScript, receivedUpdate.verb);

					if (methodInfo.GetParameters().Length == 0)
					{
						RuleEngine.GetInstance().ExecuteAction(
							new Action(gameObject, receivedUpdate.verb)
						);
						Debug.Log($"Action {receivedUpdate.verb} with no parameter runned");
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
							var action = new Action(gameObject, receivedUpdate.verb, parameter);
							RuleEngine.GetInstance().ExecuteAction(
								action
							);
							Debug.Log($"Action {receivedUpdate.verb} with no variable runned");
						}
						else
						{
							var action = new Action(gameObject, receivedUpdate.verb, receivedUpdate.variable,
								receivedUpdate.modifier, parameter);
							RuleEngine.GetInstance().ExecuteAction(
								action
							);
							Debug.Log($"Action {receivedUpdate.verb} with variable runned");
						}
					}
					Debug.Log($"Reflection method (aka Action) {methodInfo.Name} performed");	
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
	        else if (typeParameter == typeof(float) || typeParameter == typeof(int))
	        {
		        parameter = float.Parse(receivedParameter);
	        }

	        return parameter;
        }

        protected override async void RegisterVirtualObject(object sender, List<ComponentTrackerPair> pairs)
        {
	        List<object> payload = new List<object>();
	        
	        foreach (var pair in pairs)
	        {
		        var attribute = pair.GetAttributes();
		        
		        var p = new
		        {
			        eca_script = pair.GetECAScript(),
			        game_object = pair.GetName(),
			        unity_id = pair.GetName(),
			        attributes = attribute.Count > 0 ? attribute : null
		        };

		        var settings = new JsonSerializerSettings
		        {
			        NullValueHandling = NullValueHandling.Ignore
		        };

		        payload.Add(p);
	        }
	        var options = new JsonSerializerOptions
	        {
		        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	        };
	        string jsonBody = JsonSerializer.Serialize(new {pairs = payload}, options);
	        
	        using (HttpClient client = new HttpClient())
	        {
		        try
		        {
			        var stringContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
			        string urlService = $"{this.url}/api/services/eud4xr/add_virtual_object";
			        string requestBody = await stringContent.ReadAsStringAsync();
			        client.DefaultRequestHeaders.Authorization =
				        new AuthenticationHeaderValue("Bearer", this.token);
			        HttpResponseMessage response = await client.PostAsync(urlService, stringContent);
			        response.EnsureSuccessStatusCode();

			        Debug.Log($"Registered a new virtual object to Home Assistant Client at {this.url}");
		        }
		        catch (HttpRequestException e)
		        {
			        Debug.LogError($"An error occured while registering a new virtual object - {e.Message}");
		        }
	        }
        }
    }
}
