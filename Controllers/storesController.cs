using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppWebServer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppWebServer.Controllers
{
    [Authorize(Roles = "admin,manage,store")]
    public class storesController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        // GET: stores
        public ActionResult Index()
        {
            if (User.IsInRole("store"))
            {
                int userId = int.Parse(User.Identity.Name);
                var store = db.store.Where(i => i.ownerUser == userId);
                return View(store.OrderByDescending(o => o.storeId));//.Include(s => s.StoreType).Include(s => s.User);
            }
            else
                return View(db.store.Include(s => s.StoreType).Include(s => s.User).OrderByDescending(o=>o.storeId));
        }
        public string FormatPotos(string potos)
        {

            //"<td>{0}</td><td><input name='name{1}' type='text' placeholder='url' class='form-control input-md'/> </td>"
            if (potos == null)
                potos = "";
            string[] values = potos.Split(',');
            List<string[]> value = new List<string[]>();
            foreach(string a in values)
            {
                value.Add(new string[]{a});
            }
            List<string> title = new List<string>();
            title.Add("圖片URL");
            TempData["potoNames"] = title;
            return DynamictableHelper.GetDynamictable(value, title, "poto");
        }
        public string FormatProudcts(string products)
        {
            try
            {
                List<string> title;
                List<string[]> value = new List<string[]>();
                if (products != null)
                {
                    JObject jObject = JsonConvert.DeserializeObject("{ \"product\":" + products + "}") as JObject;
                    Dictionary<string, string> dictObj = jObject["product"].First.ToObject<Dictionary<string, string>>();
                    title = dictObj.Keys.ToList();
                    string[] valuestring;
                    foreach (var item in jObject["product"])
                    {
                        valuestring = new string[title.Count];
                        for (int i = 0; i < title.Count; i++)
                        {
                            valuestring[i] = item[title[i]].ToString();
                        }
                        value.Add(valuestring);
                    }
                }
                else
                {
                    title = new List<string>();
                    title.Add("pName");
                    title.Add("pText");
                    title.Add("pImage");
                    value.Add(new string[title.Count]);
                }
                TempData["productNames"] = title;
                return DynamictableHelper.GetDynamictable(value, title, "products");
            }
            catch
            {
            }
            return "";
        }
        // GET: stores/Details/5
        /*
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            store store = db.store.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }
        */
        // GET: stores/Create
        public ActionResult Create()
        {
            ViewBag.stId = new SelectList(db.StoreType, "stId", "stName");
            ViewBag.ownerUser = new SelectList(db.User, "userId", "userName");
            return View();
        }
        // POST: stores/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "storeId,storeName,html,sotrePhone,sotreAddr,sotreWeb,sotreWeb2,potos,products,isEnable,stId,ownerUser")] store store)
        {
            if (ModelState.IsValid)
            {
                if (User.IsInRole("store"))
                {
                    store.ownerUser =int.Parse( User.Identity.Name);
                }
                db.store.Add(store);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.stId = new SelectList(db.StoreType, "stId", "stName", store.stId);
            ViewBag.ownerUser = new SelectList(db.User, "userId", "userName", store.ownerUser);
            return View(store);
        }

        // GET: stores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            store store = db.store.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            ViewBag.stId = new SelectList(db.StoreType, "stId", "stName", store.stId);
            ViewBag.ownerUser = new SelectList(db.User, "userId", "userName", store.ownerUser);
            return View(store);
        }

        // POST: stores/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "storeId,storeName,html,sotrePhone,sotreAddr,sotreWeb,sotreWeb2,potos,products,isEnable,stId,ownerUser")] store store)
        {
            if (ModelState.IsValid)
            {

                store.potos = formatpotoString(store);
                store.products = formatproductString(store);
                if (User.IsInRole("store"))
                {
                    store.ownerUser = int.Parse(User.Identity.Name);
                }
                db.Entry(store).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.stId = new SelectList(db.StoreType, "stId", "stName", store.stId);
            ViewBag.ownerUser = new SelectList(db.User, "userId", "userName", store.ownerUser);
            return View(store);
        }

        // GET: stores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            store store = db.store.Find(id);
            if (store == null)
            {
                return HttpNotFound();
            }
            return View(store);
        }
        // POST: stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            store store = db.store.Find(id);
            db.store.Remove(store);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private string formatpotoString(store store)
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
        private string formatproductString(store store)
        {
            string datas = "";
            string pName = "", pText = "", pImage = "";
            int i = 0;
            List<string> potosReqname = (TempData["productNames"] as List<string>);
            while (Request[potosReqname.FirstOrDefault() + i] != null)
            {
                pName = Request[potosReqname[0] + i];
                pText = Request[potosReqname[1] + i];
                pImage = Request[potosReqname[2] + i];

                if (pName.Length + pText.Length + pImage.Length != 0)
                {
                    datas += string.Format(",{{\"pName\":\"{0}\",\"pText\": \"{1}\",\"pImage\":\"{2}\"}}", pName, pText, pImage);
                }
                i++;
            }
            return datas.Length > 1 ? "[" + datas.Substring(1) + "]" : "";
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
