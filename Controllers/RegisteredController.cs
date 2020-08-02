using AppWebServer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppWebServer.Controllers
{
    public class RegisteredController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        // GET: Registered
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "userId,loginName,password")] User user,string phone, string submitButton)
        {
            if (submitButton.Contains("查詢"))
                return queryphone(phone);
            else if (submitButton.Contains("申請"))
            {
                return editUser(user);
            }
            else
                return View();
        }
        private ActionResult queryphone(string phone)
        {
            User user = db.User.Where(i => i.phone.Contains(phone)).FirstOrDefault();
            if (user == null)
                ViewBag.message = "查無此號碼:" + phone;
            else if (user.isEanble != null&&user.isEanble.Value)
            {
                ViewBag.message = "此手機已註冊過帳號:" + phone;
                user = null;
            }
            TempData["user"] = user;
            return View(user);
        }
        public ActionResult editUser(User user)
        {
            if (ModelState.IsValid)
            {
                User edituser = TempData["user"] as User;
                if (user.password==null||user.loginName==null|| user.password.Length==0||user.loginName.Length==0)
                {
                    Response.Write("<script>alert('帳號及密碼不得為空');</script>");
                    TempData["user"] = edituser;
                    return View(edituser);
                }
                User existUser = db.User.Where(i => i.loginName.Contains(user.loginName)).FirstOrDefault();
                if (existUser != null)
                {
                    Response.Write("<script>alert('帳號已經存在，請重新設定帳號');</script>");
                    TempData["user"] = edituser;
                    return View(edituser);
                }
                string[] password = Request["password"].Split(',');
                if (password[0].CompareTo(password[1]) == 0)
                {
                    User thisuser = db.User.Find(edituser.userId);
                    thisuser.loginName = user.loginName;
                    thisuser.password = Utility.HMACSHA256(password.First(), thisuser.loginName.ToString());
                    db.Entry(thisuser).State = EntityState.Modified;
                    db.SaveChanges();
                    Response.Write("<script>alert('申請成功');</script>");
                }
                else
                {
                    TempData["user"] = edituser;
                    Response.Write("<script>alert('申請失敗，密碼再次確認不一致');</script>");
                    return View(edituser);
                }
                return RedirectToAction("Index","Home",null);
            }
            return View(user);
        }
    }
}