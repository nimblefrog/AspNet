using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using prjProductSys.Models;

namespace prjProductSys.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string account, string pwd)
        {
            var db = new dbProductEntities();
            var member = db.會員.FirstOrDefault(x => x.帳號 == account && x.密碼 == pwd);
            if(member != null)
            {
                FormsAuthentication.RedirectFromLoginPage(member.帳號, true);
                return RedirectToAction("Index", "Category");
            }

            ViewBag.IsLogin = true;
            return View();
        }
    }
}