using Masar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace Masar.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            SetSession("id", Guid.NewGuid().ToString());
            if (User.Identity.IsAuthenticated)
            {
                SetCookies("userName", User.Identity.Name);
                SetSession("username", User.Identity.Name);
            }
            else
            {
                SetCookies("userName", "guest");
                SetSession("username", "guest");
            }
            SetCookies("browserName", Request.Headers["User-Agent"].ToString());
            return View();
        }

        public IActionResult SetCookies(string cookieName, string cookieValue)
        {
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(15),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append(cookieName, cookieValue, options);
            return Ok("Cookies has been set.");
        }

        public IActionResult SetSession(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string fname, string lname, string email, string subject, string feedback)
        {
            ViewBag.message = $"Thank you {fname} {lname}! We received your message about: {subject}";
            return View();
        }

        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

