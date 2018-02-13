using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using xNet;
namespace InstaGod_By_MadMax
{

    

    public class UserInfo
    {
        public string login, password;
        public int followers;
        public int following;
        public int posts;
        public string status;
        public string task;
        public int process;
        public string uuid;
        public string phone_id;
        public string device_id;
        public string agent;
        public string guid;
        public CookieDictionary cookie;
    }
 

    public class Accounts
    {
      public JArray Users = new JArray();
      public List<UserInfo> MainUserList = new List<UserInfo>();


      public Accounts()
        {
            try
            {

                string strJson = File.ReadAllText(@"data");
                var serialized = JsonConvert.DeserializeObject(strJson);
                JObject rss = JObject.Parse(serialized.ToString());
                Users = (JArray)rss["Users"];

                foreach (var user in Users)
                {
                   MainUserList.Add(JsonConvert.DeserializeObject<UserInfo>(user.ToString()));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("test");
            }
        
        }

  

        public List<UserInfo> GetAccounts()
        {
            return MainUserList;
        }

        public UserInfo GetAccount(int id)
        {
            string strJson = File.ReadAllText(@"data");
            var serialized = JsonConvert.DeserializeObject(strJson);
            JObject rss = JObject.Parse(serialized.ToString());
            Users = (JArray)rss["Users"];
            var UserJS = Users[id];
            
            UserInfo User = new UserInfo
            {
                login = UserJS["login"].ToString(),
                password = UserJS["password"].ToString(),
                agent = UserJS["agent"].ToString(),
                followers = Convert.ToInt32(UserJS["followers"]),
                following = Convert.ToInt32(UserJS["following"]),
                device_id = UserJS["device_id"].ToString(),
                guid = UserJS["guid"].ToString(),
                phone_id = UserJS["phone_id"].ToString(),
                posts = Convert.ToInt32(UserJS["posts"]),
                process = Convert.ToInt32(UserJS["process"]),
                status = UserJS["status"].ToString(),
                task = UserJS["task"].ToString(),
                cookie = null
            };

            return User;


        }

        public void ChangeAccount(UserInfo ChUser)
        {
            try
            {


            int id = MainUserList.FindIndex(a => a.login == ChUser.login);
            MainUserList[id] = ChUser;
            JObject o = new JObject();
            o["Users"] = JToken.FromObject(MainUserList);

            for (int i = 0; i <= MainUserList.Count - 1; i++)
                o["Users"][i]["cookie"] = "";
            var serialized = JsonConvert.SerializeObject(o);
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (var sw = new StreamWriter(@"data"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, serialized);
            }

            }
            catch (Exception e)
            {
                
            }

        }

        public void DeleteAccount(UserInfo ChUser)
        {

            int id = MainUserList.FindIndex(a => a.login == ChUser.login);
            MainUserList.RemoveAt(id);
          
            JObject o = new JObject();
            o["Users"] = JToken.FromObject(MainUserList);

            var serialized = JsonConvert.SerializeObject(o);
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (var sw = new StreamWriter(@"data"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, serialized);
            }


        }

        public void AddAcount(string login,string pass,int followers=0,int following=0,int posts=0,string status="-",string task="-",int process =0)
        {
            Random _rnd = new Random();
            string uuid = Guid.NewGuid().ToString();
            string device_id = "android-" + Functions.HMAC(_rnd.Next(1000, 99999).ToString()).Substring(0, Math.Min(64, 16));
            string phone_id = Guid.NewGuid().ToString();
            string guid = Guid.NewGuid().ToString();
            string agent = Functions.RandomUserAgentInsta();
            var data = new UserInfo{ login = login, password = pass, followers = followers, following = following,posts = posts,task=task, process = process, status = status,uuid=uuid,phone_id = phone_id, device_id = device_id, agent=agent, guid = guid };
            MainUserList.Add(data);
            JObject o = new JObject();
            o["Users"] = JToken.FromObject(MainUserList);
            for (int i = 0; i <= MainUserList.Count - 1; i++)
            o["Users"][i]["cookie"] = "";
            var serialized = JsonConvert.SerializeObject(o);
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (var sw = new StreamWriter(@"data"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, serialized);
            }

        }

   




    }

    }

