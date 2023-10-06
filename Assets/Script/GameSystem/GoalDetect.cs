using Assets.Script.NetCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalDetect : MonoBehaviour
{
   public GameSystem.Team ownerTeam;
    GameSystem thisGamesystem;
    private void Start()
    {
        thisGamesystem = transform.root.GetComponent<GameSystem>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            if (!thisGamesystem.MatchAction.MatchPause)
            switch (ownerTeam)
            {
                case GameSystem.Team.blue:
                    thisGamesystem.ScoreRedTeam.Value++;
                    break;
                case GameSystem.Team.red:
                    thisGamesystem.ScoreBlueTeam.Value++;
                    break;
                default:
                    Debug.Log("Please assign team for the goal");
                    break;
            }
        }    
    }
    
    // Update is called once per frame

}
