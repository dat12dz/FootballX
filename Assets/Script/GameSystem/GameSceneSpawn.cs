using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

internal class GameSceneSpawn   :MonoBehaviour
{
     [SerializeField]   GameSystem gameSystem;
    [SerializeField] public GameSystem currGameSystem;
   public void StartSpawn(Room r)
    {
        currGameSystem =  GameObject.Instantiate(gameSystem);
        SceneManager.MoveGameObjectToScene(currGameSystem.gameObject, gameObject.scene);
        currGameSystem.Init(r);
        currGameSystem.NetworkObject.Spawn(true);
    }
}

