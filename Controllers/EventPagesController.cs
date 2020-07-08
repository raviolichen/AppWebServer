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
    public class EventPagesController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();

        // GET: EventPages
        public ActionResult Index()
        {
            return View(db.EventPage.ToList());
        }

        // GET: EventPages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventPage eventPage = db.EventPage.Find(id);
            if (eventPage == null)
            {
                return HttpNotFound();
            }
            return View(eventPage);
        }

        // GET: EventPages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventPages/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "eid,url,name,html,dateStart,dateEnd,slvLimit,toGoldNum,postDate,evenType")] EventPage eventPage)
        {
            if (ModelState.IsValid)
            {
                db.EventPage.Add(eventPage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(eventPage);
        }

        // GET: EventPages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventPage eventPage = db.EventPage.Find(id);
            if (eventPage == null)
            {
                return HttpNotFound();
            }
            return View(eventPage);
        }

        // POST: EventPages/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "eid,url,name,html,dateStart,dateEnd,slvLimit,toGoldNum,postDate,evenType")] EventPage eventPage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventPage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eventPage);
        }

        // GET: EventPages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventPage eventPage = db.EventPage.Find(id);
            if (eventPage == null)
            {
                return HttpNotFound();
            }
            return View(eventPage);
        }

        // POST: EventPages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventPage eventPage = db.EventPage.Find(id);
            db.EventPage.Remove(eventPage);
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
