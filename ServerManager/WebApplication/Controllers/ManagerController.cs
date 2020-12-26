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
        public ActionResult AddServer([FromBody]RequestUri uri) // return success state?
        {
            try
            {
                manager.AddServer(new Uri(uri.uri));
            }
            catch (UriFormatException)
            {
                return BadRequest();
            }
            return Ok();
        }
        

        /*
        // GET: Manager
        public ActionResult Index()
        {
            return View();
        }

        // GET: Manager/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Manager/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Manager/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Manager/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Manager/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Manager/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Manager/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/
    }

    public class RequestUri
    {
        public string uri { get; set; }
    }
}