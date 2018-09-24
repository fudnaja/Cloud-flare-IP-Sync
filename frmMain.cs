using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusCom;
using Newtonsoft.Json.Linq;

namespace CloudflareApp
{
    public partial class frmMain : Form
    {
        System.Globalization.CultureInfo enGB = new System.Globalization.CultureInfo("en-US");
        private FormWindowState currentWindowState = FormWindowState.Normal;
        API api = new API();
        int sync_minute = 30;
        DateTime last_sync = DateTime.Now;
        int process_minute = -1;
        bool process = false;
        Converter vCon = new Converter();
        LogFile vLog = new LogFile();
        public frmMain()
        {
            InitializeComponent();
            string value = ConfigurationManager.AppSettings["SyncTime"].ToString();
            int.TryParse(value, out sync_minute);

            tm_timer.Enabled = true;
            tm_timer.Start();
        }

        private void CheckIP()
        {
            try
            {
                string url = ConfigurationManager.AppSettings["check_ip_url"].ToString();
                string json_res = api.GetIPNow(url);
                json_res = "{\"res\":{\"info\":" + json_res + "}}";
                DataSet ds = vCon.JsonToDataset(json_res);
                if (ds.Tables.Contains("info"))
                {
                    if (ds.Tables["info"].Columns.Contains("ip"))
                    {
                        lblIP.Text = ds.Tables["info"].Rows[0]["ip"].ToString();
                        vLog.WriteLogEvent("IP : "+ lblIP.Text);
                    }
                }

                GetList();
            }
            catch(Exception ex)
            {
                vLog.WriteLogError("frmMain.CheckIP Exception|"+ex.Message);
            }
        }

        private void GetList()
        {
            try
            {
                string get_list_url = ConfigurationManager.AppSettings["get_list_url"].ToString();
                string update_ip_url = ConfigurationManager.AppSettings["update_ip_url"].ToString();
                string www = ConfigurationManager.AppSettings["www"].ToString();
                string api_www = "api." + www;
                string json_res = api.GetDNSLists(get_list_url);
                int lenth_end = json_res.IndexOf("result_info");
                string data = "{\"info\":" + json_res.Substring(0, lenth_end - 2) + "}}";
                DataSet ds = vCon.JsonToDataset(data);
                if (ds.Tables.Contains("result"))
                {
                    for (int i = 0; i < ds.Tables["result"].Rows.Count; i++)
                    {
                        string type = ds.Tables["result"].Rows[i]["type"].ToString();
                        string name = ds.Tables["result"].Rows[i]["name"].ToString();
                        string content = ds.Tables["result"].Rows[i]["content"].ToString();
                        if (name == www && type == "A")
                        {
                            if (content != lblIP.Text.Trim())
                            {
                                string json_req = "{\"type\":\"A\",\"name\":\"" + www + "\",\"content\":\"" + lblIP.Text.Trim() + "\",\"ttl\":120,\"proxied\":false}";
                                string res_json = api.UpdateDNS(json_req, update_ip_url);
                                if (res_json != string.Empty)
                                {
                                    var jsonObj = JObject.Parse(res_json);
                                    string success = (string)jsonObj["success"];
                                    if (success.ToLower() == "true")
                                    {
                                        last_sync = DateTime.Now;
                                        vLog.WriteLogEvent("Updated DNS : " + name + " IP : " + content);
                                    }
                                }
                            }
                        }

                        if (name == api_www && type == "A")
                        {
                            if (content != lblIP.Text.Trim())
                            {
                                string json_req = "{\"type\":\"A\",\"name\":\"" + api_www + "\",\"content\":\"" + lblIP.Text.Trim() + "\",\"ttl\":120,\"proxied\":false}";
                                string res_json = api.UpdateDNS(json_req, update_ip_url);
                                if (res_json != string.Empty)
                                {
                                    var jsonObj = JObject.Parse(res_json);
                                    string success = (string)jsonObj["success"];
                                    if (success.ToLower() == "true")
                                    {
                                        last_sync = DateTime.Now;
                                        vLog.WriteLogEvent("Updated DNS : " + name + " IP : " + content);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                vLog.WriteLogError("frmMain.GetList Exception|" + ex.Message);
                tm_timer.Stop();
                tm_timer.Start();
            }
        }

        public bool CheckInternetConnection()
        {
            try
            {
                System.Net.Sockets.TcpClient tcpClient = new System.Net.Sockets.TcpClient("www.google.co.th", 80);
                tcpClient.Close();
                return true;
            }
            catch (Exception ex)
            {
                vLog.WriteLogError("frmMain.CheckInternetConnection Exception|" + ex.Message);
                tm_timer.Stop();
                tm_timer.Start();
                return false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text+=  " : Version " + Application.ProductVersion;
            lblVersion.Text = "v" + Application.ProductVersion;
            int x;
            int y;
            x = Screen.PrimaryScreen.WorkingArea.Width/2;
            y = Screen.PrimaryScreen.WorkingArea.Height/2;
            this.Location = new Point(x, y);

            CheckIP();
        }

        private void tm_timer_Tick(object sender, EventArgs e)
        {
            tm_timer.Stop();
            lblDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", enGB);

            if (DateTime.Now.Second == 0)
            {
                if (this.WindowState != FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Minimized;
                }
            }

            TimeSpan ts = DateTime.Now - last_sync;
            int differenceInMinutes = ts.Minutes;

            if (CheckInternetConnection() && DateTime.Now.Minute % 10==0 && !process)
            {
                process_minute = DateTime.Now.Minute;
                process = true;
                GetList();
            }

            if (process && (process_minute + 5 < DateTime.Now.Minute))
            {
                process = false;
            }

            if (DateTime.Now.Minute % 2 == 0)
            {
                vLog.WriteLogEvent("Sync IP : " + lblIP.Text);
            }

            if (DateTime.Now.Minute == 57)
            {
                vLog.WriteLogEvent("Auto Exit");
                Application.Exit();
            }
            tm_timer.Start();
        }

        private void CloudflareIcon_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                int x;
                int y;
                x = Screen.PrimaryScreen.WorkingArea.Width / 2;
                y = Screen.PrimaryScreen.WorkingArea.Height / 2;
                this.Location = new Point(x, y);
                this.Show();
                this.WindowState = this.currentWindowState;
            }
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            tm_timer.Stop();
            DialogResult dialogResult = MessageBox.Show("Exit to Cloudflare IP Sync ?", "Exit", MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                //do something
                tm_timer.Enabled = false;
                Application.Exit();
            }
            else
            {
                tm_timer.Start();
            }
        }
    }
}
