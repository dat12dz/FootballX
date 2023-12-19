using Assets.Script.NetCode;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ThrowInCheck : MonoBehaviour
{
    GameSystem gameSystem;
    LayerMask GroundMask;
    async void Start()
    {
        gameSystem = await SceneHelper.GetGameSystem(gameObject.scene);
        GroundMask = LayerMask.GetMask("Ground");
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if(NetworkManager.Singleton.IsServer && gameSystem.MatchAction.gameState == GameStateEnum.Playing && !gameSystem.MatchAction.MatchPause)
        {
            if (collision.gameObject != null)
            {
                var Ball = collision.gameObject.GetComponent<Ball>();
                var ThrowInMaker = Ball.lastToucher;
                var ThrowInMakerTeam = Ball.lastToucher.team;
                var ThrowInTeam = ThrowInMakerTeam.GetOpponentTeam();
              var PhysicsScene =  gameObject.scene.GetPhysicsScene();

                RaycastHit hit;
                PhysicsScene.Raycast(Ball.transform.position, Vector3.down, out hit, Mathf.Infinity,GroundMask);

                gameSystem.MatchAction.StartThrowInPhase(new StartThrowInPhase_Info()
                {
                    ThrowInmakaer = ThrowInMaker,
                    ThrowInTeam = ThrowInTeam,
                     ThrowInPosition = hit.point
                });
            }
        }
    }
    
    void Update()
    {
        
    }
}
