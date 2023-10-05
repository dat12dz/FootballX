using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


    public class WaitForInstaceNotNull<T> : MonoBehaviour
    {
     public static void WaitForInstace(Action then)
    { 
        if (instace_ != null)
        { 
            then();
        }
        else
        {
            WhenInstaceNotNull += then;
        }
     }
    public static void WaitForInstace(Action<T> then)
    {
        if (instance != null)
        {
            then(instance);
        }
        else
        {
            WhenInstaceNotNull += () => then(instace_);
        }
    }

    private void OnDestroy()
    {
  
        instance = default(T);
    }
    static Action WhenInstaceNotNull;
        static T instace_;
        public static T instance 
      { 
            get 
            {
                return instace_;         
            } 
            set {
                instace_ = value; 
                if (WhenInstaceNotNull != null)
                {
                    WhenInstaceNotNull();
                    WhenInstaceNotNull = null;
                }
            }
        }

}

