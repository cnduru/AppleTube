using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleTube
{
    public static class CommandHelper
    {
        public static string getPlaybackCommand(string mode)
        {
            return String.Format("POST /rate?value={0} HTTP/1.1\n" +
                   "User-Agent: iTunes/10.6 (Macintosh; Intel Mac OS X 10.7.3) AppleWebKit/535.18.5\n" +
                   "Content-Length: 0\n", mode);
        }

        public static string getPlayBody(string url, string filename)
        {
            string req = String.Format("POST /play HTTP/1.1\n" +
                   "User-Agent: MediaControl/1.0\n" +
                   "Content-Type: text/parameters\n" +
                   "Content-Length: {0}\n" +
                   "X-Apple-Session-ID: fb6d816a-a5ad-4e8f-8830-9642b6e6eb35\n\n", getPlayContent(url, filename).Length + 1);

            return req + getPlayContent(url, filename);
        }

        public static string getPlayContent(string url, string filename)
        {
            // make a textbox for server address
            return String.Format("Content-Location: http://192.168.0.10:80/{0}\n" +
                   "Start-Position: 0\n", filename);
        }
    }
}
