using Assets.Script.NetCode;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script.Player.PlayerModle
{
    class GameSystemSpawner : MonoBehaviour
    {
        [SerializeField] GameSystem Prefab;
        public GameSystem InGameGameSystem;

        public void Awake()
        {
            
            if (NetworkManager.Singleton.IsServer)
            {               
                var IngameObj = Instantiate(Prefab);
                IngameObj.NetworkObject.Spawn(true);
                SceneHelper.cache = IngameObj;
            }
        }
    }
}
