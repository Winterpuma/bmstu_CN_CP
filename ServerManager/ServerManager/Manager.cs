using System;
using System.Collections.Generic;
using System.Text;

namespace ServerManager
{
    public class Manager
    {
        List<Server> availableServers;
        public int CurRequestIndex { get; private set; }
        Dictionary<int, Server> requestToServer;

        public IEnumerable<Server> Servers
        {
            get => availableServers;
        }

        public Manager()
        {
            availableServers = new List<Server>();
            requestToServer = new Dictionary<int, Server>();
        }

        public void AddServer(Uri uri)
        {
            availableServers.Add(new Server(uri));
            CurRequestIndex++; // REMOVEEEEEEEEE
        }

        public int SendGetRequest(string path) // возможно переделать на asynk
        {
            foreach (Server server in availableServers)
            {
                if (!server.IsBusy) // нашли свободный сервер
                {
                    CurRequestIndex++;
                    requestToServer[CurRequestIndex] = server; // текущий реквест связываем с сервером
                    server.SendGetRequest(path); // TODO проверка что успешно отправили?
                }
            }

            return CurRequestIndex;
        }

        public string GetRequestState(int requestId) // а что если такого нет???
        {
            string path = "/state";

            return requestToServer[requestId].SendGetRequest(path);
        }
        
        public void UpdateAllServers()
        {
            foreach (Server server in availableServers)
                server.UpdateServerState();
        }
    }
}
