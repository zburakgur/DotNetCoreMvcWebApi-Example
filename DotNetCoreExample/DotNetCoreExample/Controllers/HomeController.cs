using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreExample.Models;
using System.Text;
using DotNetCoreExample.Utils;

namespace DotNetCoreExample.Controllers
{

    public class HomeController : Controller
    {
        public IActionResult MainPage()
        {
            string userName =  Functions.getSession(HttpContext.Session, "userName");
            string password = Functions.getSession(HttpContext.Session, "password");
            
            if(userName == null || password == null)            
                return Redirect("/");

            LoginClass login = new LoginClass();
            login.uN = userName;
            login.p = password;

            ViewData.Model = login;

            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
