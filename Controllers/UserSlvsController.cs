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
    public class UserSlvsController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();

        // GET: UserSlvs
        public ActionResult Index()
        {
            return View(db.UserSlv.ToList());
        }

        // GET: UserSlvs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSlv userSlv = db.UserSlv.Find(id);
            if (userSlv == null)
            {
                return HttpNotFound();
            }
            return View(userSlv);
        }

        // GET: UserSlvs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserSlvs/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "eId,userId,name,currNum,accNum,dateStart,dateEnd")] UserSlv userSlv)
        {
            if (ModelState.IsValid)
            {
                db.UserSlv.Add(userSlv);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(userSlv);
        }

        // GET: UserSlvs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSlv userSlv = db.UserSlv.Find(id);
            if (userSlv == null)
            {
                return HttpNotFound();
            }
            return View(userSlv);
        }

        // POST: UserSlvs/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "eId,userId,name,currNum,accNum,dateStart,dateEnd")] UserSlv userSlv)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userSlv).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userSlv);
        }

        // GET: UserSlvs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSlv userSlv = db.UserSlv.Find(id);
            if (userSlv == null)
            {
                return HttpNotFound();
            }
            return View(userSlv);
        }

        // POST: UserSlvs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserSlv userSlv = db.UserSlv.Find(id);
            db.UserSlv.Remove(userSlv);
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
