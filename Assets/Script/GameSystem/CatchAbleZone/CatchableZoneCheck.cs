using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CatchableZoneCheck : MonoBehaviour
{
   [SerializeField] TeamEnum team;

    private void OnTriggerEnter(Collider other)
    {
       Ball ball = other.GetComponent<Ball>(); 
        if (team == TeamEnum.Red)
        {
            ball.OnCatchableZone = OnCatchableZoneEnum.Red;
        }
        else
        {
            ball.OnCatchableZone = OnCatchableZoneEnum.Blue;
        }
    }
     void OnTriggerExit(Collider other)
    { 
        Ball ball = other.GetComponent<Ball>();
        ball.OnCatchableZone = OnCatchableZoneEnum.None;

    }

}

