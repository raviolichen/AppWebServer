using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppWebServer.Models;

namespace AppWebServer.Controllers.eventpage
{
    [Authorize(Roles = "admin,manage,store")]
    public class VoteItemsController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();

        // GET: VoteItems
        public ActionResult Index(int? Id)
        {
            ViewData["Id"]=Id;
            List<VoteItem> voteItems;
            Vote vote = db.Vote.Find(Id);
            if (User.IsInRole("store"))
            {
                int userId = int.Parse(User.Identity.Name);
                voteItems = db.VoteItem.SqlQuery("SELECT VoteItem.* FROM VoteItem inner join store On itemId=storeId WHERE vId="+Id+" AND ownerUser=" + userId).ToList();
            }
            else
                voteItems = vote.VoteItem.OrderByDescending(o => o.viId).ToList();
            return View(voteItems);
        }
        public string GetItemNameAndPotos(VoteItem voteItem)
        {
            string tbody = "<td><img src='{0}' alt='{1}' style='width:150px'/></td><td>{1}</td>";

            if (voteItem.Vote.voteType.Contains("store"))
            {
                store item = db.store.Find(voteItem.itemId);
                if(item==null)
                {
                    VoteItem _voteItem=db.VoteItem.Find(voteItem.viId);
                    db.VoteItem.Remove(_voteItem);
                    db.SaveChanges();
                    return string.Format(tbody, "資料不存在。", "資料不存在，將由系統自動刪除。"); ;
                }
                return string.Format(tbody, (item.potos!=null?item.potos.Split(',')[0]:""), item.storeName);
            }
            else if (voteItem.Vote.voteType.Contains("Slvs"))
            {
                Slvs item = db.Slvs.Find(voteItem.itemId);
                return string.Format(tbody, (item.potos != null ? item.potos.Split(',')[0] : ""), item.sname);
            }
            else
            {

            }
            return "";

        }
        public string GetItemNameAndPotosOnCols(VoteItem voteItem)
        {
            string tbody = "<div class=\"form-group\"><label class=\"control-label col-md-2\">圖片</label><div class=\"col-md-10\"><img src='{0}' alt='{1}' style='width:150px'/></div></div><div class=\"form-group\"><label class=\"control-label col-md-2\">名稱</label><div class=\"col-md-10\"><p>{1}</p></div></div>";
            if (voteItem.Vote.voteType.Contains("store"))
            {
                store item = db.store.Find(voteItem.itemId);
                return string.Format(tbody, (item.potos != null ? item.potos.Split(',')[0] : ""), item.storeName);
            }
            else if (voteItem.Vote.voteType.Contains("Slvs"))
            {
                Slvs item = db.Slvs.Find(voteItem.itemId);
                return string.Format(tbody, (item.potos != null ? item.potos.Split(',')[0] : ""), item.sname);
            }
            else
            {

            }
            return "";

        }



        // POST: VoteItems/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        public SelectList GetItemNameAndPotosDropList(int? Id)
        {
            Vote vote = db.Vote.Find(Id);
            List<VoteItem> voteItems = vote.VoteItem.ToList();
            string vitems = "";
            foreach(VoteItem v in voteItems)
            {
                vitems += v.itemId + ",";
            }

            if (vote.voteType.Contains("store"))
            {
                if (User.IsInRole("store"))
                {
                    int userId = int.Parse(User.Identity.Name);
                    return new SelectList(db.store.Where(i => i.isEnable == true && i.ownerUser==userId && !vitems.Contains(i.storeId+",")), "storeId", "storeName");
                }
                else
                {            
                    return new SelectList(db.store.Where(i => i.isEnable == true && !vitems.Contains(i.storeId + ",")), "storeId", "storeName");
                }
            }
            else if (vote.voteType.Contains("Slvs"))
            {
                return new SelectList(db.Slvs.Where(i => i.isEnable == true&& !vitems.Contains(i.slvId + ",")), "slvId", "sname");
            }
            else
            {

            }
            return null;

        }
        public ActionResult Create(int? vId)
        {
            Vote vote = db.Vote.Find(vId.Value);
            if (vote != null)
            {
               ViewBag.itemId = GetItemNameAndPotosDropList(vote.vId);
            }
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "viId,itemId,vcount,isEnable,ittype,vId")] VoteItem voteItem)
        {
            if (ModelState.IsValid)
            {
                voteItem.vcount = 0;
                db.VoteItem.Add(voteItem);
                db.SaveChanges();
                return RedirectToAction("Index","voteitems",new { Id=voteItem.vId});
            }

            ViewBag.vId = new SelectList(db.Vote, "vId", "vlog", voteItem.vId);
            return View(voteItem);
        }

        // GET: VoteItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VoteItem voteItem = db.VoteItem.Find(id);
            if (voteItem == null)
            {
                return HttpNotFound();
            }
            return View(voteItem);
        }

        // POST: VoteItems/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "viId,vcount,isEnable")] VoteItem voteItem)
        {
            if (ModelState.IsValid)
            {
                VoteItem editvoteItem = db.VoteItem.Find(voteItem.viId);
                editvoteItem.isEnable = voteItem.isEnable;
                editvoteItem.vcount = voteItem.vcount;
                db.Entry(editvoteItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "voteitems", new { Id = editvoteItem.vId });
            }
            ViewBag.vId = new SelectList(db.Vote, "vId", "vlog", voteItem.vId);
            return View(voteItem);
        }

        // POST: VoteItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            VoteItem voteItem = db.VoteItem.Find(id);
            int _vId = voteItem.vId.Value;
            db.VoteItem.Remove(voteItem);
            db.SaveChanges();
            return RedirectToAction("Index", "voteitems", new { Id = _vId });
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
