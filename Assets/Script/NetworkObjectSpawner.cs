using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkObjectSpawner : MonoBehaviour
{
  [SerializeField]  NetworkObject[] NeedSpawning_;

    private void Start()
    {

    }
    [ContextMenu("SpawnAllGameObject")]
    public void Spawn(params object[] spawn)
    {
        for (int i = 0; i < NeedSpawning_.Length; i++)
        {
            NetworkObject NeedSpawning = NeedSpawning_[i];
            var newNetobj = Instantiate(NeedSpawning);
            newNetobj.Spawn();
        }
    }
}
