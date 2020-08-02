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
    [Authorize(Roles = "admin,manage")]
    public class SignRecordsController : Controller
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        // GET: SignRecords
        public ActionResult Index(int? Id, string submitButton)
        {
            ViewBag.id = Id;
            var signRecords = db.SignForm.Find(Id).SignRecords;
            if (Request["search_isComplete"] != null)
            {
                if (!Request["search_isComplete"].Contains("all")){
                    bool isComplete =bool.Parse(Request["search_isComplete"]);
                    if(isComplete)
                        signRecords = signRecords.Where((i) => i.isComplete == isComplete).ToList();
                    else
                        signRecords = signRecords.Where((i) => i.isComplete == isComplete||i.isComplete==null).ToList();
                    ViewBag.search_isComplete = Request["search_isComplete"];
                }
                if(Request["search_textBox_keyword"]!=null && Request["search_textBox_keyword"].Length > 0)
                {
                    signRecords = signRecords.Where((i) => i.signData.Contains(Request["search_textBox_keyword"])).ToList();
                    ViewBag.search_textBox_keyword = Request["search_textBox_keyword"];
                }
            }
            if (submitButton!=null&& submitButton.Contains("停權"))
            {
                SeTprohibit_datetime(signRecords);
            }
            return View(signRecords.OrderByDescending(o => o.sId).ToList());
        }
        [HttpPost]
        public void SeTprohibit_datetime(ICollection<SignRecords> signRecords)
        {
            if (Request["prohibit"] == null|| Request["prohibit_checks"]==null|| Request["prohibit_checks"].Length==0)
                return;
            foreach(string userId in Request["prohibit_checks"].Split(','))
            {         
                    User user = db.User.Find(int.Parse(userId));
                    user.prohibit = DateTime.Parse(Request["prohibit"].ToString());
                    db.Entry(user).State = EntityState.Modified;    
            }
            int num= db.SaveChanges();
            Response.Write("<script>alert('已經設定停權資料，共計"+num+"筆。');</script>");
        }
        public string FormatFeildName(int? Id)
        {

             List<string> feilds = new List<string>();
            string names = "";
            if (Id == null) return names;
            SignForm signForm= db.SignForm.Find(Id);
            if (signForm != null)
            {
                string[] fns = signForm.field.Split(',');
                foreach(string th in fns)
                {
                    names += "<th>" + th + "</th>";
                    feilds.Add(th);
                }

            }
            TempData["feilds"] = feilds;
            return names;
        }
        public string FormatFeildValue(string valueJson)
        {
            string view = "";
            JObject values = JsonConvert.DeserializeObject(valueJson) as JObject;
            List<string> feilds = TempData["feilds"] as List<string>;
            if (feilds != null) {
                foreach(string field in feilds)
                {
                    if (values!=null&&values.ContainsKey(field))
                        view += "<td>" + values[field] + "</td>";
                    else
                        view += "<td></td>";
                }
            }
            return view;
        }

        public string FormatFeildInput(string valueJson,int sId)
        {

            List<string> feilds=null;
            string view = "";
            JObject values = JsonConvert.DeserializeObject(valueJson) as JObject;
            SignForm s= db.SignForm.Find(sId);
            if(s!=null)
               feilds =s.field.Split(',').ToList<string>();
            if (feilds != null)
            {
                foreach (string field in feilds)
                {
                    if (values!=null&&values.ContainsKey(field))
                        view +=string.Format("<div class='form-group'><label class='control-label col-md-2'>{0}</label><div class='col-md-10'><input class='form-control text-box single-line' id='dyn_{0}' name='dyn_{0}' type='text' value='{1}'><span class='field-validation-valid text-danger' data-valmsg-for='{0}' data-valmsg-replace='true'></span></div></div>", field, values[field]);
                    else
                        view += string.Format("<div class='form-group'><label class='control-label col-md-2'>{0}</label><div class='col-md-10'><input class='form-control text-box single-line' id='dyn_{0}' name='dyn_{0}' type='text' value=''><span class='field-validation-valid text-danger' data-valmsg-for='{0}' data-valmsg-replace='true'></span></div></div>", field);
                }
            }
            return view;
        }
        private string ValueSave()
        {
            string json = "";
            foreach(var key in Request.Form.Keys)
            {
                if (key.ToString().StartsWith("dyn_"))
                {
                    json += ",\"" + key.ToString().Replace("dyn_", "") + "\":\"" + Request[key.ToString()]+"\"";
                }

            }
            return "{"+json.Substring(1)+"}";
        }


        // GET: SignRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SignRecords signRecords = db.SignRecords.Find(id);
            if (signRecords == null)
            {
                return HttpNotFound();
            }
            return View(signRecords);
        }

        // GET: SignRecords/Create
        public ActionResult Create()
        {
            ViewBag.sId = new SelectList(db.SignForm, "sId", "field");
            ViewBag.userId = new SelectList(db.User, "userId", "userName");
            return View();
        }

        // POST: SignRecords/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sId,userId,signData,isComplete,completeDatetime")] SignRecords signRecords)
        {
            if (ModelState.IsValid)
            {
                db.SignRecords.Add(signRecords);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.sId = new SelectList(db.SignForm, "sId", "field", signRecords.sId);
            ViewBag.userId = new SelectList(db.User, "userId", "userName", signRecords.userId);
            return View(signRecords);
        }

        // GET: SignRecords/Edit/5
        public ActionResult Edit(int? sid,int? userId)
        {
            if (sid == null && userId==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SignRecords signRecords = db.SignRecords.Where(i=>i.sId==sid && i.userId==userId).FirstOrDefault();
            return View(signRecords);
        }

        // POST: SignRecords/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "sId,userId,isComplete,completeDatetime")] SignRecords signRecords)
        {
            if (ModelState.IsValid)
            {
                signRecords.signData = ValueSave();
                db.Entry(signRecords).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { Id = signRecords.sId });
            }
            //ViewBag.sId = new SelectList(db.SignForm, "sId", "field", signRecords.sId);
            //ViewBag.userId = new SelectList(db.User, "userId", "userName", signRecords.userId);
            return View(signRecords);
        }

        // GET: SignRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SignRecords signRecords = db.SignRecords.Find(id);
            if (signRecords == null)
            {
                return HttpNotFound();
            }
            return View(signRecords);
        }

        // POST: SignRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int userId,int sId)
        {
            SignRecords signRecords = db.SignRecords.Where(i=>i.userId==userId && i.sId==sId).FirstOrDefault();
            db.SignRecords.Remove(signRecords);
            db.SaveChanges();
            return RedirectToAction("Index", new { Id = signRecords.sId });
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
