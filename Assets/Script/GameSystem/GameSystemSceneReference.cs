using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
    public class GameSystemSceneReference
    {
     public TeamSeparateSceneRef RedTeanSceneRef, BlueTeamSceneRef;
        [Header("Prefab")]
        [SerializeField]
        public Ball ballPrefab;
        [Header("In Scene Reference")]
        public Ball ball;
        public Transform ballSpawnPos;
        public Transform GameSystem;
        public Volume PostProcessingVolume;
        
        public void Init(GameSystem g) 
        {
            if (NetworkManager.Singleton.IsServer)
            {
                ball = GameObject.Instantiate(ballPrefab, ballSpawnPos.position, Quaternion.identity, GameSystem);
                ball.NetworkObject.Spawn();
                ball.init(g, ballSpawnPos);
            }     
        }
    }
[Serializable]
public class TeamSeparateSceneRef
{
    public GoalDetect Goal;
    [Header("Corner Kick")]
    public Transform CornerKickBallSetterL, CornerKickBallSetterR;
    public Transform CornerKickPlayerPosL, CornerKickPlayerPosR;
    [Header("Goal Keeper")]
    public Player GoalKeeper;
    public Transform GoalKeeperBallSetter;
}