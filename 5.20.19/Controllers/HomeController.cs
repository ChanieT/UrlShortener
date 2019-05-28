using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using _5._20._19.Models;
using Data;
using shortid;
using Microsoft.Extensions.Configuration;

namespace _5._20._19.Controllers
{
    public class HomeController : Controller
    {
        private string _conn;
        public HomeController(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("ConStr");
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var repo = new UrlRepository(_conn);
                var user = repo.GetUserByEmail(User.Identity.Name);
                return View(user.Id);
            }
            else
            {
                return View(0);
            }
        }

        [HttpPost]
        public string AddUrl(string completeUrl, int userId)
        {
            var repo = new UrlRepository(_conn);
            string urlHash = repo.AddUrl(completeUrl, userId);
            //return Json(new { urlHash = completeUrl });
            return urlHash;
        }


        [Route("/hash/{urlHash}")]
        public IActionResult Reroute(string urlHash)
        {
            var repo = new UrlRepository(_conn);
            string url = repo.GetCompleteUrl(urlHash);
            return Redirect($"{url}");
        }

        public IActionResult MyUrls()
        {
            if (User.Identity.IsAuthenticated)
            {
                var repo = new UrlRepository(_conn);
                var urls = repo.GetUrlsForUser(User.Identity.Name);
                return View(urls);
            }
            else
            {
                return Redirect("/account/login");
            }
        }

    }
}
