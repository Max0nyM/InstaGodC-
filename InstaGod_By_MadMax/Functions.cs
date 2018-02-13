using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;



namespace InstaGod_By_MadMax
{
public  class Functions
    {

      
        private static Random Random = new Random();

        public static string HMAC(string String)
        {
            var keyByte = Encoding.UTF8.GetBytes("b03e0daaf2ab17cda2a569cace938d639d1288a1197f9ecf97efd0a4ec0874d7");
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(String));
                return ByteToString(hmacsha256.Hash).ToLower();
            }
        }

        public static string ByteToString(byte[] buff)
        {
            var sbinary = "";
            for (var i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2");
            return sbinary;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return
                new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray()).ToLower();
        }

        public static string Pars(string strSource, string strStart, string strEnd, int startPos)
        {
            var length = strStart.Length;
            var result = "";
            var num = strSource.IndexOf(strStart, startPos);
            var num2 = strSource.IndexOf(strEnd, num + length);
            if (num != -1 & num2 != -1)
            {
                result = strSource.Substring(num + length, num2 - (num + length));
            }
            return result;
        }

        public static string GenerateGuid()
        {
            var rnd = new Random();
            var test = string.Format("{0}-{1}-{2}-{3}-{4}", RandomString(8), RandomString(4), RandomString(4),
                RandomString(4), RandomString(12));
            return test;
        }

        public static string CreateRandomPassword(int passwordLength)
        {
            var allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var chars = new char[passwordLength];
            var rd = new Random();

            for (var i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }
        public static string RandomUserAgentInsta()
        {
            StringBuilder Test = new StringBuilder();
            string[] lines = System.IO.File.ReadAllLines(@"devices.txt");
            Random Number = new Random();
            string device = lines[Number.Next(0, lines.Length - 1)];
            string one = device.Split(';')[0];
            string two = device.Split(';')[1].Split(';')[0];
            string three = device.Split(';')[2];
            Test.AppendFormat("Instagram 10.15.0 Android (19/4.4.2; 480dpi; 1152x1920; {0}; {1}; {2}; qcom; en_US)", one, two, three);

            return Test.ToString();
        }
        public static string CreateRandomUserName(int passwordLength)
        {
            var allowedChars = "abcdefghijkmnopqrstuvwxyz";
            var chars = new char[passwordLength];
            var rd = new Random();

            for (var i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        

        public static string GenerateSignature(string data)
        {
            var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes("b03e0daaf2ab17cda2a569cace938d639d1288a1197f9ecf97efd0a4ec0874d7"));
            hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));

            string teste = null;
            foreach (byte test in hmacsha256.Hash)
            {
                teste += test.ToString("X2");
            }

            return teste;
        }


      


     

        
    }
}
