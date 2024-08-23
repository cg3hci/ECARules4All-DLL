using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using UnityEngine;


namespace ECARules4All_DLL.SmartHomeHubClients
{
    public class ReceivedUpdate
    {
        public string subject { get; set; }
        public string verb { get; set; }
        public Dictionary<string, object> parameters { get; set; }

        public ReceivedUpdate(string subject, string verb, Dictionary<string, object> parameters = null)
        {
            this.subject = subject;
            this.verb = verb;
            this.parameters = parameters;
        }
    }
    
    public class APIServer
    {
        private string _url;
        private int _port;
        private HttpListener _listener;
        public event EventHandler<ReceivedUpdate> Update; 

        public APIServer(string url = "http://localhost",  int port = 8080)
        {
            _url = url;
            _port = port;
            _listener = new HttpListener();
            this._listener.Prefixes.Add($"{this._url}:{this._port}/api/external_updates/");
            this.Start();
        }

        public void Start()
        {
            _listener.Start();
            Debug.Log($"Server started");
            Receive();
        }

        public void Stop()
        {
            _listener.Stop();
            Debug.Log($"Server stopped.");
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
            if (context.Request.HttpMethod == "POST")
            {
                using (var reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    string requestBody = reader.ReadToEnd();
                    Debug.Log($"Received POST data: {requestBody}");
                    
                    try
                    {
                        // read data and throw the event
                        ReceivedUpdate data = JsonSerializer.Deserialize<ReceivedUpdate>(requestBody);
                        Update?.Invoke( this, data);

                        // response
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.Close();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"Error processing POST data: {ex.Message}");
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        context.Response.Close();
                    }
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                context.Response.Close();
            }
        }
    }
}