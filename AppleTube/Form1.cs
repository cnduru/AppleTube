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
        // globals
        List<string> urls = new List<string>();
        TcpClient tc;

        public Form1()
        {
            InitializeComponent();
            initialize();
        }

        private void initialize()
        {
            textBoxAppleTV.Text = "192.168.0.18";
            webBrowser1.Navigate("youtube.com");

            // buttons
            // adapt switchbuttonmode to incorporate these with a "startup" option or the like
            buttonDisconnect.Enabled = false;
            buttonPause.Enabled = false;
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if(webBrowser1.Url != null && webBrowser1.Url.ToString().Contains("watch"))
            {
                string url = webBrowser1.Url.ToString();

                // to avoid repeat loading of URL
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
            // update buttons
            switchButtonState("play");

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

        private void sendPlayRequest(string url, string filename)
        {
            try
            {
                tc = new TcpClient(textBoxAppleTV.Text, 7000);

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

        private void sendCommand(string cmd)
        {
            try
            {
                TcpClient tc = new TcpClient(textBoxAppleTV.Text, 7000);
                StreamWriter st = new StreamWriter(tc.GetStream());

                st.Write(cmd);
                st.Write("\n\n");
                st.Flush();
                tc.Close();
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
            switchButtonState("stopStream");
            tc.Close();
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            switchButtonState("pause");
            string command = CommandHelper.getPlaybackCommand("0.00000");
            sendCommand(command);
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            switchButtonState("play");
            string command = CommandHelper.getPlaybackCommand("1.00000");
            sendCommand(command);
        }            

        private void switchButtonState(string mode)
        {
            switch(mode)
            {
                case "play":
                    buttonPlay.Enabled = true;
                    buttonDisconnect.Enabled = true;
                    buttonPause.Enabled = true;
                    break;
                case "pause":
                    buttonPlay.Enabled = true;
                    buttonDisconnect.Enabled = true;
                    buttonPause.Enabled = false;
                    break;
                case "stopStream":
                    buttonPlay.Enabled = true;
                    buttonDisconnect.Enabled = false;
                    buttonPause.Enabled = false;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
