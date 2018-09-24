using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CloudflareApp
{
    public class API
    {
        private string WebApiHelper(string routing, string method, string json)
        {
            string url = ConfigurationManager.AppSettings["LogUrl"].ToString();
            try
            {
                HttpWebRequest request;
                HttpWebResponse response;

                request = (HttpWebRequest)WebRequest.Create(url);
                // request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.72 Safari/537.36"
                request.AllowAutoRedirect = true;
                request.ContentType = "application/json; charset=UTF-8";
                request.Method = "POST";
                //request.Headers.Add("app_id", vCon.getAppID());
                //request.Headers.Add("app_key", vCon.getAppKey());
                request.KeepAlive = true;


                byte[] postBytes = UTF8Encoding.UTF8.GetBytes(json);
                request.ContentLength = postBytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string result = reader.ReadToEnd().ToString();
                response.Close();
                return result;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public string WebApiHelper(string url, string json)
        {
            try
            {
                HttpWebRequest request;
                HttpWebResponse response;

                request = (HttpWebRequest)WebRequest.Create(url);
                request.AllowAutoRedirect = true;
                request.ContentType = "application/json; charset=UTF-8";
                request.Method = "POST";
                //request.Headers.Add("app_id", Master.oApp.app_id);
                //request.Headers.Add("app_key", Master.oProfile.api_key);
                request.KeepAlive = true;

                byte[] postBytes = UTF8Encoding.UTF8.GetBytes(json);
                request.ContentLength = postBytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string result = reader.ReadToEnd().ToString();
                response.Close();
                return result;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public string UpdateDNS(string json, string url)
        {
            try
            {
                HttpWebRequest request;
                HttpWebResponse response;

                string auth_key = ConfigurationManager.AppSettings["X-Auth-Key"].ToString();
                string auth_email = ConfigurationManager.AppSettings["X-Auth-Email"].ToString();

                request = (HttpWebRequest)WebRequest.Create(url);
                // request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.72 Safari/537.36"
                request.AllowAutoRedirect = true;
                request.ContentType = "application/json";
                request.Method = "PUT";
                request.Headers.Add("X-Auth-Key", auth_key);
                request.Headers.Add("X-Auth-Email", auth_email);
                request.KeepAlive = true;

                byte[] postBytes = UTF8Encoding.UTF8.GetBytes(json);
                request.ContentLength = postBytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string result = reader.ReadToEnd().ToString();
                response.Close();
                return result;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public string GetIPNow(string url)
        {
            try
            {
                HttpWebRequest request;
                HttpWebResponse response;

                request = (HttpWebRequest)WebRequest.Create(url);
                request.AllowAutoRedirect = true;
                //request.ContentType = "application/json; charset=UTF-8";
                request.Method = "GET";
                //request.Headers.Add("app_id", Master.oApp.app_id);
                //request.Headers.Add("app_key", Master.oProfile.api_key);
                request.KeepAlive = true;

                //byte[] postBytes = UTF8Encoding.UTF8.GetBytes("{}");
                //request.ContentLength = postBytes.Length;

                //Stream requestStream = request.GetRequestStream();
                //requestStream.Write(postBytes, 0, postBytes.Length);
                //requestStream.Close();

                response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string result = reader.ReadToEnd().ToString();
                response.Close();
                return result;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public string GetDNSLists(string url)
        {
            try
            {
                HttpWebRequest request;
                HttpWebResponse response;

                string auth_key = ConfigurationManager.AppSettings["X-Auth-Key"].ToString();
                string auth_email = ConfigurationManager.AppSettings["X-Auth-Email"].ToString();

                request = (HttpWebRequest)WebRequest.Create(url);
                request.AllowAutoRedirect = true;
               // request.ContentType = "application/json; charset=UTF-8";
                request.Method = "GET";
                request.Headers.Add("X-Auth-Key", auth_key);
                request.Headers.Add("X-Auth-Email", auth_email);
                request.KeepAlive = true;

                response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string result = reader.ReadToEnd().ToString();
                response.Close();
                return result;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
