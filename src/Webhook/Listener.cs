#if !NoBotSupport

using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace DiscordConnector.Webhook
{

    /// <summary>
    /// Helper class definition which defines the structure of (valid) outgoing JSON
    /// </summary>
    internal class Response
    {
        public string status { get; set; }
        public string data { get; set; }
    }
    internal class Listener
    {
        private static int _port;
        private static string _host;
        private static string _prefix;
        private static HttpListener _httpListener;
        private static Thread _listenerThread;
        private static string _expectAuthHeader;

        public Listener()
        {
            if (!Plugin.StaticConfig.DiscordBotEnabled)
            {
                return;
            }

            _expectAuthHeader = Plugin.StaticConfig.DiscordBotAuthorization;
            _port = Plugin.StaticConfig.DiscordBotPort;

            // Right now, host just matches all IPs on server.
            _host = "+";

            _prefix = $"http://{_host}:{_port}/discord/";


            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(_prefix);

            Plugin.StaticLogger.LogDebug($"Created HTTP Listener at {_prefix}");

            _listenerThread = new Thread(new ThreadStart(ChildListener));
            _listenerThread.Start();
        }

        private static void ChildListener()
        {
            _httpListener.Start();
            Plugin.StaticLogger.LogDebug($"Listening for incoming requests from Discord on {_httpListener.Prefixes.Count} endpoints.");

            _httpListener.BeginGetContext(ListenerCallback, _httpListener);
        }

        public static void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            // Call EndGetContext to complete the asynchronous operation.
            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;

            string method = request.HttpMethod;
            string contentType = request.ContentType;
            string authHeader = request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader))
            {
                authHeader = "";
            }

            if (method.Equals("POST"))
            {
                string body = GetRequestPostData(request);

                Plugin.StaticLogger.LogDebug($"Webhook Request: {method} {contentType}\nAuthorization: {authHeader}\n{body}");
                StringCommand command;
                try
                {
                    command = JsonConvert.DeserializeObject<StringCommand>(body);
                    Plugin.StaticLogger.LogDebug($"Parsed a command of '{command.command}' with data '{command.data}'");

                    if (authHeader.Equals(_expectAuthHeader))
                    {
                        Plugin.StaticLogger.LogDebug("Authorization accepted.");
                        SendResponse(context.Response, new Response
                        {
                            status = "accepted",
                            data = ""
                        });
                    }
                    else
                    {
                        Plugin.StaticLogger.LogDebug("Authorization rejected (doesn't match).");
                        SendResponse(context.Response, new Response
                        {
                            status = "unauthorized",
                            data = ""
                        });
                    }
                }
                catch (JsonException e)
                {
                    Plugin.StaticLogger.LogError(e);
                    SendResponse(context.Response, new Response
                    {
                        status = "error",
                        data = $"Unable to parse request. {e.Message}"
                    });
                }
            }
            listener.BeginGetContext(ListenerCallback, listener);
        }

        /// <summary>
        /// Send a JSON <paramref name="response"/> to the provided <paramref name="httpResponse"/> object.
        /// Automatically converts the <paramref name="response"/> into a JSON string and sends it in a body.
        /// </summary>
        /// <param name="httpResponse">The response from the HttpListenerContext.</param>
        /// <param name="response">The response to provide to the client.</param>
        private static void SendResponse(HttpListenerResponse httpResponse, Response response)
        {
            string responseString = JsonConvert.SerializeObject(response);

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            httpResponse.ContentLength64 = buffer.Length;
            System.IO.Stream output = httpResponse.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }

        /// <summary>
        /// Builds the body data of a <paramref name="request"/> into a string.
        /// </summary>
        /// <param name="request">The HttpListenerContext.Request object.</param>
        /// <returns>Body of the request as a string.</returns>
        private static string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }
            string data = "";
            using (System.IO.Stream body = request.InputStream) // here we have data
            {
                using (var reader = new System.IO.StreamReader(body, request.ContentEncoding))
                {
                    int bufsize = 512;
                    char[] strbuf = new char[bufsize];
                    while (!reader.EndOfStream)
                    {
                        int readlen = reader.ReadBlock(strbuf, 0, bufsize);
                        data += new string(strbuf, 0, readlen);
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// Shutdown the child thread and close the HttpListener
        /// </summary>
        public void Dispose()
        {
            Plugin.StaticLogger.LogDebug("Closing Discord Bot listener");
            _listenerThread.Abort();
            _httpListener.Close();
        }
    }
}
#endif
