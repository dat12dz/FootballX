using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
     static class MatchSystem
    {

        public static int MatchTimeSpeed = 1;
        public static IMatchPhase currentMachPhase;
        public static Queue<IMatchPhase> QueuePhase = new Queue<IMatchPhase>();
        public static Action<IMatchPhase> OnStartPhase,OnEndPhase,OnPausePhase,OnResumePhase;
    }
}
