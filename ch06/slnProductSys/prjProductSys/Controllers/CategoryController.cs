using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace prjProductSys.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        //加上Authorize後，必須登入後才能使用這個動作方法
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}