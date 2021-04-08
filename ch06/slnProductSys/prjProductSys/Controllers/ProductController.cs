using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using prjProductSys.Models;

namespace prjProductSys.Controllers
{
    public class ProductController : Controller
    {
        dbProductEntities db = new dbProductEntities();
        // GET: Product
        [Authorize]
        public ActionResult Index(int cid=1)
        {
            var uid = User.Identity.Name;
            var permission = db.會員.FirstOrDefault(x => x.帳號 == uid).權限;
            ViewBag.Permission = permission;

            var vm = new ProductCategoryViewModel();
            vm.Category = db.產品類別.OrderByDescending(m => m.修改日).ToList();

            var tempProduct = db.產品資料.Where(x => x.類別編號 == cid).OrderByDescending(x => x.修改日).ToList();
            foreach(var p in tempProduct)
            {
                p.修改日 = Sys.StringConverDateTimeString(p.修改日);
                p.建立日 = Sys.StringConverDateTimeString(p.建立日);
            }
            vm.Product = tempProduct;

            return View(vm);
        }

        [Authorize]
        public ActionResult Create()
        {
            var uid = User.Identity.Name;
            var permission = db.會員.FirstOrDefault(x => x.帳號 == uid).權限;
            if(!permission.Contains("C"))
            {
                return RedirectToAction("Index", "PermissionErrorMsg", new { msg = "您的身份無新增的權限" });
            }

            ViewBag.Category = db.產品類別.ToList();
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(string 產品編號, string 品名, int 單價, HttpPostedFileBase fImg, int 類別編號)
        {
            var tempProduct = db.產品資料.FirstOrDefault(x => x.產品編號 == 產品編號);
            if(tempProduct != null)
            {
                ViewBag.IsProduct = true;
                ViewBag.Category = db.產品類別.ToList();
                return View();
            }

            string fileName = "question.png";
            if(fImg != null)
            {
                if(fImg.ContentLength > 0)
                {
                    fileName = Guid.NewGuid().ToString() + ".jpg";
                    var path = string.Format("{0}/{1}", Server.MapPath("~/Images"), fileName);
                    fImg.SaveAs(path);
                }
            }

            string editDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var product = new 產品資料();
            product.產品編號 = 產品編號;
            product.品名 = 品名;
            product.單價 = 單價;
            product.圖示 = fileName;
            product.類別編號 = 類別編號;
            product.編輯者 = User.Identity.Name;
            product.建立日 = editDate;
            product.修改日 = editDate;
            db.產品資料.Add(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Delete(string pid)
        {
            var uid = User.Identity.Name;
            var permission = db.會員.FirstOrDefault(x => x.帳號 == uid).權限;
            if (!permission.Contains("D"))
            {
                return RedirectToAction("Index", "PermissionErrorMsg", new { msg = "您的身份無編輯的權限" });
            }

            var product = db.產品資料.FirstOrDefault(x => x.產品編號 == pid);
            var filename = product.圖示;
            if(filename != "question.png")
            {
                System.IO.File.Delete(string.Format("{0}/{1}", Server.MapPath("~/Images"), filename));
            }

            db.產品資料.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Edit(string pid)
        {
            var uid = User.Identity.Name;
            var permission = db.會員.FirstOrDefault(x => x.帳號 == uid).權限;
            if(!permission.Contains("U"))
            {
                return RedirectToAction("Index", "PermissionErrorMsg", new { msg = "您的身份無編輯的權限" });
            }

            var product = db.產品資料.FirstOrDefault(x => x.產品編號 == pid);
            ViewBag.Category = db.產品類別.ToList();
            return View(product);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(string 產品編號, string 品名, int 單價, HttpPostedFileBase fImg, string 圖示, int 類別編號)
        {
            string fileName = "";
            if(fImg != null)
            {
                if(fImg.ContentLength > 0)
                {
                    fileName = Guid.NewGuid().ToString() + ".jpg";
                    var path = string.Format("{0}/{1}", Server.MapPath("~/Images"), fileName);
                    fImg.SaveAs(path);
                }
            }
            else
            {
                fileName = 圖示;  //若無上傳圖，則使用hidden隱藏欄位的舊檔名
            }

            string editdate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var product = db.產品資料.Where(m => m.產品編號 == 產品編號)
                .FirstOrDefault();
            product.品名 = 品名;
            product.單價 = 單價;
            product.圖示 = fileName;
            product.類別編號 = 類別編號;
            product.編輯者 = User.Identity.Name;
            product.修改日 = editdate;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}