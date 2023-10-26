using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public partial class UInew_Setting
{
    [ContextMenu("Turn on setting")]
    void DisplayTrue()
    {
        Display(true);
    }
    [ContextMenu("Turn off setting")]
    void DisplayFalse()
    {
        Display(false);
    }
}

