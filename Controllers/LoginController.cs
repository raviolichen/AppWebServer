using AppWebServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AppWebServer.Controllers
{
    public class LoginController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string acc, string Password)
        {
            acc = acc.Replace("<", string.Empty).Replace(">", string.Empty).Replace("'", string.Empty).Replace("%", string.Empty);
            Password = Password.Replace("<", string.Empty).Replace(">", string.Empty).Replace("'", string.Empty).Replace("%", string.Empty);
            // 登入的密碼（以 SHA1 加密）
            // FormsAuthentication.HashPasswordForStoringInConfigFile(Password, "SHA1");

            //這一條是去資料庫抓取輸入的帳號密碼的方法請自行實做
            //var r = account.GetSingleAccount(acc, Password);
            string E_password = Utility.HMACSHA256(Password, acc);
            User user= db.User.Where(i => i.password == E_password).FirstOrDefault();
            if (user == null)
            {
                ViewBag.message = "您輸入的帳號不存在或者密碼錯誤!";
                ViewBag.acc = acc;
                return View();
            }
            UserGroup userGroup=null;
            if (user.powerGroup != null&&user.isEanble!=null&& user.isEanble.Value)
            {
                userGroup = db.UserGroup.Find(user.powerGroup);
            }
            // 登入時清空所有 Session 資料
            if (userGroup == null||user.isEanble==null|| !user.isEanble.Value)
            {
                ViewBag.message = "您輸入的帳號，目前不可用!";
                ViewBag.acc = acc;
                return View();
            }

            Session.RemoveAll();

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
              user.userId.ToString(),//你想要存放在 User.Identy.Name 的值，通常是使用者帳號
              DateTime.Now,
              DateTime.Now.AddMinutes(30),
              false,//將管理者登入的 Cookie 設定成 Session Cookie
              userGroup.gname,//userdata看你想存放啥
              FormsAuthentication.FormsCookiePath);
            string encTicket = FormsAuthentication.Encrypt(ticket);

            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            return RedirectToAction("Index", "Home");

        }
    }
}