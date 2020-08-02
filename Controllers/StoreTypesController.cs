using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppWebServer.Models;

namespace AppWebServer.Controllers
{
    [Authorize(Roles = "admin,manage")]
    public class StoreTypesController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();

        // GET: StoreTypes
        public ActionResult Index()
        {
            return View(db.StoreType.OrderByDescending(o => o.stId).ToList());
        }

        // GET: StoreTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StoreType storeType = db.StoreType.Find(id);
            if (storeType == null)
            {
                return HttpNotFound();
            }
            return View(storeType);
        }

        // GET: StoreTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StoreTypes/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "stId,stName,stpotos")] StoreType storeType)
        {
            if (ModelState.IsValid)
            {
                storeType.stpotos = formatpotoString();
                db.StoreType.Add(storeType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(storeType);
        }

        // GET: StoreTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StoreType storeType = db.StoreType.Find(id);
            if (storeType == null)
            {
                return HttpNotFound();
            }
            return View(storeType);
        }

        // POST: StoreTypes/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "stId,stName,stpotos")] StoreType storeType)
        {
            if (ModelState.IsValid)
            {
                storeType.stpotos = formatpotoString();
                db.Entry(storeType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(storeType);
        }

        // GET: StoreTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StoreType storeType = db.StoreType.Find(id);
            if (storeType == null)
            {
                return HttpNotFound();
            }
            return View(storeType);
        }

        // POST: StoreTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StoreType storeType = db.StoreType.Find(id);
            db.StoreType.Remove(storeType);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public string FormatPotos(string potos)
        {

            //"<td>{0}</td><td><input name='name{1}' type='text' placeholder='url' class='form-control input-md'/> </td>"
            if (potos == null)
                potos = "";
            string[] values = potos.Split(',');
            List<string[]> value = new List<string[]>();
            foreach (string a in values)
            {
                value.Add(new string[] { a });
            }
            List<string> title = new List<string>();
            title.Add("圖片URL");
            TempData["potoNames"] = title;
            return DynamictableHelper.GetDynamictable(value, title, "poto");
        }
        private string formatpotoString()
        {
            string datas = "";
            string potosReqname = (TempData["potoNames"] as List<string>).FirstOrDefault();
            int i = 0;
            while (Request[potosReqname + i] != null)
            {
                if (Request[potosReqname + i].ToString().Length > 0)
                    datas += "," + Request[potosReqname + i];
                i++;
            }
            return datas.Length >= 1 ? datas.Substring(1) : "";

        }
    }
}
