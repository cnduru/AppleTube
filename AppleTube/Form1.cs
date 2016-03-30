using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppleTube
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //webBrowser1.Navigate("www.youtubeinmp3.com/fetch/?video=http://www.youtube.com/watch?v=i62Zjga8JOM");
            webBrowser1.Navigate("youtube.com");
        }

        List<string> urls = new List<string>();

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if(webBrowser1.Url != null && webBrowser1.Url.ToString().Contains("watch"))
            {
                string url = webBrowser1.Url.ToString();

                // to avoid repeat loading of URLs
                if (!urls.Contains(url))
                {
                    // thread parameterize this
                    download("http://www.youtubeinmp3.com/fetch/?video=" + url);
                    //sendPlayRequest("");
                }

                urls.Add(url);

                //webBrowser1.Navigate("http://www.youtube.com");
            } 
            
        }

        private void webBrowser1_FileDownload(object sender, EventArgs e)
        {
            // remove this
        }

        private void download(string url)
        {
            string fileName = url.Substring(url.IndexOf("v=") + 2) + ".mp3";

            // download file
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myHttpWebRequest.MaximumAutomaticRedirections = 10;
            myHttpWebRequest.AllowAutoRedirect = true;
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            using (var responseStream = myHttpWebResponse.GetResponseStream()) 
            {
                using (var fileStream = new FileStream(@"C:\Users\Avalon\Desktop\php\" + fileName, FileMode.Create)) 
                {
                    responseStream.CopyTo(fileStream);
                }
            }

            sendPlayRequest("http://192.168.0.13:80/" + fileName, fileName);
        }

        TcpClient tc;

        private void sendPlayRequest(string url, string filename)
        {
            try
            {
                tc = new TcpClient("192.168.0.18", 7000);

                StreamWriter st = new StreamWriter(tc.GetStream());
                StreamReader sr = new StreamReader(tc.GetStream());
                string body = getPlayBody(url, filename);
                st.Write(body);
                st.Write("\n\n");
                st.Flush();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string getPlayBody(string url, string filename)
        {
            string req = String.Format("POST /play HTTP/1.1\n" +
                   "User-Agent: MediaControl/1.0\n" +
                   "Content-Type: text/parameters\n" +
                   "Content-Length: {0}\n" +           
                   "X-Apple-Session-ID: fb6d816a-a5ad-4e8f-8830-9642b6e6eb35\n\n", getPlayContent(url, filename).Length + 1);

            return req + getPlayContent(url, filename);
        }

        private string getPlayContent(string url, string filename)
        {
            return String.Format("Content-Location: http://192.168.0.13:80/{0}\n" +
                   "Start-Position: 0\n", filename);
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            tc.Close();
        }            
    }
}
