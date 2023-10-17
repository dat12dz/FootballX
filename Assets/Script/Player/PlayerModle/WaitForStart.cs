using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WaitForStart : MonoBehaviour
{
    bool isStart;
    Action onObjectStart;
    public void WaitForStart_(Action a)
    {
        if (isStart)
        {
            if (onObjectStart != null)
            a();
        }
        else
        {
            onObjectStart += a;
        }
    }
    protected virtual void Start()
    {
        isStart = true;
        if (onObjectStart != null)
        onObjectStart();
    }
}

