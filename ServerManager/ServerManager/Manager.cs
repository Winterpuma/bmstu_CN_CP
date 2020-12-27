﻿using System;
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
            bool success = false;
            foreach (Server server in availableServers.Values)
            {
                if (server.State == ServerState.Free) // нашли свободный сервер
                {
                    success = true;
                    CurRequestIndex++;
                    requestToServer[CurRequestIndex] = server; // текущий реквест связываем с сервером
                    server.SendGetRequest(path);
                    break;
                }
            }

            if (!success)
                return -1;

            return CurRequestIndex;
        }

        public string GetRequestState(int requestId) // а что если такого нет???
        {
            return requestToServer[requestId].UpdateServerState();
        }
        
        public void UpdateAllServers()
        {
            foreach (Server server in availableServers.Values)
                server.UpdateServerState();
        }
    }
}
