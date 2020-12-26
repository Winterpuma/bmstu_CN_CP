using System;

namespace ServerManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager m = new Manager();
            m.AddServer(new Uri("http://localhost:8080/"));
            m.UpdateAllServers();
            //int iReq = m.SendGetRequest("/fact/5");
            //var res = m.GetRequestState(iReq);

            Console.ReadLine();
        }
    }
}
