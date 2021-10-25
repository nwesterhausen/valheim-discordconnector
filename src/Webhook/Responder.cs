
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
        public static void SendResponse(HttpListenerResponse httpResponse, Response response)
        {
            // set the status code 
            httpResponse.StatusCode = response.statusCode;
            // serialize the response for adding to the body
            string responseString = JsonConvert.SerializeObject(response);

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            httpResponse.ContentLength64 = buffer.Length;
            System.IO.Stream output = httpResponse.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }
    }
}
