using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Script.Networking.NetworkRoom
{
    public class SlotSwapRequest
    {
        public SlotSwapRequest(CancellationTokenSource token) {
            CancelToken = token;
           
        }
      public  CancellationTokenSource CancelToken = new CancellationTokenSource();
       public bool accept;
       
    }
}
