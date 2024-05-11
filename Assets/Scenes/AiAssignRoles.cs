using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiAssignRoles : AiState
{
    NavMeshAgent navAgent;
    Animator animator;
    AiAgent agent;

    List<AiAgent> comrades;
    int patrolCount = 1;
    int assignedPatrol = 0;

    public void Enter(AiAgent agent)
    {
        assignedPatrol = 0;
        // play animation
        // physics overlap sphere checking for same-team tag
        comrades = agent.eyes.ScanForEnemies(!agent.friendly);


        foreach (var comrade in comrades)
        {
            Debug.Log("Found comrade: " + comrade);
            if (patrolCount > assignedPatrol)
            {
                comrade.stateMachine.ChangeState(AiStateId.Patrol);
                Debug.Log("Assigned patrol role to: " + comrade.name);
                break;
            }
            
        }
    }

    public void Exit(AiAgent agent)
    {

    }

    public AiStateId GetId()
    {
        return AiStateId.AssignRoles;
    }

    public void GetNetworkUpdates(AiAgent agent)
    {

    }

    public void Update(AiAgent agent)
    {

    }
}
