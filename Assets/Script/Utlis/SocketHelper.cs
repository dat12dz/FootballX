using Assets.Script.Utlis;
using Assets.Utlis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Script.NetCode
{
    static class SocketHelper
    {
        public static void isSocketConnect(Socket s)
        {
            ThreadHelper.SafeThreadCall(() =>
            {
                while (true)
                {
                    try
                    {
                        s.Send(new byte[4] { 0, 0, 0, 0 });
                    }
                    catch (SocketException) {
                        if (s.Connected)
                        {
                          //  NetworkSystem.instance.TryDisconnectoall();
                            Logging.LogError("network timeout");
                            break;
                        }
                    }
                    Thread.Sleep(30000);
                }
            });
        }
    }
}
