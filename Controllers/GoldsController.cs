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
    [Authorize(Roles = "admin,manage,store")]
    public class GoldsController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();

        // GET: Golds
        public ActionResult Index()
        {
            if (User.IsInRole("store"))
            {
                int userId = int.Parse(User.Identity.Name);
                var gold = db.Gold.Where(i => i.ownerUserId == userId);
                return View(gold.OrderByDescending(o => o.goldId).ToList());//.Include(s => s.StoreType).Include(s => s.User);
            }
            else
                return View(db.Gold.OrderByDescending(o => o.goldId).ToList());
        }
        public string GetGoldRem(int? goldId, int? totalmaxUse)
        {
            int sumtotal = db.UserCoinReord.Where(i => i.coinId == goldId && i.cointype == "gold").Count();
            return sumtotal == 0 ? "0" : (totalmaxUse - sumtotal).ToString();
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
            ViewBag.ownerUserId = new SelectList(db.User, "userId", "userName");
            return View();
        }

        // POST: Golds/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "goldId,goldname,html,potos,goldnum,qrcode,isEnable,ownerUserId,userUsetimes,totalUsetimes")] Gold gold)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("store"))
                {
                    gold.ownerUserId = int.Parse(User.Identity.Name);
                }
                gold.potos = formatpotoString();
                gold.lastEditDateTime = DateTime.Now;
                db.Gold.Add(gold);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ownerUserId = new SelectList(db.User, "userId", "userName", gold.ownerUserId);
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
            ViewBag.ownerUserId = new SelectList(db.User, "userId", "userName", gold.ownerUserId);
            return View(gold);
        }
        // POST: Golds/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "goldId,goldname,html,potos,goldnum,isEnable,ownerUserId,userUsetimes,totalUsetimes")] Gold gold)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("store"))
                {
                    gold.ownerUserId = int.Parse(User.Identity.Name);
                }
                gold.potos = formatpotoString();
                gold.lastEditDateTime = DateTime.Now;
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
