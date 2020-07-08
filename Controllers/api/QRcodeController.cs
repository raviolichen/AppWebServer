using AppWebServer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace AppWebServer.Controllers.api
{
    public class QRcodeController : ApiController
    {
        private AppDataBaseEntities db = new AppDataBaseEntities();
        public HttpResponseMessage PostQRCode([FromBody] string data)
        {
            JObject jsondata = JsonConvert.DeserializeObject(data) as JObject;
            User user = db.User.Find((int)jsondata["userId"]);
            string str = "";
            if (user != null)
            {
                try
                {
                    String[] value = decrypt(user.token, jsondata["data"].ToString()).Split(',');
                    if (value[0].ToLower().CompareTo("slvs") == 0)
                    {
                        UserSlv userSlv = slvoperate(user, value);
                        if (slvoperate(user, value) != null)
                        {
                            str += "{result:\"掃描成功，持有銀幣數量：" + userSlv.currNum+"。\",";
                            str += "\"slvId\":" + value[1]+"}";
                        }
                    }
                    if (value[0].ToLower().CompareTo("gold") == 0)
                    {

                    }



                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Data);

                }
            }
            return new HttpResponseMessage { Content = new StringContent("[" + str + "]", Encoding.GetEncoding("UTF-8"), "application/json") };
        }
        private UserSlv slvoperate(User user, string[] value)
        {
            UserSlv userSlv = null;
            if (value[0].ToLower().CompareTo("slvs") == 0)
            {
                int? num;
                int eId;
                int slvId = int.Parse(value[1]);
                EventSlvs Eventslvs = db.EventSlvs.Where(i => i.slvId == slvId).FirstOrDefault();
                if (Eventslvs != null && Eventslvs.slvNum != null)
                {
                    int usetimes = user.UserCoinReord.Where((i) => i.coinId == Eventslvs.slvId).Count();
                    num = Eventslvs.slvNum;
                    eId = Eventslvs.eid;
                    //  UserSlv userSlv = db.UserSlv.SqlQuery("SELECT * FROM UserSlv WHERE eId="+eId+" AND userId="+ user.userId);
                    userSlv = user.UserSlv.Where((i) => i.eId == eId).FirstOrDefault();
                    if (userSlv == null)
                    {

                        userSlv = new UserSlv { eId = eId, userId = user.userId, currNum = 0, accNum = 0, name = Eventslvs.name, dateStart = Eventslvs.dateStart, dateEnd = Eventslvs.dateEnd };
                        db.UserSlv.Add(userSlv);
                    }
                    else
                    {
                        db.Entry(userSlv).State = EntityState.Modified;
                    }
                    userSlv.currNum += num;
                    if (num > 0)
                        userSlv.accNum += num;
                    UserCoinReord userCoinReord = new UserCoinReord { coinId = Eventslvs.slvId, userId = user.userId, num = Eventslvs.slvNum, dateGet = DateTime.Now, cointype = value[0] };
                    db.UserCoinReord.Add(userCoinReord);
                    if (db.SaveChanges() > 0) return userSlv;
                }
            }
            return userSlv;
        }
        private string decrypt(string token, string datas)
        {
            try
            {
                SymmetricAlgorithm des = Rijndael.Create();
                byte[] keyArray = Encoding.UTF8.GetBytes(token.Substring(0, 32));
                byte[] IVArray = Encoding.UTF8.GetBytes(token.Substring(32, 16));
                byte[] toEncryptArray = Convert.FromBase64String(datas);
                des.Key = keyArray;
                des.IV = IVArray;
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = des.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return "";
            }
        }
    }

}
