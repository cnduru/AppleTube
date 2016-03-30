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
    }
}
