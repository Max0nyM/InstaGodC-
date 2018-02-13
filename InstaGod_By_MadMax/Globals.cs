using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaGod_By_MadMax
{
    public  class Globals
    {
        private static Job JobMake = new Job();
        private static int t = 5;
        private static Accounts Test =  new Accounts();
        private static List<UserInfo>  MainUserList = new List<UserInfo>();
        public Globals()
        {

         
        }

        public static bool isJobEnd
        {
            get
            {
                return JobMake.endJob;
            }
        }

        public static Accounts Acclist
        {
            get
            {
               return Test;
            }
        }
         public static List<UserInfo> GetMainUserList
        {
            get
            {
                MainUserList = Test.GetAccounts();
                return MainUserList;
            }
        }

        public static Job JobMaker
        {
            get
            {
                return JobMake;
            }
        }
    }
}
