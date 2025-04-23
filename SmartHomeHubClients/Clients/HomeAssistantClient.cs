using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json.Linq;
using Serilog;


namespace ECARules4All_DLL.SmartHomeHubClients.Clients
{
    public class HomeAssistantClient : AbstractClient<HomeAssistantClient>
    {
	    private static class URLS
	    {
		    public static readonly string SEND_NOTIFICATION = "/api/services/eud4xr/receive_update_from_unity";
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
            string jsonBody = JsonConvert.SerializeObject(new
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
	        var options = new JsonSerializerSettings
	        {
		        NullValueHandling = NullValueHandling.Ignore
	        };
	        string jsonBody = JsonConvert.SerializeObject(new {pairs = payload}, options);
	        
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

        public override void RegisteredAutomations(object sender, List<AutomationDTO> automations)
        {
	        registeredAutomations = this.ConvertAutomationsToRules(automations);
        }
        
        public override async Task<List<Rule>> GetListAutomations()
        {
	        List<Rule> rules = new List<Rule>();
	        
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
			        JObject jsonObject = JObject.Parse(jsonResponse);
			        var a = jsonObject["automations"]?.ToString() ;
			        Log.Information($"Received Automations: {a}");
			        List<AutomationDTO> automations = JsonConvert.DeserializeObject<List<AutomationDTO>>(jsonObject["automations"]?.ToString() ?? throw new InvalidOperationException());
			        rules = this.ConvertAutomationsToRules(automations);
			        
			        Log.Information($"Receive automations list by contacting {this.url}");
		        }
		        catch (HttpRequestException e)
		        {
			        Log.Error($"An error occured while receiving the list of registered automations on home assistant - {e.Message}");
		        }
	        }

	        return rules;
        }

        private List<Rule> ConvertAutomationsToRules(List<AutomationDTO> automations)
        {
	        var rules = new List<Rule>();
	        foreach (var a in automations)
	        {
		        rules.Add(a.ConvertToRule());
	        }
	        return rules;
        }
    }
}
