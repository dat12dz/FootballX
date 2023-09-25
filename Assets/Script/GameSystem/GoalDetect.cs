using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalDetect : MonoBehaviour
{
   public GameSystem.Team ownerTeam;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("H321o");
        if (other.tag == "Ball")
        {
            switch (ownerTeam)
            {
                case GameSystem.Team.blue:
                    GameSystem.instance.ScoreRedTeam++;
                    break;
                case GameSystem.Team.red:
                    GameSystem.instance.ScoreBlueTeam++;
                    break;
                default:
                    Debug.Log("Please assign team for the goal");
                    break;
            }
        }    
    }
    // Update is called once per frame

}
