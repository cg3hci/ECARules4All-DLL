using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Serilog;


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
        private string apiTest = $"/test/";

        public APIServer(string url = "http://localhost",  int port = 8080)
        {
            _url = url;
            _port = port;
            _listener = new HttpListener();
            this._listener.Prefixes.Add($"{this._url}:{this._port}{this.apiExternalUpdates}");
            this._listener.Prefixes.Add($"{this._url}:{this._port}{this.apiAutomations}");
            this._listener.Prefixes.Add($"{this._url}:{this._port}{this.apiTest}");
            this.Start();
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
        
        private void HandleTest(HttpListenerContext context)
        {
            using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                var responseObject = new { 
                    message = "Success",
                    timestamp = DateTime.Now,
                };
                
                string jsonResponse = JsonConvert.SerializeObject(responseObject);
                context.Response.ContentType = "application/json";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonResponse);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                
                context.Response.Close();
            }
        }
    }
}