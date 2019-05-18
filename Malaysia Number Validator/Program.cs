using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Web.Script.Serialization;

namespace Malaysia_Number_Validator
{
    class Program
    {
        static readonly string BASE_URI = "https://www.11street.my/pay/orderProcess/doSaleReserve.do";
        static JavaScriptSerializer jss = new JavaScriptSerializer();

        static readonly object locker = new object();

        static void Main(string[] args)
        {
            var title = "Malaysian Phone Number Validator - Coded by Sky";
            Console.Title = title;
            string isp;

#if TEST
            int celcom = 0;
            int hotlink = 0;
            int digi = 0;
            int startIndex = 0;

            if (File.Exists("celcom.txt"))
            {
                celcom = ParseNumberFromPath("celcom.txt");
            }
            if (File.Exists("hotlink.txt"))
            {
                hotlink = ParseNumberFromPath("hotlink.txt");
            }
            if (File.Exists("digi.txt"))
            {
                digi = ParseNumberFromPath("digi.txt");
            }

            if (celcom != 0 && hotlink != 0 && digi != 0)
            {
                var numArray = new int[] { celcom, digi, hotlink };
                startIndex = GetGreaterNumber(numArray);
            }

            Parallel.For(startIndex, 9999999, new ParallelOptions { MaxDegreeOfParallelism = 5 }, i =>
            {
                
            });
#endif
            for (int i = 80000; i < 9999999; i++)
            {
                var combination = string.Format("{0:D7}", i);
                string phoneNumber = $"014{combination}";
                Console.Title = $"{title} - Current Phone Number : {phoneNumber}"/*| Start from {startIndex.ToString().Substring(2)}" */;
                if (IsValidNumber(phoneNumber, out isp))
                {
                    using (StreamWriter sw = File.AppendText($"{isp}.txt"))
                    {
                        sw.WriteLine(phoneNumber);
                    }
                }
            }


#if DEBUG
            Console.ReadKey();
#endif
        }

        public static bool IsValidNumber(string phoneNumber, out string isp)
        {
            isp = "Unknown";
            if (IsCelcom(phoneNumber))
            {
#if DEBUG
                Console.WriteLine("Celcom");
#endif
                isp = "celcom";
                return true;
            }
            else if (IsDigi(phoneNumber))
            {
#if DEBUG
                Console.WriteLine("Digi");
#endif
                isp = "digi";
                return true;
            }
            else if (IsHotlink(phoneNumber))
            {
#if DEBUG
                Console.WriteLine("Hotlink");
#endif
                isp = "hotlink";
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsCelcom(string phoneNumber)
        {
            bool result = false;
            try
            {
                var postData = new NameValueCollection();
                postData["eCouponSms1"] = phoneNumber;
                postData["orgAmount"] = "1000";
                postData["sellerPrdCd"] = "019;00000010";
                postData["prdNo"] = "12424480";
                postData["prdNm"] = "Xpax RM10 Reload + 5% discount";
                postData["sellerMemNo"] = "51101187";

                if (POST(postData))
                {
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static bool IsDigi(string phoneNumber)
        {
            bool result = false;
            try
            {
                var postData = new NameValueCollection();
                postData["eCouponSms1"] = phoneNumber;
                postData["orgAmount"] = "1000";
                postData["sellerPrdCd"] = "016;00000500";
                postData["prdNo"] = "15395118";
                postData["prdNm"] = "Digi RM 5 Reload";
                postData["sellerMemNo"] = "51100737";

                if (POST(postData))
                {
                    result = true;
                }
            }
            catch { result = false; }
            return result;
        }

        public static bool IsHotlink(string phoneNumber)
        {
            bool result = false;
            try
            {
                var postData = new NameValueCollection();
                postData["eCouponSms1"] = phoneNumber;
                postData["orgAmount"] = "500";
                postData["sellerPrdCd"] = "017;00000500";
                postData["prdNo"] = "15395407";
                postData["prdNm"] = "Hotlink RM 5 Reload";
                postData["sellerMemNo"] = "51100737";

                if (POST(postData))
                {
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static bool POST(NameValueCollection postData)
        {
            using (var client = new WebClientX())
            { 
                var data = client.UploadValues(BASE_URI, postData);
                var json = jss.Deserialize<List<ElevenStreet>>(Encoding.ASCII.GetString(data));
                if (json[0].ret.Equals("success"))
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetGreaterNumber(int[] numArray) => numArray.Max();

        public static int ParseNumberFromPath(string path)
        {
            int res = 0;
            try
            {
                var file = File.ReadAllLines(path);
                var lastIndex = file[file.Length - 1];

                return int.Parse(lastIndex);
            }
            catch
            {
                
            }
            return res;
        }

    }

    public class ElevenStreet
    {
        public string rtnMsg { get; set; }
        public string ret { get; set; }
        public string planName { get; set; }
        public string result { get; set; }
        public string timeoutYn { get; set; }
        public string rtnCd { get; set; }
        public string suspendDate { get; set; }
        public object areaId { get; set; }
        public string sndScssYn { get; set; }
        public string h2hTime { get; set; }
        public string balance { get; set; }
        public string disableDate { get; set; }
        public string retCode { get; set; }
        public string mthNm { get; set; }
    }

    public class WebClientX : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var w = base.GetWebRequest(address) as HttpWebRequest;
            w.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36";
            w.Timeout = 60000;
            
            return w;
        }
    }
}
