using Assets.Script.NetCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

namespace Assets.Script.Networking.NetworkRoom
{
    /// <summary>
    /// NetworkBehavior chỉ hiện khi cùng 1 scene
    /// (Người chơi chỉ có thể tương tác,nhìn thấy với gameobject này khi cùng 1 scene)
    /// </summary>
    public class SceneNetworkBehavior : NetworkBehaviour
    {

        GameSystem system;
        public virtual void Awake()
        {
      
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                system = SceneHelper.GetGameSystem(gameObject.scene);
            }
            ShowObjecttoScenePlayer();
        }
        public virtual void Start()
        {
            
        }
        void ShowObjecttoScenePlayer()
        {
            var listPlayer = system.room.playerDict;
            if (listPlayer.Count > 0)
            {
                // Hiện tất cả tầm nhìn cho người chơi
                foreach (var player in listPlayer.Values)
                {
                    NetworkObject.NetworkShow(player.OwnerClientId);
                }
            }
        }
    }
}
