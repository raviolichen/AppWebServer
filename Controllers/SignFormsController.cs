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
    public class SignFormsController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();

        // GET: SignForms
        public ActionResult Index(int? eid)
        {
            if (eid != null) {
                var eventPage = db.EventPage.Find(eid);
                TempData["eventPage"] = eventPage;
                return View(eventPage.SignForm.ToList());
            }
            return null;
        }

        // GET: SignForms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SignForm signForm = db.SignForm.Find(id);
            if (signForm == null)
            {
                return HttpNotFound();
            }
            return View(signForm);
        }

        // GET: SignForms/Create
        public ActionResult Create()
        {
            //ViewBag.eId = new SelectList(db.EventPage, "eid", "url");
            return View();
        }

        // POST: SignForms/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sId,eId,userLimit,dateStart,dateEnd,field,isEanble")] SignForm signForm)
        {
            EventPage eventPage = ((EventPage)TempData["eventPage"]);
            if (ModelState.IsValid)
            {
                signForm.field = formatpotoString();
                signForm.eId = eventPage.eid;
                db.SignForm.Add(signForm);
                db.SaveChanges();
                return RedirectToAction("Index", new { eId = signForm.eId });
            }
            TempData["eventPage"] = eventPage;
            return View(signForm);
        }

        // GET: SignForms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SignForm signForm = db.SignForm.Find(id);
            if (signForm == null)
            {
                return HttpNotFound();
            }
            //ViewBag.eId = new SelectList(db.EventPage, "eid", "url", signForm.eId);
            return View(signForm);
        }

        // POST: SignForms/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "sId,eId,userLimit,dateStart,dateEnd,field,isEanble")] SignForm signForm)
        {
            if (ModelState.IsValid)
            {
                signForm.field = formatpotoString();
                db.Entry(signForm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { eId = signForm.eId });
            }
            //ViewBag.eId = new SelectList(db.EventPage, "eid", "url", signForm.eId);
            return View(signForm);
        }

        // GET: SignForms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SignForm signForm = db.SignForm.Find(id);
            if (signForm == null)
            {
                return HttpNotFound();
            }
            return View(signForm);
        }

        // POST: SignForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SignForm signForm = db.SignForm.Find(id);
            db.SignForm.Remove(signForm);
            db.SaveChanges();
            return RedirectToAction("Index", new { eId = signForm.eId });
        }

       public string FormatFeild(string field)
        {
            if (field == null)
                field = "";
            string[] values = field.Split(',');
            List<string[]> value = new List<string[]>();
            List<string> title = new List<string>();
            foreach (string a in values)
            {
                value.Add(new string[] { a });
            }
            title.Add("欄位名稱");
            TempData["feild"] = title;
            return DynamictableHelper.GetDynamictable(value, title, "feild");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private string formatpotoString()
        {
            string datas = "";
            string potosReqname = (TempData["feild"] as List<string>).FirstOrDefault();
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
