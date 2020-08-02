using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AppWebServer.Models;

namespace AppWebServer.Controllers
{
    [Authorize(Roles = "admin,manage")]
    public class VotesController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        private List<SelectListItem> getTypeList(string val = "")
        {
            List<SelectListItem> selectLists = new List<SelectListItem>();
            selectLists.Add(new SelectListItem { Text = "商店票選", Value = "store" });
            selectLists.Add(new SelectListItem { Text = "銀幣票選", Value = "Slvs" });
            if (val.Length > 0)
                selectLists.Where(i => i.Value.Contains(val)).FirstOrDefault().Selected = true;
            return selectLists;
        }
        // GET: Votes
        public string GetTypeName(string voteType)
        {
            if (voteType.Contains("store"))
            {
                return "商店票選";
            }
            else if (voteType.Contains("Slvs"))
            {
                return "銀幣票選";
            }
            else
                return "";
        }
        public ActionResult Index(int? eid)
        {

            TempData["eid"] = eid;
            if (eid == null)
                return HttpNotFound();
            ViewBag.voteType = getTypeList();
            return View(db.EventPage.Find(eid).Vote.OrderByDescending(o => o.vId));
        }
        public ActionResult VoteUsers(int? Id)
        {
            if (Id == null) return HttpNotFound();
            string ids = db.Vote.Find(Id).vlog;
            List<User> users = db.User.Where(i => ids.Contains(i.userId + ",")).ToList();
            return View(users);
        }

        // GET: Votes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vote vote = db.Vote.Find(id);
            if (vote == null)
            {
                return HttpNotFound();
            }
            return View(vote);
        }

        // GET: Votes/Create
        public ActionResult Create(int? eid)
        {
            ViewBag.voteType = getTypeList();
            TempData["eid"] = eid;
            return View();
        }

        // POST: Votes/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "vId,dateStart,dateEnd,RdateStart,RdateEnd,isEnable,voteType,voteCount")] Vote vote)
        {
            int _eid = int.Parse(TempData["eid"].ToString());
            if (ModelState.IsValid)
            {
                vote.eId = _eid;
                db.Vote.Add(vote);
                db.SaveChanges();
                return RedirectToAction("Index",new {eid=_eid });
            }
            TempData["eid"] = _eid;
            ViewBag.voteType = getTypeList();
            return View(vote);
        }

        // GET: Votes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vote vote = db.Vote.Find(id);
            if (vote == null)
            {
                return HttpNotFound();
            }
            ViewBag.voteType = getTypeList(vote.voteType);
            return View(vote);
        }

        // POST: Votes/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "vId,dateStart,dateEnd,RdateStart,RdateEnd,isEnable,voteCount")] Vote vote)
        {
            int _eid = int.Parse(TempData["eid"].ToString());
            if (ModelState.IsValid)
            {
                Vote orginvote = db.Vote.Find(vote.vId);
                orginvote.voteCount = vote.voteCount;
                orginvote.dateStart = vote.dateStart;
                orginvote.dateEnd = vote.dateEnd;
                orginvote.RdateStart = vote.RdateStart;
                orginvote.RdateEnd = vote.RdateEnd;
                orginvote.isEnable = vote.isEnable;
                db.Entry(orginvote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { eid = _eid });
            }
            return View(vote);
        }

        // GET: Votes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vote vote = db.Vote.Find(id);
            if (vote == null)
            {
                return HttpNotFound();
            }
            return View(vote);
        }

        // POST: Votes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vote vote = db.Vote.Find(id);
            int _eId = vote.eId.Value;
            db.Vote.Remove(vote);
            db.SaveChanges();
            return RedirectToAction("Index", new { eid = _eId });
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
