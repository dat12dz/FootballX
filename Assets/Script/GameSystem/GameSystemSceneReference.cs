using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
 
    [Serializable]
    public class GameSystemSceneReference
    {
        [Header("Prefab")]
        [SerializeField]
        public Ball ballPrefab;
        [Header("InScene Reference")]
        public Ball ball;
        public Transform ballSpawnPos;
        public Transform GameSystem;

        public void Init() 
        { 
            ball = GameObject.Instantiate(ballPrefab, ballSpawnPos.position,Quaternion.identity,GameSystem);
            ball.NetworkObject.Spawn();
                
        }
    }