using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TrainingAgent : Agent
{
    [SerializeField] Transform ball;
    [SerializeField] CharacterController controller;
   [SerializeField]  Move moveHelper;
    [SerializeField] Player player;
    int e = 0;
    public override void OnEpisodeBegin()
    {
        moveHelper.enabled = true;
        controller.enabled = true;
        player.TogglePoolObj(true);
        base.OnEpisodeBegin();
    }
    private void Start()
    {
    
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        var moveX = actions.ContinuousActions[0];
        var moveZ = actions.ContinuousActions[1];
      //  moveHelper.(new Vector3(moveX, 0, moveZ));

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
    }
}
