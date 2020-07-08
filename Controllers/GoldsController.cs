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
    public class GoldsController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();

        // GET: Golds
        public ActionResult Index()
        {
            return View(db.Gold.ToList());
        }

        // GET: Golds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gold gold = db.Gold.Find(id);
            if (gold == null)
            {
                return HttpNotFound();
            }
            return View(gold);
        }

        // GET: Golds/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Golds/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "goldId,goldname,html,potos,goldnum,qrcode,isEnable,ownerUserId")] Gold gold)
        {
            if (ModelState.IsValid)
            {
                db.Gold.Add(gold);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gold);
        }

        // GET: Golds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gold gold = db.Gold.Find(id);
            if (gold == null)
            {
                return HttpNotFound();
            }
            return View(gold);
        }

        // POST: Golds/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "goldId,goldname,html,potos,goldnum,qrcode,isEnable,ownerUserId")] Gold gold)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gold).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gold);
        }

        // GET: Golds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gold gold = db.Gold.Find(id);
            if (gold == null)
            {
                return HttpNotFound();
            }
            return View(gold);
        }

        // POST: Golds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Gold gold = db.Gold.Find(id);
            db.Gold.Remove(gold);
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
