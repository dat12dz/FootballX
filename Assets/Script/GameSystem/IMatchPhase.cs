using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assets
{
    internal interface IMatchPhase
    {
      

       public string Name { get; set; }

       public int Length { get; set; }
       bool isContinue { get; set; }
        
        public abstract void Begin();
        public abstract void End();
        public abstract void Pause(int sec);
        public abstract void Resume();
    }
}
