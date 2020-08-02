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
            int userId = (int)jsondata["userId"];
            bool isProxy = false;
            string deviceId = "";
            User proxyuser=null;
            User user = db.User.Find(userId);
            if (jsondata.ContainsKey("proxy"))
            {
                deviceId = jsondata["proxy"].ToString();
                proxyuser = user;
                user = db.User.Where(u => u.deviceId.CompareTo(deviceId) == 0).FirstOrDefault();
                isProxy = true;
            }
            string str = "";
            if (user != null)
            {
                try
                {
                    string _data = jsondata["data"].ToString();
                    string[] value = decrypt(isProxy?proxyuser.token:user.token, _data).Split(',');
                    if (isProxy)
                    {
                        ProxyDateLog proxyDateLog = new ProxyDateLog();
                        proxyDateLog.data = _data;
                        proxyDateLog.proxyUserId = userId;
                        proxyDateLog.userId = user.userId;
                        proxyDateLog.deviceId = deviceId;
                        proxyDateLog.proxyDate = DateTime.Now;
                        db.ProxyDateLog.Add(proxyDateLog);
                        db.SaveChanges();

                    }
                    if (value[0].ToLower().CompareTo("slvs") == 0)
                    {
                        str = slvoperate(user, value);
                    }
                    else if (value[0].ToLower().CompareTo("sign") == 0)
                    {
                        str = completeSign(user, value);
                    }
                    else if (value[0].ToLower().CompareTo("gold") == 0)
                    {
                        str = golddosome(user, value);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Data);
                }
                return new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            }
            else
            {
                return new HttpResponseMessage { Content = new StringContent("{\"result\":\"掃描失敗，對象找不到。\"}", Encoding.GetEncoding("UTF-8"), "application/json") };
            }
        }
        private string golddosome(User user, string[] value)
        {
            string str = "";
            int goldId = int.Parse(value[1]);
            int sumtotal = db.UserCoinReord.Where(i => i.coinId == goldId && i.cointype == "gold").Count();
            Gold gold = db.Gold.Find(goldId);
            int num = gold.goldnum.GetValueOrDefault();
            int usetimes = user.UserCoinReord.Where((i) => i.coinId == gold.goldId).Count();
            bool isMaxusetimes = gold.userUsetimes != null && (gold.userUsetimes == 0 || gold.userUsetimes > usetimes);
            bool istotalMaxusetimes = gold.totalUsetimes != null && (gold.totalUsetimes == 0 || gold.totalUsetimes > sumtotal);
            //判斷加點還是扣點(正數為扣)
            if (num >= 0)
            {
                //符合條件後可以扣點。
                if (isMaxusetimes && istotalMaxusetimes)
                {
                    if (user.goldnum - num >= 0 && value.Length > 2 && value[2].CompareTo("true") == 0)
                    {
                        user.goldnum = user.goldnum - num;
                        db.Entry(user).State = EntityState.Modified;
                        str += "{\"result\":\"掃描成功，" + (num <= 0 ? "增加金幣數量:" : "扣除金幣數量:") + Math.Abs(num) + "，持有金幣數量：" + user.goldnum + "。\",";
                        str += "\"Id\":" + value[1] + ",\"name\":\"" + gold.goldname + "\",\"isGuid\":\"false\"}";
                        //確認後扣點作業
                    }
                    else if (user.goldnum - num >= 0)
                    {
                        //要先確認
                        return "{\"confirm\":\"true\",\"result\":\"請確認是否兌換？\",\"ld\":\"" + goldId + "\",\"name\":\"" + gold.goldname + "\",\"potos\":\"" + (gold.potos != null ? gold.potos.Split(',')[0] : "") + "\"}";
                    }
                    else
                    {
                        //持有的點數不足
                        return "{\"result\":\"掃描失敗，銀幣數量不足，持有:" + user.goldnum + "，所需數量:" + num + "\"}";
                    }

                }
                else
                {

                    return "{\"result\":\"掃描失敗，使用數量已經到達上限\"}";
                }
                //可掃描數量不足
            }
            else//加點
            {
                user.goldnum = user.goldnum - num;
                db.Entry(user).State = EntityState.Modified;
                str += "{\"result\":\"掃描成功，" + (num <= 0 ? "增加金幣數量:" : "扣除金幣數量:") + Math.Abs(num) + "，持有金幣數量：" + user.goldnum + "。\",";
                str += "\"Id\":" + value[1] + ",\"name\":\"" + gold.goldname + "\",\"isGuid\":\"false\"}";
            }
            UserCoinReord userCoinReord = new UserCoinReord { coinId = goldId, userId = user.userId, num = gold.goldnum, dateGet = DateTime.Now, cointype = value[0] };
            db.UserCoinReord.Add(userCoinReord);
            bool isSuccess = db.SaveChanges() > 0;
            if (isSuccess)
                return str;
            else
                return "{\"result\":\"掃描失敗，發生不明狀態，請聯絡活動人員。\"}";
        }
        private string slvoperate(User user, string[] value)
        {

            string str = "";

            int slvId = int.Parse(value[1]);
            int sumtotal = db.UserCoinReord.Where(i => i.coinId == slvId && i.cointype == "slvs").Count();
            int num;
            int eventNum;
            int eId;
            int goldrate = 0;
            int goldpart = 0;
            int orgin = user.goldnum.GetValueOrDefault();
            UserSlv userSlv = null;
            Slvs slvs = db.Slvs.Find(slvId);
            if (slvs != null)
            {
                num = slvs.slvNum.GetValueOrDefault();
                eventNum = slvs.EventPage.slvLimit.GetValueOrDefault();
                //先判斷是否加點，或是已經確認扣點資料，如果是扣點尚未確認只回傳相關資料
                if (num > 0 || (value.Length > 2 && value[2].CompareTo("true") == 0))
                {
                    if (slvs.EventPage.dateStart > DateTime.Now || slvs.EventPage.dateEnd.Value.AddDays(1) < DateTime.Now)
                        return "{\"result\":\"掃描失敗，非在可累積點數的期限內。\"}";
                    //檢查數量是否可用
                    int usetimes = user.UserCoinReord.Where((i) => i.coinId == slvs.slvId).Count();
                    bool isMaxusetimes = slvs.maxUse != null && (slvs.maxUse == 0 || slvs.maxUse > usetimes);
                    bool istotalMaxusetimes = slvs.totalmaxUse != null && (slvs.totalmaxUse == 0 || slvs.totalmaxUse > sumtotal);
                    //最大數量可用及個人最大數量可用
                    if (isMaxusetimes && istotalMaxusetimes)
                    {
                        eId = slvs.EventPage.eid;
                        goldrate = slvs.EventPage.toGoldNum == null ? 0 : slvs.EventPage.toGoldNum.GetValueOrDefault();
                        //判斷是否已經有相關紀錄
                        userSlv = user.UserSlv.Where((i) => i.eId == eId).FirstOrDefault();
                        //沒有，則新增一筆紀錄
                        if (userSlv == null)
                        {
                            userSlv = new UserSlv { eId = eId, userId = user.userId, currNum = 0, accNum = 0, name = slvs.EventPage.name, dateStart = slvs.EventPage.dateStart, dateEnd = slvs.EventPage.dateEnd };
                            db.UserSlv.Add(userSlv);
                        }
                        else
                        {
                            //扣點時，不做上限控制
                            if (num < 0 || (eventNum == 0 || eventNum >= (userSlv.accNum + num)))
                            {
                                db.Entry(userSlv).State = EntityState.Modified;
                            }
                            else
                            {
                                return str = "{\"result\":\"掃描失敗，累積銀幣已經到達上限:" + eventNum + "\"}";
                            }
                        }

                        if (num > 0)//加點的處理
                        {
                            userSlv.currNum += num;
                            //金幣的處裡
                            if (goldrate != 0)//判斷是否可以取得金幣
                            {
                                if (userSlv.accNum != null)
                                    goldpart = userSlv.accNum.GetValueOrDefault() % goldrate;//判斷累積差多少可以對金幣增加
                                int addGold = (goldpart + num) / goldrate;//判斷增加的量
                                if (addGold > 0)
                                {
                                    user.goldnum = addGold + user.goldnum.GetValueOrDefault();
                                    db.Entry(user).State = EntityState.Modified;
                                }
                            }
                            //銀幣的增加
                            userSlv.accNum += num;
                        }
                        else//扣點的處理
                        {
                            if (userSlv.currNum + num >= 0)
                            {
                                userSlv.currNum += num;
                            }
                            else
                                return str += "{\"result\":\"掃描失敗，銀幣數量不足，持有:" + userSlv.currNum + "，所需數量:" + Math.Abs(num) + "\"}";
                        }
                        UserCoinReord userCoinReord = new UserCoinReord { coinId = slvs.slvId, userId = user.userId, num = slvs.slvNum, dateGet = DateTime.Now, cointype = value[0] };
                        db.UserCoinReord.Add(userCoinReord);
                        bool isSuccess = db.SaveChanges() > 0;
                        if (isSuccess)
                        {
                            str += "{\"result\":\"掃描成功，" + (num >= 0 ? "增加銀幣數量:" : "扣除銀幣數量:") + Math.Abs(num) + "，持有銀幣數量：" + userSlv.currNum + "。" + (orgin >= user.goldnum ? "" : "額外獲得金幣：" + (user.goldnum.GetValueOrDefault() - orgin)) + "\",";
                            str += "\"Id\":" + value[1] + ",\"name\":\"" + slvs.sname + "\",\"isGuid\":\"" + slvs.isGuid.ToString().ToLower() + "\"}";
                            return str;
                        }
                    }
                    else
                    {
                        return str += "{\"result\":\"掃描失敗，使用數量已經到達上限\"}";
                    }
                    //回傳數量不可用
                }
                else
                {
                    userSlv = user.UserSlv.Where((i) => i.eId == slvs.EventPage.eid).FirstOrDefault();
                    if (userSlv == null)
                    {
                        return "{\"result\":\"掃描失敗，資料錯誤。\"}";
                    }
                    else if (DateTime.Now < userSlv.dateStart || DateTime.Now > userSlv.dateEnd.Value.AddDays(1))
                    {
                        return "{\"result\":\"掃描失敗，銀幣非在可使用的期限。\"}";
                    }
                    else
                        return "{\"confirm\":\"true\",\"result\":\"請確認是否兌換？\",\"Id\":\"" + slvId + "\",\"name\":\"" + slvs.sname + "\",\"potos\":\"" + (slvs.potos != null ? slvs.potos.Split(',')[0] : "") + "\"}";
                }
            }
            return "{\"result\":\"掃描失敗，可能已經持有或是超過取得數量或不是可以使用的狀態。\"}";
        }
        private string completeSign(User user, string[] value)
        {
            string str = "";
            int sId = int.Parse(value[1]);
            SignRecords signRecords = user.SignRecords.Where(i => i.sId == sId).FirstOrDefault();
            if (signRecords != null)
            {
                signRecords.isComplete = true;
                signRecords.completeDatetime = DateTime.Now;
                db.Entry(signRecords).State = EntityState.Modified;
                if (db.SaveChanges() > 0)
                    return str = "{\"result\":\"掃描成功，已經完成活動。\"}";
            }
            return str = "{\"result\":\"掃描失敗，發生不明狀態，請聯絡活動人員。\"}";
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
