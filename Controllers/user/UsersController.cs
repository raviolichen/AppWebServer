
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AppWebServer.Models;

namespace AppWebServer.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        // GET: Users
        public ActionResult Index()
        {
            return View(db.User.OrderByDescending(o=>o.userId).ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.powerGroup = new SelectList(db.UserGroup, "gId", "gname");
            return View();
        }

        // POST: Users/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userId,userName,phone,oAuth,powerGroup,isEanble,password,loginName")] User user)
        {
            if (ModelState.IsValid)
            {
                user.password = Utility.HMACSHA256(user.password, user.loginName.ToString());
                db.User.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.powerGroup = new SelectList(db.UserGroup, "gId", "gname");
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.powerGroup = new SelectList(db.UserGroup, "gId", "gname",user.powerGroup);
            return View(user);
        }

        // POST: Users/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userId,userName,phone,oAuth,powerGroup,isEanble,password,loginName,prohibit")] User user)
        {
            if (ModelState.IsValid)
            {
                User originUser = db.User.Find(user.userId);
                user.password = Utility.HMACSHA256(user.password, user.loginName.ToString());
                user.deviceId = originUser.deviceId;
                user.token = originUser.token;
                user.tokendate = originUser.tokendate;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.powerGroup = new SelectList(db.UserGroup, "gId", "gname");
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.User.Find(id);
            db.User.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public string getGroupName(int? id)
        {
            if (id == null)
                return "";
          UserGroup ug=db.UserGroup.Find(id);
          return   ug==null?"":ug.gname;
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
