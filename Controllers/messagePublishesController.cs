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
    public class messagePublishesController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        private List<SelectListItem> GetUserGroup(string usergroup)
        {
            List<SelectListItem> items;
            if (usergroup.Length > 0)
                items = new SelectList(db.UserGroup,  "gId", "gname", usergroup).ToList();
            else
                items = new SelectList(db.UserGroup, "gId","gname").ToList();

            items.Insert(0, (new SelectListItem { Text = "全部", Value = "All" }));
            if (!"all,user,admin,store,manage".Contains(usergroup.ToLower()))
                items.Add(new SelectListItem { Text = "指定", Value = "spec", Selected = true });
            else
                items.Add(new SelectListItem { Text = "指定", Value = "spec" });
            return items;
        }
        // GET: messagePublishes
        public ActionResult Index(int? Id)
        {
            ViewBag.Id = Id;
            return View(db.messagePublish.OrderByDescending(o => o.pId).ToList());
        }

        // GET: messagePublishes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            messagePublish messagePublish = db.messagePublish.Find(id);
            if (messagePublish == null)
            {
                return HttpNotFound();
            }
            return PartialView(messagePublish);
        }

        // GET: messagePublishes/Create
        public ActionResult Create()
        {
            ViewBag.p_users = GetUserGroup("");
            return PartialView();
        }
        // POST: messagePublishes/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pId,p_users,pDateStart,pDateEnd,pmessage")] messagePublish messagePublish)
        {
            if (ModelState.IsValid)
            {
                if (messagePublish.p_users.ToLower().Contains("spec"))
                    messagePublish.p_users = Request["p_users"].Replace("spec,", string.Empty);
                db.messagePublish.Add(messagePublish);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            TempData["messagePublish"] = messagePublish;
            var message = string.Join(" | ", ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage));
            Response.Write("<script>alert('輸入的資料錯誤'" + message + ");</script>");
            ViewBag.p_users = GetUserGroup(messagePublish.p_users);
            return RedirectToAction("Index");
        }

        // GET: messagePublishes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            messagePublish messagePublish = db.messagePublish.Find(id);
            if (messagePublish == null)
            {
                return HttpNotFound();
            }
            ViewBag.p_users = GetUserGroup(messagePublish.p_users);
            return PartialView(messagePublish);
        }

        // POST: messagePublishes/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pId,p_users,pDateStart,pDateEnd,pmessage")] messagePublish messagePublish)
        {

            if (ModelState.IsValid)
            {
                if (messagePublish.p_users.ToLower().Contains("spec"))
                    messagePublish.p_users = Request["p_users"].Replace("spec,", string.Empty);
                db.Entry(messagePublish).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var message = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            Response.Write("<script>alert('輸入的資料錯誤'" + message + ");</script>");
            TempData["messagePublish"] = messagePublish;
            ViewBag.p_users = GetUserGroup(messagePublish.p_users);
            return RedirectToAction("Index");
        }

        // GET: messagePublishes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            messagePublish messagePublish = db.messagePublish.Find(id);
            if (messagePublish == null)
            {
                return HttpNotFound();
            }
            return PartialView(messagePublish);
        }

        // POST: messagePublishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            messagePublish messagePublish = db.messagePublish.Find(id);
            db.messagePublish.Remove(messagePublish);
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
