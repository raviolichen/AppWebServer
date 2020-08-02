
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AppWebServer.Models;

namespace AppWebServer.Controllers
{
    [Authorize(Roles = "admin,manage,store,user")]
    public class UserEditController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        // GET: Users
        public ActionResult Edit()
        {
            int id = int.Parse(User.Identity.Name.ToString());
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userId,password")] User user)
        {
            if (ModelState.IsValid)
            {
                int id = int.Parse(User.Identity.Name.ToString());
                User edituser = db.User.Find(id);
                string[] password = Request["password"].Split(',');
                if (password[0].CompareTo(password[1]) == 0)
                {
                    edituser.password = Utility.HMACSHA256(password.First(), edituser.loginName.ToString());
                    db.Entry(edituser).State = EntityState.Modified;
                    db.SaveChanges();
                    Response.Write("<script>alert('修改成功');</script>");
                }
                else
                {
                    Response.Write("<script>alert('修改失敗，密碼再次確認不一致');</script>");
                }
                return RedirectToAction("Edit");
            }
            return View(user);
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
