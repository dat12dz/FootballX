using Assets.Utlis;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using System;
public class TrainingAgent : Agent
{
    [SerializeField] Transform ball;
    [SerializeField] CharacterController controller;
   [SerializeField]  Move moveHelper;
    [SerializeField] Player player;
  
    Vector3 startingPoint;
    public override void OnEpisodeBegin()
    {
        if (startingPoint == default)
        {
            player.TogglePoolObj(true);
            startingPoint = transform.position;

        }
        transform.position = startingPoint;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(ball.transform.position);
    }
    float oldDis;
    float moveX,moveZ;
    public override void OnActionReceived(ActionBuffers actions)
    {
         moveX = actions.ContinuousActions[0];
         moveZ = actions.ContinuousActions[1];
        moveHelper.MovePlayer_ManuallySpeed(new Vector3(moveX, 0, moveZ));
        var distancec = Vector3.Distance(transform.position, ball.position);
        if (distancec > oldDis)
        {
            AddReward(-1);
            
        }
        else
        {
            AddReward(1);
        }
        oldDis = distancec;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "traningwall")
        {
            Logging.Log("Player go out off the map");          
            SetReward(-10);
            Debug.Log(GetCumulativeReward());
            EndEpisode();
        }

    }
    [ContextMenu("end episode")]
    void EndEp()
    {
        EndEpisode();
    }
}
