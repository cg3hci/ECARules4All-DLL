using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using Serilog;
using Object = UnityEngine.Object;


namespace ECARules4All_DLL.SmartHomeHubClients
{
    public class APIServer
    {
        private string _url;
        private int _port;
        private HttpListener _listener;
        public event EventHandler<ActionDTO> ActionUpdate;
        public event EventHandler<List<AutomationDTO>> RegisteredAutomations;

        private string apiExternalUpdates = $"/api/external_updates/";
        private string apiAutomations = $"/api/automations/";
        private string apiTest = $"/api/test/";
        private string apiForceRestart = $"/api/force_restart/";


        public APIServer(string url, int port)
        {
            // if urls doesn't start with http:// or https://, add http://
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "http://" + url;
            }

            _url = url;
            _port = port;
            _listener = new HttpListener();
            this._listener.Prefixes.Add($"{this._url}:{this._port}{this.apiExternalUpdates}");
            this._listener.Prefixes.Add($"{this._url}:{this._port}{this.apiAutomations}");
            this._listener.Prefixes.Add($"{this._url}:{this._port}{this.apiTest}");
            this._listener.Prefixes.Add($"{this._url}:{this._port}{this.apiForceRestart}");
            this.Start();
        }
        
        public APIServer() : this(GetLocalIPAddress(), 8080)
        {
            // Default constructor that initializes with localhost and port 8080
        }
        
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "No IPv4 address found";
        }

        public void Start()
        {
            _listener.Start();
            Receive();
        }

        public void Stop()
        {
            _listener.Stop();
            Log.Information($"Server stopped.");
        }

        private async void Receive()
        {
            while (_listener.IsListening)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    HandleRequest(context);
                }
                catch (HttpListenerException) when (!_listener.IsListening)
                {
                    break;
                }
            }
        }
        
        private void HandleRequest(HttpListenerContext context)
        {
            string path = context.Request.Url.AbsolutePath;

            if (context.Request.HttpMethod == "POST")
            {
                if (path.Contains(this.apiExternalUpdates))
                {
                    this.HandleExternalUpdates(context);
                }
                else if(path.Contains(this.apiAutomations))
                {
                    this.HandleAutomations(context);
                }
                else if(path.Contains(this.apiForceRestart))
                {
                    this.HandleForceRestart(context);
                }
                else if(path.Contains(this.apiTest))
                {
                    this.HandleTest(context);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.Close();
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                context.Response.Close();
            }
        }
        
        private void HandleTest(HttpListenerContext context)
        {
            using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                // Notify message
                var responseObject = new
                {
                    message = "Success",
                    timestamp = DateTime.Now,
                };
                string jsonResponse = JsonConvert.SerializeObject(responseObject);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonResponse);

                // Set status code before writing to the stream
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "application/json";
                context.Response.ContentLength64 = buffer.Length;

                // Write to the output stream
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.Close();
            }
        }
        
        private void HandleExternalUpdates(HttpListenerContext context)
        {
            using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                string requestBody = reader.ReadToEnd();
                Log.Information($"Received POST data on {this.apiExternalUpdates}: {requestBody}");

                try
                {
                    ActionDTO data = JsonConvert.DeserializeObject<ActionDTO>(requestBody);
                    ActionUpdate?.Invoke(this, data);

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.Close();
                }
                catch (Exception ex)
                {
                    Log.Information($"Error processing POST data: {ex.Message}");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.Close();
                }
            }
        }

        private void HandleAutomations(HttpListenerContext context)
        {
            using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                string requestBody = reader.ReadToEnd();
                Log.Information($"Received POST data on {this.apiAutomations}: {requestBody}");
                
                try
                {
                    List<AutomationDTO> data = JsonConvert.DeserializeObject<List<AutomationDTO>>(requestBody);
                    RegisteredAutomations?.Invoke(this, data);

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.Close();
                }
                catch (Exception ex)
                {
                    Log.Information($"Error processing POST data: {ex.Message}");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.Close();
                }
            }
        }
        
        private void HandleForceRestart(HttpListenerContext context)
        {
            Boolean success = true;

            try
            {
                // clean component tracker dictionary
                ComponentTracker.Instance.RemoveAllComponents();

                // force object subscription in hass
                foreach (var gameObject in Object.FindObjectsByType<ECATracker>(UnityEngine.FindObjectsSortMode.None))
                {
                    gameObject.SubscribeObject();
                }
            }catch (Exception e)
            {
                success = false;
            }
            
            // Notify message
            var responseObject = new
            {
                message = success ? "Success" : "Error",
                timestamp = DateTime.Now,
            };
            string jsonResponse = JsonConvert.SerializeObject(responseObject);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonResponse);

            // Set status code before writing to the stream
            context.Response.StatusCode = success ? (int)HttpStatusCode.OK : (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = buffer.Length;

            // Write to the output stream
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
        }
    }
}