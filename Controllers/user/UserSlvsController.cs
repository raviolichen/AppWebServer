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
    public class UserSlvsController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();

        // GET: UserSlvs
        public ActionResult Index(int? id)
        {
            if(id==null) return HttpNotFound();
            User user= db.User.Find(id);
            ViewBag.userName = user.userName;
            return View(user.UserSlv.OrderByDescending(o => o.eId).ToList());
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
                return RedirectToAction("Index",new {Id=userSlv.userId });
            }

            return View(userSlv);
        }

        // GET: UserSlvs/Edit/5
        public ActionResult Edit(int? userId,int? eId)
        {
            if (eId == null || userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSlv userSlv = db.UserSlv.Where(i=>i.userId== userId && i.eId== eId).FirstOrDefault();
            if (userSlv == null)
            {
                return HttpNotFound();
            }
            TempData["name"] = userSlv.name;
            return View(userSlv);
        }

        // POST: UserSlvs/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "eId,name,userId,currNum,accNum,dateStart,dateEnd")] UserSlv userSlv)
        {
            if (ModelState.IsValid)
            {
                userSlv.name = TempData["name"].ToString();
                db.Entry(userSlv).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index",new { Id=userSlv.userId});
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
            return RedirectToAction("Index",new {Id=userSlv.userId });
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
