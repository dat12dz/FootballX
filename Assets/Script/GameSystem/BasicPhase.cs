using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    internal class BasicPhase : IMatchPhase
    {
        public string Name { get; set; }
        public int Length { get; set; }
        public bool isContinue { get; set; }

        public virtual void Begin()
        {
            UIHandler.Instance.ShowInformation(Name,"Start a new game phase",3);
        }
        public virtual void Pause(int sec)
        {
            UIHandler.Instance.ShowInformation("Pause", "Start a new game phase", 3);
            isContinue = false;
            Time.timeScale = 0;
            Task.Delay(sec * 1000).ContinueWith((TaskRes) => {
                MainThreadDispatcher.ExecuteInMainThread(() => { Resume(); });
                Resume();
            });
          
        }
        public virtual void Resume()
        {
            isContinue = true;
            Time.timeScale = 1;
        }
        public virtual void End()
        {
            
        }

        public virtual void Pause()
        {

        }
    }
}
