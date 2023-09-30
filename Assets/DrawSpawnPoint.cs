using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSpawnPoint : MonoBehaviour
{
   [SerializeField] Color spawnColor;
    private void OnDrawGizmos()
    {
        Gizmos.color = spawnColor;
        Gizmos.DrawSphere(gameObject.transform.position, 1);
    }
}
