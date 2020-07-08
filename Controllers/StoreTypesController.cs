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
    public class StoreTypesController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();

        // GET: StoreTypes
        public ActionResult Index()
        {
            return View(db.StoreType.ToList());
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
    }
}
