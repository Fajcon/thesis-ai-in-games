using UnityEngine;

public class SingleCube: MonoBehaviour
{
    public ComplexAgent agent;
    public int capacity;
    public int harvestedFood;
    private const int NumberOfRays = 21;
    private const int MaxDistanceOfRay = 21;
    private void OnCollisionEnter(Collision collision)
    {
        if (capacity <= harvestedFood) return;

        if (collision.transform.CompareTag("food"))
        {
            harvestedFood++;
            AgentController.EatFood(agent, collision.gameObject, agent.arena);
            agent.agentKnowledge.observedFoods.Remove(collision.gameObject);
        }
        if (collision.transform.CompareTag("badFood"))
        {
            harvestedFood++;
            AgentController.EatBadFood(agent, collision.gameObject, agent.arena);
            agent.agentKnowledge.observedFoods.Remove(collision.gameObject);
        }
    }

    private void UpdateObservations()
    {
        Vector3 direction = transform.forward;
        direction.y -= 90;
        for (var i = 0; i < NumberOfRays; i++)
        {
            direction.y += 180f / NumberOfRays;
            if (Physics.Raycast(agent.transform.position, direction, out var hit, MaxDistanceOfRay))
            {
                if (hit.collider.gameObject.CompareTag("food"))
                {
                    agent.agentKnowledge.observedFoods.Add(hit.collider.gameObject);
                }
            }
        }

    }
    
    private void Update()
    {
        UpdateObservations();
    }
}