using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using Newtonsoft.Json.Linq;

namespace ServerManager
{
    public class Server
    {
        public string Name { get; private set; }
        public Uri Uri { get; set; }

        // state
        // is up?
        public bool IsBusy { get; set; } = true;
        //DateTime busyStart;

        public int CurTaskNumber { get; private set; } = 0;
        public int TotalTasksNumber { get; private set; } = 0;
        public int PersentageDone { get; private set; } = 100;

        // curRequest
        // response

        public Server(Uri uri)
        {
            Uri = uri;
        }

        public string SendGetRequest(string path)
        {
            return GetMethod(path).Result;
        }

        public string UpdateServerState()
        {
            string resJson = GetMethod("/state").Result;

            JObject root = JObject.Parse(resJson);

            Name = root.Value<string>("name");
            CurTaskNumber = root.Value<int>("current");
            TotalTasksNumber = root.Value<int>("total");
            PersentageDone =  (TotalTasksNumber == 0) ? 0 : 
                Convert.ToInt32(100 * ((double)CurTaskNumber / (double)TotalTasksNumber));

            return resJson;
        }

        public async Task<string> GetMethod(string path)
        {
            HttpClient client = new HttpClient();

            client.Timeout = TimeSpan.FromMinutes(1);
            client.BaseAddress = Uri;
            var response = await client.GetAsync(path);

            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}
