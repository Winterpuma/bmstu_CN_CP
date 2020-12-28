using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace ServerManager
{
    public class Manager
    {
        ConcurrentDictionary<Uri, Server> serverPool;
        public int CurRequestIndex { get; private set; }
        Dictionary<int, Server> requestToServer;

        public IEnumerable<Server> Servers
        {
            get => serverPool.Values;
        }

        public Manager()
        {
            serverPool = new ConcurrentDictionary<Uri, Server>();
            requestToServer = new Dictionary<int, Server>();
        }

        /// <summary>
        /// Добавляет сервер в отслеживаемые
        /// </summary>
        /// <param name="uri">Адрес сервера</param>
        /// <returns>Успешность добавления</returns>
        public int AddServer(Uri uri)
        {
            bool added = serverPool.TryAdd(uri, new Server(uri));
            if (!added)
                return -1;
            return 0;
        }

        /// <summary>
        /// Удаляет сервер из отслеживаемых
        /// </summary>
        /// <param name="uri">Адрес сервера</param>
        /// <returns>Успешность удаления</returns>
        public int DeleteServer(Uri uri)
        {
            bool removed = serverPool.TryRemove(uri, out Server value);
            if (!removed)
                return -1;
            return 0;
        }

        /// <summary>
        /// Отправляет Get запрос первому свободному серверу
        /// </summary>
        /// <param name="path">Адрес запроса</param>
        /// <returns>Индекс запроса для отслеживания</returns>
        public int SendGetRequest(string path) // возможно переделать на asynk
        {
            bool success = false;
            foreach (Server server in serverPool.Values)
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

        /// <summary>
        /// Отправляет Get запрос конкретному серверу
        /// </summary>
        /// <param name="uri">Адрес сервера</param>
        /// <param name="path">Адрес запроса</param>
        /// <returns>Индекс запроса для отслеживания</returns>
        public int SendGetRequest(Uri uri, string path)
        {
            if (serverPool.TryGetValue(uri, out Server server))
            {
                CurRequestIndex++;
                requestToServer[CurRequestIndex] = server; // текущий реквест связываем с сервером
                server.SendGetRequest(path);
            }
            else
                return -1; // такого сервера нет

            return CurRequestIndex;
        }

        /// <summary>
        /// По индексу получить состояние сервера
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public string GetRequestState(int requestId)
        {
            return requestToServer[requestId].UpdateServerState();
        }
        
        /// <summary>
        /// Обновить все сервера
        /// </summary>
        public void UpdateAllServers()
        {
            foreach (Server server in serverPool.Values)
                server.UpdateServerState();
        }
    }
}
