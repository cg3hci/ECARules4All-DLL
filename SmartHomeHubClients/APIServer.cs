using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ECARules4All_DLL.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using UnityEngine;


namespace ECARules4All_DLL.SmartHomeHubClients
{
    public class APIServer
    {
        private string _url;
        private int _port;
        private HttpListener _listener;
        public event EventHandler<ActionDTO> ActionUpdate;
        public event EventHandler<List<AutomationDTO>> RegisteredAutomations;
        public event EventHandler<List<Expression>> RegisteredExpressions;

        private string apiExternalUpdates = $"/api/external_updates/";
        private string apiAutomations = $"/api/automations/";
        private string apiExpressions = $"/api/expressions/";
        private string apiTest = $"/api/test/";
        private string apiForceRestart = $"/api/force_restart/";

        public APIServer(int port)
        {
            _port = port;
            _listener = new HttpListener();

            _listener.Prefixes.Add($"http://*:{_port}/api/external_updates/");
            _listener.Prefixes.Add($"http://*:{_port}/api/automations/");
            _listener.Prefixes.Add($"http://*:{_port}/api/expressions/");
            _listener.Prefixes.Add($"http://*:{_port}/api/test/");
            _listener.Prefixes.Add($"http://*:{_port}/api/force_restart/");

            Start();
        }

        public void Start()
        {
            _listener.Start();
            _ = Receive();
        }

        public void Stop()
        {
            _listener.Stop();
            Log.Information($"Server stopped.");
        }

        private async Task Receive()
        {
            while (_listener.IsListening)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    HandleRequest(context);
                }
                catch (ObjectDisposedException)
                {
                    Log.Information("[Receive] Listener disposed, exiting loop.");
                    break;
                }
                catch (HttpListenerException ex) when (!_listener.IsListening)
                {
                    Log.Error($"[Receive] [HttpListenerException] HTTP listener error: {ex}");
                    break;
                }
                catch (Exception ex)
                {
                    Log.Error($"[Receive] [Generic Exception] HTTP listener error: {ex}");
                }
            }
        }
        
        private void HandleRequest(HttpListenerContext context)
        {
            string path = context.Request.Url.AbsolutePath;
            
            Log.Information($"[HandleRequest] start handling a request at: {path}");

            if (context.Request.HttpMethod == "POST")
            {
                if (path.Equals(this.apiExternalUpdates, StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleExternalUpdates(context);
                }
                else if(path.Equals(this.apiAutomations, StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleAutomations(context);
                }
                else if(path.Equals(this.apiExpressions, StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleExpressions(context);
                }
                else if(path.Equals(this.apiForceRestart, StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleForceRestart(context);
                }
                else if(path.Equals(this.apiTest, StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleTest(context);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.Close();
                }
            }
            else if (context.Request.HttpMethod == "GET")
            {
                if (path.Equals(this.apiTest, StringComparison.OrdinalIgnoreCase))
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
            
            Log.Information($"[HandleRequest] end handling a request at: {path}");
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
            Log.Information($"[HandleExternalUpdates] start");
            
            using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                string requestBody = reader.ReadToEnd();
                Log.Information($"[HandleExternalUpdates] Received POST data on {this.apiExternalUpdates}: {requestBody}");

                try
                {
                    ActionDTO data = JsonConvert.DeserializeObject<ActionDTO>(requestBody);
                    ActionUpdate?.Invoke(this, data);
                    Log.Information($"[HandleExternalUpdates] end");
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.Close();
                }
                catch (Exception ex)
                {
                    Log.Information($"[HandleExternalUpdates] Error processing POST data: {ex.Message}");
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
                    List<AutomationDTO> parsedObjects = JsonConvert.DeserializeObject<List<AutomationDTO>>(requestBody);
                    RegisteredAutomations?.Invoke(this, parsedObjects);

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
        
        private void HandleExpressions(HttpListenerContext context)
        {
            using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                string requestBody = reader.ReadToEnd();
                Log.Information($"Received POST data on {this.apiExpressions}: {requestBody}");
                try
                {
                    JObject jsonObject = JObject.Parse(requestBody);
                    List<Expression> expressions = ExpressionUtils.ParseExpressions(jsonObject);
                    RegisteredExpressions?.Invoke(this, expressions);

                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.Close();
                }
                catch (Exception ex)
                {
                    Log.Error($"Error processing POST data: {ex.Message}");
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

                /*// force object subscription in hass
                foreach (var gameObject in Object.FindObjectsByType<ECATracker>(UnityEngine.FindObjectsSortMode.None)) //TODO optimize
                {
                    gameObject.SubscribeObject();
                }*/
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