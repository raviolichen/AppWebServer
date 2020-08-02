using AppWebServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppWebServer.Controllers
{
    [Authorize(Roles = "admin,manage,store")]
    public class HomeController : Controller
    {
        // GET: Home
        private AppDataBaseEntities db = new AppDataBaseEntities();
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "admin,manage")]
        public ActionResult ApproveGolds()
        {
            return PartialView("ApproveGolds",db.Gold.Where(i=>i.isEnable==null).OrderByDescending(o => o.goldId));
        }
        [Authorize(Roles = "admin,manage")]
        public ActionResult ApproveStores()
        {
            return PartialView("ApproveStores", db.store.Where(i=>i.isEnable==null).OrderByDescending(o => o.storeId));
        }
        [Authorize(Roles = "admin,manage")]
        public ActionResult ApproveUsers()
        {
            return PartialView("ApproveUsers", db.User.Where(i => (i.isEanble == null|| i.powerGroup == null )&& (i.loginName!=null&&i.password!=null)).OrderByDescending(o => o.userId));
        }
        [Authorize(Roles = "admin,manage,store")]
        public ActionResult regVote()
        {
            int userId = int.Parse(User.Identity.Name);
            List<Vote> votes = db.Vote.Where(i => i.dateStart <= DateTime.Now ).ToList().Where(i=> i.dateEnd.GetValueOrDefault().AddDays(1) >= DateTime.Now && i.voteType.ToLower().CompareTo("store") == 0).OrderByDescending(o => o.eId).ToList();
            return PartialView(votes);
        }
    }
}