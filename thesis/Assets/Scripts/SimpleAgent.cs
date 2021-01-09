using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SimpleAgent : Agent
{
    public float speed ;
    public float rotationSpeed = 180f;
    public bool useFoodMap = true;
    public AgentKnowledge agentKnowledge;
    public int capacity;
    public bool rewardsForAllAgents = true;
    
    private int _harvestedFood;
    private Rigidbody _rb;

    public Arena arena;

    public override void OnActionReceived(float[] vectorAction)
    {

        if (transform.position.y < -10)
        {
            transform.position = new Vector3(transform.position.x, 4, transform.position.z);
        }

        var forwardAmount = vectorAction[0] > 0 ? 1 : 0;
        var turnAmount = 0f;
        
        switch (vectorAction[1])
        {
            case 1:
                turnAmount = -1f;
                break;
            case 2:
                turnAmount = 1f;
                break;
        }

        _rb.MovePosition(transform.position + transform.forward * (forwardAmount * speed * Time.fixedDeltaTime));
        transform.Rotate(transform.up * (turnAmount * rotationSpeed * Time.fixedDeltaTime));

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
        agentKnowledge.observedFoods = new HashSet<GameObject>();
        _harvestedFood = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useFoodMap)
        {
            foreach (var vector in agentKnowledge.getObservedFoodsPositions())
            {
                sensor.AddObservation(vector);
            }
        }
        sensor.AddObservation(capacity > _harvestedFood);
     
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (capacity <= _harvestedFood) return;
        
        if (collision.transform.CompareTag("food"))
        {
            _harvestedFood++;
            EatFood(collision.gameObject);
        }
        if (collision.transform.CompareTag("badFood"))
        {
            _harvestedFood++;
            EatBadFood(collision.gameObject);
        }
    }

    private void EatFood(GameObject foodObject)
    {
        if (rewardsForAllAgents)
        {
            var agents = arena.simpleAgents;
            foreach (var agent in agents)
            {
                agent.AddReward(1f);
            }
        }
        else
        {
            this.AddReward(1f*arena.agents.Length);
        }
        
        arena.RemoveSpecificFood(foodObject, 1);
        
        if (arena.RemainingFood <= 0)
        {
            foreach (var agent in arena.simpleAgents)
            {
                agent.EndEpisode();
            }
        }
    }

    private void EatBadFood(GameObject foodObject)
    {
        if (rewardsForAllAgents)
        {
            var agents = arena.simpleAgents;
            foreach (var agent in agents)
            {
                agent.AddReward(-0.5f);
            }
        }
        else
        {
            this.AddReward(-0.5f*arena.agents.Length);
        }
        

        arena.RemoveSpecificFood(foodObject, -1);
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
    }

    private void updateObservations()
    {
        var angle = -90;

        Vector3 noAngle = this.transform.forward;
        Quaternion spreadAngle = Quaternion.AngleAxis(angle, new Vector3(0, 1, 0));
        Vector3 direction = spreadAngle * noAngle;
        for (var i = 0; i < 21; i++)
        {
            angle += 180 / 21;
            spreadAngle = Quaternion.AngleAxis(angle, new Vector3(0, 1, 0));
            direction = spreadAngle * noAngle;
            
            if (Physics.Raycast(this.transform.position, direction, out var hit, 20))
            {

                if (hit.collider.gameObject.CompareTag("food") && agentKnowledge.observedFoods.Count < 5)
                {
                    agentKnowledge.observedFoods.Add(hit.collider.gameObject);
                }
            }
            Debug.DrawRay(transform.position, direction, Color.green);

        }
    }

    public override void Initialize()
    {
        _rb = GetComponent<Rigidbody>();
        agentKnowledge.m_Recorder = Academy.Instance.StatsRecorder;
    }
}
