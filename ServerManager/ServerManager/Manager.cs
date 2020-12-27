using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace ServerManager
{
    public class Manager
    {
        ConcurrentDictionary<Uri, Server> availableServers;
        public int CurRequestIndex { get; private set; }
        Dictionary<int, Server> requestToServer;

        public IEnumerable<Server> Servers
        {
            get => availableServers.Values;
        }

        public Manager()
        {
            availableServers = new ConcurrentDictionary<Uri, Server>();
            requestToServer = new Dictionary<int, Server>();
        }

        public int AddServer(Uri uri)
        {
            bool added = availableServers.TryAdd(uri, new Server(uri));
            if (!added)
                return -1;
            return 0;
        }

        public int DeleteServer(Uri uri)
        {
            bool removed = availableServers.TryRemove(uri, out Server value);
            if (!removed)
                return -1;
            return 0;
        }

        public int SendGetRequest(string path) // возможно переделать на asynk
        {
            foreach (Server server in availableServers.Values)
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
            foreach (Server server in availableServers.Values)
                server.UpdateServerState();
        }
    }
}
