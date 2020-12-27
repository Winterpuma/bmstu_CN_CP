using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ServerManager
{
    public class Server
    {
        private HttpClient _client;
        public string Name { get; private set; }
        public Uri Uri { get; set; }
        

        public ServerState State { get; set; }
        public string Request { get; private set; }
        public string Response { get; private set; }
        //DateTime busyStart;

        public int CurTaskNumber { get; private set; } = 0;
        public Int64 TotalTasksNumber { get; private set; } = 0;
        public int PersentageDone { get; private set; } = 100;


        public Server(Uri uri)
        {
            Uri = uri;

            _client = new HttpClient();
            _client.BaseAddress = Uri;
            //_client.Timeout = TimeSpan.FromMinutes(1);
        }

        public void SendGetRequest(string path)
        {
            GetMethod(path);
        }

        public string UpdateServerState(bool force = false)
        {
            if (!force && (State == ServerState.NoState || State == ServerState.Down))
                return null;

            JObject root;
            string resJson = GetMethod("/state", false).Result;

            try
            {
                root = JObject.Parse(resJson);
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                if (resJson.Contains("404 Not Found"))
                    State = ServerState.NoState; // нет пути для получения статуса

                return null;
            }

            Name = root.Value<string>("name");
            CurTaskNumber = root.Value<int>("current");
            TotalTasksNumber = root.Value<Int64>("total");
            PersentageDone =  (TotalTasksNumber == 0) ? 0 : 
                Convert.ToInt32(100 * ((double)CurTaskNumber / (double)TotalTasksNumber));

            return resJson;
        }

        public async Task<string> GetMethod(string path, bool saveRes = true)
        {
            if (saveRes)
            {
                Request = path;
                Response = null;
                State = ServerState.Busy;
            }

            var response = await _client.GetAsync(path);

            var responseString = await response.Content.ReadAsStringAsync();

            if (saveRes)
            {
                Response = responseString;
                State = ServerState.Free;
            }

            return responseString;
        }
    }

    public enum ServerState
    {
        Free,
        Busy,
        Down, // TODO
        NoState
    }
}
