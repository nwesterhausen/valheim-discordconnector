#if !NoBotSupport

using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordConnector.Webhook
{
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

            if (!method.Equals("POST"))
            {
                return;
            }
            string body = GetRequestPostData(request);

            Plugin.StaticLogger.LogDebug($"Webhook Request: {method} {contentType}\nAuthorization: {authHeader}\n{body}");


            try
            {
                JObject parsedResponse = JObject.Parse(body);
                bool isAuthorized = authHeader.Equals(_expectAuthHeader);
                if (!isAuthorized)
                {
                    Responder.SendResponse401(context.Response, new Response
                    {
                        status = "unauthorized"
                    });
                    return;
                }
                string command = (string)parsedResponse["command"];

                switch (command)
                {
                    case "status":
                        Responder.SendResponse200(context.Response, new Response
                        {
                            status = "accepted"
                        });

                        break;
                    case "chat":
                        SpeakerCommand fullCommand = parsedResponse.ToObject<SpeakerCommand>();
                        Responder.SendResponse501(context.Response, new MessageResponse
                        {
                            status = "accepted",
                            message = $"Not fully implemented. {fullCommand.data.username} shouts: {fullCommand.data.content}"
                        });
                        break;
                    case "leaderboard":
                    case "reload":
                    case "save":
                    case "shutdown":
                        Responder.SendResponse501(context.Response, new MessageResponse
                        {
                            status = "error",
                            message = $"{command} not yet implemented"
                        });
                        break;
                    default:
                        Plugin.StaticLogger.LogDebug($"Unknown command: ${command}");
                        Responder.SendResponse400(context.Response, new MessageResponse
                        {
                            status = "error",
                            message = $"unknown command {command}"
                        });
                        break;
                }
            }
            catch (JsonSerializationException e)
            {
                Plugin.StaticLogger.LogError(e);
                Responder.SendResponse400(context.Response, new MessageResponse
                {
                    status = "error",
                    message = "invalid JSON body"
                });

            }
            listener.BeginGetContext(ListenerCallback, listener);
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
