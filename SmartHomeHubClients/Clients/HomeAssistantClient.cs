using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using UnityEngine;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Serilog;


namespace ECARules4All_DLL.SmartHomeHubClients.Clients
{
    public class HomeAssistantClient : AbstractClient<HomeAssistantClient>
    {
	    private static class URLS
	    {
		    public static readonly string SEND_NOTIFICATION = "";
		    public static readonly string REGISTER_VIRTUAL_OBJECT = "/api/services/eud4xr/add_virtual_object";
		    public static readonly string AUTOMATIONS = "/api/eud4xr/automations";
	    }
	    
	    protected override async void SendNotification(object sender, ContentNotification newItem)
        {
            if (string.IsNullOrEmpty(this.token) || string.IsNullOrEmpty(this.url))
            {
                Log.Information("Token or url is empty");
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
                    string urlService = $"{this.url}{URLS.SEND_NOTIFICATION}";
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", this.token);
                    HttpResponseMessage response = await client.PostAsync(urlService, body);
                    response.EnsureSuccessStatusCode();
                    
                    Log.Information("Sent update {@Content}", jsonBody);
                    Log.Information($"Sent an update to Home Assistant Client at {this.url} - content: {lastContent.jsonContent} - timestamp: {lastContent.timestamp}");
                    // delete list content
                    //this.updates.Remove(lastContent);
                }
                catch (HttpRequestException e)
                {
                    Log.Error($"An error occured during an update request - {e.Message}");
                }
            }
        }
	    
        public async void ReceivedUpdateHandler(object sender, ActionDTO receivedUpdate)
        {
	        Action action = receivedUpdate.ConvertToAction();

	        if (action == null)
	        {
		        Log.Error($"Error on handling the action '{receivedUpdate.verb}' of the subject '{receivedUpdate.subject}'");
	        }
	        else
	        {
		        RuleEngine.GetInstance().ExecuteAction(
			        action
		        );
		        Log.Information($"Reflection method (aka Action) {action.GetActionMethod()} performed");
	        }
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
			        string urlService = $"{this.url}{URLS.REGISTER_VIRTUAL_OBJECT}";
			        string requestBody = await stringContent.ReadAsStringAsync();
			        client.DefaultRequestHeaders.Authorization =
				        new AuthenticationHeaderValue("Bearer", this.token);
			        HttpResponseMessage response = await client.PostAsync(urlService, stringContent);
			        response.EnsureSuccessStatusCode();
					
			        Log.Information($"Registered a new virtual object to Home Assistant Client at {this.url}");
		        }
		        catch (HttpRequestException e)
		        {
			        Log.Error($"An error occured while registering a new virtual object - {e.Message}");
		        }
	        }
        }

        protected override void RegisteredAutomations(object sender, List<AutomationDTO> automations)
        {
	        var rules = new List<Rule>();
	        foreach (var a in automations)
	        {
		        rules.Add(a.ConvertToRule());
	        }
	        registereAutomations = rules;
        }

        /*protected override async Task<List<AutomationDTO>> GetAutomations()
        {
	        List<AutomationDTO> automations = new List<AutomationDTO>();
	        
	        using (HttpClient client = new HttpClient())
	        {
		        try
		        {
			        string urlService = $"{this.url}{URLS.AUTOMATIONS}";
			        client.DefaultRequestHeaders.Authorization =
				        new AuthenticationHeaderValue("Bearer", this.token);
			        HttpResponseMessage response = await client.GetAsync(urlService);
			        response.EnsureSuccessStatusCode();
			        
			        // get automations and convert them to rules
			        string jsonResponse = await response.Content.ReadAsStringAsync();
			        var options = new JsonSerializerOptions
			        {
				        PropertyNameCaseInsensitive = true
			        };
			        automations = JsonSerializer.Deserialize<List<AutomationDTO>>(
				        jsonResponse, 
				        options
			        );
			        foreach (var automation in automations)
			        {
				        automation.ConvertToRule();
			        }
			        
			        Log.Information($"Receive automations list by contacting {this.url}");
		        }
		        catch (HttpRequestException e)
		        {
			        Log.Error($"An error occured while receiving the list of registered automations on home assistant - {e.Message}");
		        }
	        }

	        return automations;
        }*/
    }
}
