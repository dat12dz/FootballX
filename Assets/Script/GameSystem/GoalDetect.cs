using Assets.Script.NetCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalDetect : MonoBehaviour
{
   public GameSystem.Team ownerTeam;
    GameSystem thisGamesystem;
    /// <summary>
    /// /
    /// </summary>
    private void Start()
    {
        thisGamesystem = transform.root.GetComponent<GameSystem>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            var ball = other.GetComponent<Ball>();
            if (!thisGamesystem.MatchAction.MatchPause)
            switch (ownerTeam)
            {
                case GameSystem.Team.blue:
                    thisGamesystem.ScoreRedTeam.Value++;
                        thisGamesystem.MatchAction.OnGoal(ball.lastToucher,TeamEnum.Red);
                    break;
                case GameSystem.Team.red:
                    thisGamesystem.ScoreBlueTeam.Value++;
                        thisGamesystem.MatchAction.OnGoal(ball.lastToucher,TeamEnum.Blue);
                        break;
                default:
                    Debug.Log("Please assign team for the goal");
                    break;

            }
            ball.lastToucher.Score++;
        }    
    }
    
    // Update is called once per frame

}
