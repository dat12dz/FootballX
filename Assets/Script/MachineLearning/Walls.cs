using Assets.Utlis;
using Unity.MLAgents;
using UnityEngine;

public class Walls : MonoBehaviour
{
    [SerializeField] LayerMask layer = 12;

    void Start()
    {
    }
/*    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "player")
        {
            Logging.Log("Player go out off the map");
            TrainingAgent a = collision.gameObject.GetComponent<TrainingAgent>();
            a.SetReward(-10);
            a.EndEpisode();
            Debug.Log(a.GetCumulativeReward());
        }

    }*/
    // Update is called once per frame
    void Update()
    {

    }
}
