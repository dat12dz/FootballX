using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets
{
    internal class WarmUpPhase : BasicPhase , IMatchPhase
    {

        public WarmUpPhase()
        {
            Name = "Warm up phase";
            Length = 60 * 5;
            isContinue = true;
        }
        public  override void Begin()
        {
           // All login in here
           base.Begin();
        }
        public override void Pause(int sec)
        {
            base.Pause(sec);
        }
        public override void Resume()
        {
            base.Resume();
        }
        public override void End()
        {
            base.End();
        }

        public override void Pause()
        {
           base.Pause();
        }
    }
}
