
using System.Net;
using Newtonsoft.Json;

namespace DiscordConnector.Webhook
{
    internal static class Responder
    {


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

        public static void SendResponse401(HttpListenerResponse httpResponse, Response response)
        {
            httpResponse.StatusCode = 401;
            SendResponse(httpResponse, response);
        }
        public static void SendResponse400(HttpListenerResponse httpResponse, Response response)
        {
            httpResponse.StatusCode = 400;
            SendResponse(httpResponse, response);
        }
        public static void SendResponse415(HttpListenerResponse httpResponse, Response response)
        {
            httpResponse.StatusCode = 415;
            SendResponse(httpResponse, response);
        }
        public static void SendResponse200(HttpListenerResponse httpResponse, Response response)
        {
            httpResponse.StatusCode = 200;
            SendResponse(httpResponse, response);
        }
    }
}
