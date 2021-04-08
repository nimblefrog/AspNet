using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using prjProductSys.Models;

namespace prjProductSys.Controllers
{
    public class CategoryController : Controller
    {
        dbProductEntities db = new dbProductEntities();
        // GET: Category
        //加上Authorize後，必須登入後才能使用這個動作方法
        [Authorize]
        public ActionResult Index()
        {
            string uid = User.Identity.Name;
            var permission = db.會員.FirstOrDefault(x => x.帳號 == uid).權限;
            ViewBag.Permission = permission;

            List<產品類別> category = new List<產品類別>();
            foreach (var item in db.產品類別.OrderByDescending(m => m.修改日))
            {
                //將日期格式變更
                category.Add(new 產品類別()
                {
                    類別編號 = item.類別編號,
                    類別名稱 = item.類別名稱,
                    編輯者 = item.編輯者,
                    修改日 = Sys.StringConverDateTimeString(item.修改日),
                    建立日 = Sys.StringConverDateTimeString(item.建立日)
                });
            }

            return View(category);
        }

        [Authorize]
        public ActionResult Create()
        {
            string uid = User.Identity.Name;
            var permission = db.會員.FirstOrDefault(x => x.帳號 == uid).權限;
            if(!permission.Contains("C"))
            {
                RedirectToAction("Index", "PermissionErrorMsg", new { msg = "您的身份無新增的權限" });
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(string 類別名稱)
        {
            string editDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var category = new 產品類別();
            category.類別名稱 = 類別名稱;
            category.編輯者 = User.Identity.Name;
            category.建立日 = editDate;
            category.修改日 = editDate;

            db.產品類別.Add(category);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Delete(int cid)
        {
            var uid = User.Identity.Name;
            var permission = db.會員.FirstOrDefault(x => x.帳號 == uid).權限;
            if(!permission.Contains("D"))
            {
                return RedirectToAction("Index", "PermissionErrorMsg", new { msg = "您的身份無編輯的權限" });
            }

            var products = db.產品資料.Where(x => x.類別編號 == cid).ToList();
            var category = db.產品類別.FirstOrDefault(x => x.類別編號 == cid);

            db.產品資料.RemoveRange(products); //關聯的產品也要刪除
            db.產品類別.Remove(category);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Edit(int cid)
        {
            var uid = User.Identity.Name;
            var permission = db.會員.FirstOrDefault(x => x.帳號 == uid).權限;
            if (!permission.Contains("U"))
            {
                return RedirectToAction("Index", "PermissionErrorMsg", new { msg = "您的身份無編輯的權限" });
            }

            var category = db.產品類別.FirstOrDefault(x => x.類別編號 == cid);

            return View(category);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(int 類別編號, string 類別名稱)
        {
            var editDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var category = db.產品類別.FirstOrDefault(x => x.類別編號 == 類別編號);
            category.類別名稱 = 類別名稱;
            category.編輯者 = User.Identity.Name;
            category.修改日 = editDate;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}