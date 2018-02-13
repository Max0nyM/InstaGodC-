using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace InstaGod_By_MadMax
{


 
    public class ActionPars
    {
        public bool fromTag = false;
        public UserInfo user;
        public string TagOrUser;
        public int actions;
        public int progress=0;
        public string path;
    }


    public class ActionFollow
    {
        public UserInfo user;
        public Queue<string> FollowList;
        public int workTime;
        public int progress = 0;
        
    }

    public class ActionLike
    {
        public UserInfo user;
        public Queue<string> LikeList;
        public int workTime;
        public int progress = 0;

    }



    public class ActionUnFollow
    {
        public UserInfo user;
        public int workTime;
        public bool atTheend;
        public int action;
        public int progress = 0;

    }


    public class Job
    {

        public bool endJob = false;
        private static List<ActionPars> ParsList = new List<ActionPars>();
        private static List<ActionFollow> FollowList = new List<ActionFollow>();
        private static List<ActionLike> LikeList = new List<ActionLike>();
        private static List<ActionUnFollow> UnFollowList = new List<ActionUnFollow>();

        public void CreateAction(string type, string[] data, UserInfo User, Queue<string> userList = null)
        {
            if (type.Contains("pars"))
            {
                bool fromtag = type.Contains("tag");
                ActionPars Pars = new ActionPars
                {

                    fromTag = fromtag,
                    user = User,
                    TagOrUser = data[0],
                    actions = Convert.ToInt32(data[1]),
                    path = data[2]
                };
                ParsList.Add(Pars);
                Globals.Acclist.ChangeAccount(User);
            }


            if (type.Contains("follow"))
            {

                ActionFollow Follow = new ActionFollow
                {
                    user = User,
                    workTime = Convert.ToInt32(data[0]),
                    FollowList = userList
                };
                FollowList.Add(Follow);
                Globals.Acclist.ChangeAccount(User);
            }


            if (type == "unfollow")
            {

                ActionUnFollow UnFollow = new ActionUnFollow
                {
                    user = User,
                    workTime = Convert.ToInt32(data[0]),
                    atTheend = Convert.ToBoolean(data[1]),
                    action = Convert.ToInt32(data[2])
                };
                UnFollowList.Add(UnFollow);
                Globals.Acclist.ChangeAccount(User);
            }


            if (type == "like")
            {

                ActionLike Like = new ActionLike
                {
                    user = User,
                    workTime = Convert.ToInt32(data[0]),
                    LikeList = userList
                };
                LikeList.Add(Like);
                Globals.Acclist.ChangeAccount(User);
            }
        }


        public void GetActions(UserInfo user)
        {

            var User = ParsList.Find(item => item.user == user && item.user.status == "Ожидание");
            var UserFollow = FollowList.Find(item => item.user == user && item.user.status == "Ожидание");
            var UserLike = LikeList.Find(item => item.user == user && item.user.status == "Ожидание");
            var UnUserFollow = UnFollowList.Find(item => item.user == user && item.user.status == "Ожидание");

            if (user.task == "Фолловинг")
                StartAction(UserFollow);
            if (user.task == "АнФолловинг")
                StartAction(UnUserFollow);
            if (user.task == "Парсинг")
                StartAction(User);
            if (user.task == "Лайкинг")
                StartAction(UserLike);

        }

        public void StartAction(object Action)
        {
            if (Action.GetType() == typeof(ActionPars))
            {
                var parser = (ActionPars) Action;
                var user = parser.user;
                user.task = "Парсинг";
                user.status = "Работает";
                Globals.Acclist.ChangeAccount(user);
                Thread Make = new Thread(parsJob);
                Make.IsBackground = true;
                Make.Start(Action);


            }

            if (Action.GetType() == typeof(ActionUnFollow))
            {
                var Unfollower = (ActionUnFollow) Action;
                var user = Unfollower.user;
                user.task = "АнФолловинг";
                user.status = "Работает";
                Globals.Acclist.ChangeAccount(user);
                Thread Make = new Thread(UnfollowJob);
                Make.IsBackground = true;
                Make.Start(Action);
            }

            if (Action.GetType() == typeof(ActionFollow))
            {
                var follower = (ActionFollow) Action;
                var user = follower.user;
                user.task = "Фолловинг";
                user.status = "Работает";
                Globals.Acclist.ChangeAccount(user);
                Thread Make = new Thread(followJob);
                Make.IsBackground = true;
                Make.Start(Action);
            }


            if (Action.GetType() == typeof(ActionLike))
            {
                var liker = (ActionLike) Action;
                var user = liker.user;
                user.task = "Лайкинг";
                user.status = "Работает";
                Globals.Acclist.ChangeAccount(user);
                Thread Make = new Thread(likeJob);
                Make.IsBackground = true;
                Make.Start(Action);
            }
        }

        void InstaSleep(int actPerDay, int WorkTime)
        {
            Thread.Sleep((3600000 * WorkTime) / actPerDay);
        }


        private void UnfollowJob(object unfollower)
        {
            var folowData = (ActionUnFollow) unfollower;
            var UnFollowList0 = new List<string>();
            var Api = new InstaGodApi();
            var maxid = "";
            var cound = 0;

            while (cound < folowData.user.following)
            {
                var info = Api.GetUserFollowowing("", maxid, folowData.user);
                if (info != null)
                {
                    cound += Convert.ToInt32(info["num_results"]);
                    Console.WriteLine(cound);
                    folowData.progress = cound;

                    foreach (var id in info["users"])
                        UnFollowList0.Add(id["pk"].ToString());
                    if (info["next_max_id"] != null)
                        maxid = info["next_max_id"].ToString();
                    else
                        break;
                }


            }

            var i = 0;
            Console.WriteLine("PARSER ACTIONS: " + UnFollowList0.Count);
            if (folowData.atTheend)
                while (i < folowData.action)
                {
                    var user = UnFollowList0.Last();
                    UnFollowList0.Remove(user);
                    endJob = false;
                    var req = Api.unfollow(user, folowData.user);
                    if (req == 0) break;
                    folowData.progress = i;
                    folowData.user.process = i;
                    Globals.Acclist.ChangeAccount(folowData.user);
                    i++;
                    InstaSleep(UnFollowList0.Count, folowData.workTime);
                }
            else
                foreach (var user in UnFollowList0)
                {
                    UnFollowList0.Remove(user);
                    endJob = false;
                    var req = Api.unfollow(user, folowData.user);
                    if (req == 0) break;
                    i++;
                    folowData.progress = i;
                    folowData.user.process = i;
                    Globals.Acclist.ChangeAccount(folowData.user);
                    if (UnFollowList.Count <= folowData.action)
                        break;
                    InstaSleep(UnFollowList.Count, folowData.workTime);
                }

            UnFollowList.Remove(folowData);
            folowData.user.status = "Завершено";
            Globals.Acclist.ChangeAccount(folowData.user);
            endJob = true;
        }



        void likeJob(object liker)
        {
            var folowData = (ActionLike) liker;
            InstaGodApi Api = new InstaGodApi();

            int i = 0;
            Console.WriteLine("PARSER ACTIONS: " + folowData.LikeList.Count);
            foreach (var user in folowData.LikeList)
            {


                var info = Api.like(user, folowData.user);
                if (info != null)
                {
                    i++;
                    folowData.progress = i;
                    folowData.user.process = i;
                    Globals.Acclist.ChangeAccount(folowData.user);
                    InstaSleep(folowData.LikeList.Count, folowData.workTime);
                }
                else
                {
                    break;
                }
            }
            LikeList.Remove(folowData);
            folowData.user.status = "Завершенo";
            Globals.Acclist.ChangeAccount(folowData.user);
            endJob = true;
        }


        void followJob(object follower)
        {
            var folowData = (ActionFollow) follower;
            InstaGodApi Api = new InstaGodApi();

            int i = 0;
            Console.WriteLine("PARSER ACTIONS: " + folowData.FollowList.Count);
            foreach (var user in folowData.FollowList)
            {

                endJob = false;
                Api.follow(user, folowData.user);
                i++;
                folowData.progress = i;
                folowData.user.process = i;
                Globals.Acclist.ChangeAccount(folowData.user);
                Thread.Sleep(new Random().Next(152000, 172000));
                //InstaSleep(folowData.FollowList.Count, folowData.workTime);

            }
            folowData.user.status = "Завершенo";
            FollowList.Remove(folowData);
            Globals.Acclist.ChangeAccount(folowData.user);
            endJob = true;
        }


        void parsJob(object parserz)
        {
            var parser = (ActionPars) parserz;
            InstaGodApi Api = new InstaGodApi();
            string maxid = "";
            int i = 0;
            Console.WriteLine("PARSER ACTIONS: " + parser.actions);
            List<string> resuultStrings = new List<string>();
            if (parser.fromTag)
            {
                while (i <= parser.actions)
                {
                    var info = Api.parsTagFeed(parser.TagOrUser, maxid, parser.user);
                    i += Convert.ToInt32(info["num_results"]);
                    parser.progress = i;
                    parser.user.process = i;
                    Console.WriteLine(maxid);
                    Globals.Acclist.ChangeAccount(parser.user);
                    foreach (var id in info["items"])
                    {
                        resuultStrings.Add(id["id"].ToString());
                    }

                    File.WriteAllLines(parser.path, resuultStrings.ToList());
                    if (info["next_max_id"] != null)
                        maxid = info["next_max_id"].ToString();
                    else
                        break;
                }
            }
            else
            {
                List<string> likersList = new List<string>();
                UserInfo Donor = new UserInfo();
                Donor.login = parser.TagOrUser;
                var user_id = Api.GetInfo(Donor);
                
                while (i <= parser.actions)
                {
                    var info = Api.parsUserFeed(user_id["user"]["id"].ToString(), maxid, parser.user);
                    i += Convert.ToInt32(info["num_results"]);
                    foreach (var id in info["items"])
                    {
                        resuultStrings.Add(id["id"].ToString());
                    }
                    if (info["next_max_id"] != null)
                        maxid = info["next_max_id"].ToString();
                    else
                        break;
                }
                i = 0;
                foreach (var photo in resuultStrings)
                {
                    var info = Api.GetMediaLikers(photo, parser.user);
                    int count = 0;
                    foreach (var id in info["users"])
                    {
                        likersList.Add(id["pk"].ToString());
                        count++;
                    }
                    i += count;
                    parser.progress = i;
                    parser.user.process = i;
                    Console.WriteLine(maxid);
                    Globals.Acclist.ChangeAccount(parser.user);

                }
                File.WriteAllLines(parser.path, likersList.ToList());


                parser.user.status = "Завершенo";
                ParsList.Remove(parser);
                Globals.Acclist.ChangeAccount(parser.user);
                endJob = true;

            }

        }
    }

}
