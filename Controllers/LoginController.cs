using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HKAdBus.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, string rememberMe, string returnUrl)
        {
            if (String.IsNullOrEmpty(rememberMe))
                rememberMe = "false";

            List<string> correctCredentials = new List<string>()
            { 
                "admin", "aa111111" // login credential
            };
            if (ValidateLogin(username, password))
            {
                if (username.ToLower() == correctCredentials[0]
                    && password == correctCredentials[1])
                {
                    FormsAuthentication.RedirectFromLoginPage(username, Boolean.Parse(rememberMe));
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Login", new { showInvalidText = "true" });
            }
            return RedirectToAction("Index", "Login", new { showInvalidText = "true" });
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }

        private bool ValidateLogin(string username, string password)
        {
            if (String.IsNullOrEmpty(username))
                ModelState.AddModelError("username", "登入名稱不能為空");
            if (String.IsNullOrEmpty(password))
                ModelState.AddModelError("password", "登入密碼不能為空");
            return ModelState.IsValid;
        }
	}
}