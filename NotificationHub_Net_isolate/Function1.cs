using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace NotificationHub_Net_isolate
{
    public  class Function1
    {
       
      
        private static HttpClient httpClient = new HttpClient();
       

        [Function("index")]
        public static HttpResponseData Index([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestData req, FunctionContext context)
        {
             var root = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Split("\\bin")[0];
           // var root = Environment.CurrentDirectory;
            var path = Path.Combine(root, "content", "index.html");
     

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/html; charset=utf-8");

            response.WriteString(File.ReadAllText(path));


            return response;

        }


        [Function("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [SignalRConnectionInfoInput(HubName = "NotificationServerless")] SignalRConnectionInfo connectionInfo)
        {
        
            return connectionInfo;
        }

        [Function("broadcast")]
        [SignalROutput(HubName = "NotificationServerless")]
        public static async Task Broadcast([HttpTrigger(AuthorizationLevel.Function)] object item,
            [SignalRConnectionInfoInput(HubName = "NotificationServerless")] MyMessage connectionInfo,
            FunctionContext context)
        {
            var logger = context.GetLogger("SignalRFunction");

        
           

            var message = $"Output message created at {DateTime.Now}";

           connectionInfo= new MyMessage()
            {
                Target = "newMessage",
                Arguments = new[] { message }
            };
            
        }

     

    }

    public class GitResult
    {
        [JsonRequired]
        [JsonProperty("stargazers_count")]
        public string StarCount { get; set; }
    }
    public class SignalRConnectionInfo
    {
        public string Url { get; set; }

        public string AccessToken { get; set; }
    }
    public class MyMessage
    {
        public string Target { get; set; }

        public object[] Arguments { get; set; }
    }
}