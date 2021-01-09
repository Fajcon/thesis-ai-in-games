using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class ComplexAgent : Agent
{
    public float speed = 5f;
    public float scoutSpeed = 5f;
    public float rotationSpeed = 180f;
    public bool useVectorObs = true;

    public SingleCube[] agents;
    public SingleCube[] scouts;
    private Vector3 StartingPosition;
    private Rigidbody Rb;
    public AgentKnowledge agentKnowledge;

    public Arena arena;
    
    public override void Initialize()
    {
        agentKnowledge.m_Recorder = Academy.Instance.StatsRecorder;
    }
    
    public override void OnActionReceived(float[] vectorAction)
    {
        int iterator = 0;
        foreach (var agent in agents)
        {
            Rb = agent.GetComponent<Rigidbody>();

            // Convert the first action to forward movement
            var forwardAmount = vectorAction[iterator];

            // Convert the second action to turning left or right
            var turnAmount = 0f;
            if (vectorAction[iterator + 1] == 1)
            {
                turnAmount = -1f;
            }
            else if (vectorAction[iterator + 1] == 2)
            {
                turnAmount = 1f;
            }

            // Apply movement
            if (scouts.Contains(agent))
            {
                Rb.MovePosition(agent.transform.position + agent.transform.forward * forwardAmount * speed * Time.fixedDeltaTime);
                agent.transform.Rotate(agent.transform.up * turnAmount * rotationSpeed * Time.fixedDeltaTime);
            }
            else
            {
                Rb.MovePosition(agent.transform.position +
                                agent.transform.forward * forwardAmount * speed * Time.fixedDeltaTime);
                agent.transform.Rotate(agent.transform.up * turnAmount * rotationSpeed * Time.fixedDeltaTime);
            }

            // Apply a tiny negative reward every step to encourage action, for each player
            iterator = iterator + 2;
        }
        if (MaxStep > 0)
        {
            AddReward(-1f / MaxStep);
        }
        
        
    }
    
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetKey(KeyCode.W) ? 1.0f : 0.0f;
        actionsOut[1] = Input.GetKey(KeyCode.A) ? 1.0f : Input.GetKey(KeyCode.D) ? 2.0f : 0.0f; 
    }
    
    public override void OnEpisodeBegin() 
    {
        arena.ResetArea();
        foreach (var agent in agents)
        {
            agent.harvestedFood = 0;
        }
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVectorObs)
        {
            foreach (var vector in agentKnowledge.getObservedFoodsPositions())
            {
                sensor.AddObservation(vector);
            }
            foreach (var agent in agents)
            {
                sensor.AddObservation(agent.capacity > agent.harvestedFood);
            }
        }
        foreach (var agent in agents)
        {
            sensor.AddObservation(agent.capacity > agent.harvestedFood);
        }
    }

    private void FixedUpdate()
    {
        // Request a decision every 5 steps. RequestDecision() automatically calls RequestAction(),
        // but for the steps in between, we need to call it explicitly to take action using the results
        // of the previous decision
        if (StepCount % 5 == 0)
        {
            RequestDecision();
        }
        else
        {
            RequestAction();
        }

        // arena.cumulativeRewardText.text = GetCumulativeReward().ToString("0.00");

    }
}
