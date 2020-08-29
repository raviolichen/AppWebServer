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
    public class SlvsController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();

        // GET: Slvs
        public ActionResult Index(int? eId)
        {
            if (eId != null)
            {
                var slvs = db.Slvs.Where(i => i.eId == eId);
                ViewBag.parentName = slvs.FirstOrDefault().EventPage.name;
                return View(slvs.OrderByDescending(o => o.slvId).ToList());
            }
            else { return HttpNotFound();
            }
        }
        public string GetSlvsRem(int? slvId, int? totalmaxUse)
        {
            int sumtotal = db.UserCoinReord.Where(i => i.coinId == slvId && i.cointype == "slvs").Count();
            return sumtotal==0?"0":(totalmaxUse - sumtotal).ToString();
        }

        // GET: Slvs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slvs slvs = db.Slvs.Find(id);
            if (slvs == null)
            {
                return HttpNotFound();
            }
            return View(slvs);
        }

        // GET: Slvs/Create
        public ActionResult Create(int? eId)
        {
            //ViewBag.eId = new SelectList(db.EventPage, "eid", "url");
            if (eId != null)
            {
                ViewBag.parentName = db.EventPage.Find(eId).name;
                return View();
            }
            else
            {
                return HttpNotFound();
            }
        }

        // POST: Slvs/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "slvId,eId,sname,html,potos,isEnable,slvNum,maxUse,totalmaxUse,isGuid")] Slvs slvs)
        {
            if (ModelState.IsValid && Request["eId"] != null)
            {
                slvs.eId =int.Parse(Request["eId"]);
                slvs.potos = formatpotoString();
                slvs.LastEditDateTime = DateTime.Now;
                db.Slvs.Add(slvs);
                db.SaveChanges();
                return RedirectToAction("Index", new { eId = slvs.eId });
            }

            ViewBag.eId = slvs.eId;
            return View(slvs);
        }

        // GET: Slvs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slvs slvs = db.Slvs.Find(id);
            if (slvs == null)
            {
                return HttpNotFound();
            }
            ViewBag.eId = slvs.eId;
            ViewBag.parentName = slvs.EventPage.name;
            return View(slvs);
        }

        // POST: Slvs/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "slvId,eId,sname,html,potos,isEnable,slvNum,maxUse,totalmaxUse,isGuid")] Slvs slvs)
        {
            if (ModelState.IsValid)
            {
                slvs.potos = formatpotoString();
                slvs.LastEditDateTime = DateTime.Now;
                db.Entry(slvs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index",new { eId=slvs.eId});
            }
            //ViewBag.eId = new SelectList(db.EventPage, "eid", "url", slvs.eId);
            return View(slvs);
        }

        // GET: Slvs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slvs slvs = db.Slvs.Find(id);
            if (slvs == null)
            {
                return HttpNotFound();
            }
            return View(slvs);
        }

        // POST: Slvs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Slvs slvs = db.Slvs.Find(id);
            db.Slvs.Remove(slvs);
            db.SaveChanges();
            return RedirectToAction("Index", new { eId = slvs.eId });
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
