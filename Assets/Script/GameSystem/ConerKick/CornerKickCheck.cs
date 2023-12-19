using Assets.Script.NetCode;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class CornerKickCheck : MonoBehaviour
{
    public TeamEnum OwnerGoalteam;

    GameSystem gameSystem;
    private async void Start()
    {
        gameSystem = await SceneHelper.GetGameSystem(gameObject.scene);
    }
    private void OnCollisionEnter(Collision collision)
    {
        // lấy player shoot quả bóng
        // Nếu player cùng
        Ball ball = collision.gameObject.GetComponent<Ball>();
        Player PlayerShootTheBall = ball.lastToucher;
        var CornerKickMakerTeam = PlayerShootTheBall.team.GetOpponentTeam();
        if (PlayerShootTheBall.team.team == OwnerGoalteam)
        {
           
            Player CornerKickTaker = CornerKickMakerTeam.GetRandomPlayer();
            var StartCornerKickInfo_Instance = new StartCornerKickPhase_Info()
            {
                CornerKickMaker = PlayerShootTheBall,
                ConerKickTaker = CornerKickTaker,
            };
            gameSystem.MatchAction.StartConerKickPhase(StartCornerKickInfo_Instance);
        }
        else
        {
            gameSystem.MatchAction.GoalKeeperTakeTheBall(PlayerShootTheBall.initialPlayerData.Value.playerName.ToString(), CornerKickMakerTeam);
        }
    }
}

