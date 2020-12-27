using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerManager;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    public class ManagerController : Controller
    {
        private static Manager manager = new Manager();

        [HttpGet("[action]")]
        public IEnumerable<Server> GetServers()
        {
            manager.UpdateAllServers();
            return manager.Servers;
        }

        [HttpPost("[action]")]
        public ActionResult AddServer([FromBody]RequestUri uri)
        {
            int res = -1;
            try
            {
                res = manager.AddServer(new Uri(uri.uri));
            }
            catch (UriFormatException)
            {
                return BadRequest();
            }

            if (res == 0)
                return Ok();
            else
                return Conflict(res);
        }

        [HttpPost("[action]")]
        public ActionResult DeleteServer([FromBody]RequestUri uri)
        {
            int res = -1;
            try
            {
                res = manager.DeleteServer(new Uri(uri.uri));
            }
            catch (UriFormatException)
            {
                return BadRequest();
            }

            if (res == 0)
                return Ok();
            else
                return Conflict(res);
        }
    }


    public class RequestUri
    {
        public string uri { get; set; }
    }
}