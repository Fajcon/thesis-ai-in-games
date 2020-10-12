using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SimpleAgent : Agent
{
    public float speed = 5f;
    public float rotationSpeed = 180f;
    public bool useFoodMap = true;
    public int numberOfAgents = 7;
    public AgentKnowledge agentKnowledge;
    public int capacity;
    private int harvestedFood = 0;
    
    private Vector3 StartingPosition;
    private Rigidbody Rb;

    public Arena arena;
    
    public override void Initialize()
    {
        Rb = GetComponent<Rigidbody>();
    }
    
    public override void OnActionReceived(float[] vectorAction)
    {
        // Convert the first action to forward movement
        var forwardAmount = vectorAction[0];

        // Convert the second action to turning left or right
        var turnAmount = 0f;
        if (vectorAction[1] == 1)
        {
            turnAmount = -1f;
        }
        else if (vectorAction[1] == 2)
        {
            turnAmount = 1f;
        }

        // Apply movement
        Rb.MovePosition(transform.position + transform.forward * forwardAmount * speed * Time.fixedDeltaTime);
        transform.Rotate(transform.up * turnAmount * rotationSpeed * Time.fixedDeltaTime);

        // Apply a tiny negative reward every step to encourage action
        if (MaxStep > 0) AddReward(-1f / MaxStep);
    }
    
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetKey(KeyCode.W) ? 1.0f : 0.0f;
        actionsOut[1] = Input.GetKey(KeyCode.A) ? 1.0f : Input.GetKey(KeyCode.D) ? 2.0f : 0.0f; 
    }
    
    public override void OnEpisodeBegin() 
    {
        arena.ResetArea();
        harvestedFood = 0;
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        if (useFoodMap)
        { 
            sensor.AddObservation(agentKnowledge.getObservedFoodsPositions());
        }
        sensor.AddObservation(capacity > harvestedFood);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (capacity <= harvestedFood) return;

        harvestedFood++;
        if (collision.transform.CompareTag("food"))
        {
            EatFood(collision.gameObject);
        }

        if (collision.transform.CompareTag("badFood"))
        {
            EatBadFood(collision.gameObject);
        }
    }

    private void EatFood(GameObject foodObject)
    {
        var agents = arena.agents;;
        foreach (var agent in agents)
        {
            agent.GetComponent<SimpleAgent>().PositiveEvent();
        }
        arena.RemoveSpecificFood(foodObject, 1);
        if (arena.remainingFood <= 0)
        {
            EndEpisode();
        }
    }
    private void EatBadFood(GameObject foodObject)
    {
        var agents = arena.agents;
        foreach (var agent in agents)
        {
            agent.GetComponent<SimpleAgent>().NegativeEvent();
        }
        arena.RemoveSpecificFood(foodObject, -1);
        if (arena.remainingFood <= 0)
        {
            EndEpisode();
        }
    }

    private void PositiveEvent()
    {
        AddReward(1/numberOfAgents); 
    }

    private void NegativeEvent()
    {
        AddReward(-1/numberOfAgents*2); 
    }

    private void FixedUpdate()
    {
        if (StepCount % 5 == 0)
        {
            RequestDecision();
        }
        else
        {
            RequestAction();
        }

        updateObservations();
        arena.cumulativeRewardText.text = GetCumulativeReward().ToString("0.00");

    }
    
    private void updateObservations()
    {
        Vector3 direction = transform.forward;
        direction.y -= 90;
        for (var i = 0; i < 21; i++)
        {
            direction.y += 180 / 21;
            if (Physics.Raycast(transform.position, direction, out var hit, 20f))
            {
                if (hit.collider.gameObject.CompareTag("food"))
                {
                    agentKnowledge.observedFoods.Add(hit.collider.gameObject);
                }
            }
        }

    }
}
