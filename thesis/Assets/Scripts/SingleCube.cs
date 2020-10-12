using UnityEngine;

public class SingleCube: MonoBehaviour
{
    public ComplexAgent agent;
    public int capacity;
    public int harvestedFood = 0;
    private void OnCollisionEnter(Collision collision)
    {
        if (capacity <= harvestedFood) return;

        harvestedFood++;
        if (collision.transform.CompareTag("food"))
        {
            AgentController.EatFood(agent, collision.gameObject, agent.arena);
            agent.agentKnowledge.observedFoods.Remove(collision.gameObject);
        }
        if (collision.transform.CompareTag("badFood"))
        {
            AgentController.EatBadFood(agent, collision.gameObject, agent.arena);
            agent.agentKnowledge.observedFoods.Remove(collision.gameObject);
        }
    }

    private void updateObservations()
    {
        Vector3 direction = transform.forward;
        direction.y -= 90;
        for (var i = 0; i < 21; i++)
        {
            direction.y += 180 / 21;
            if (Physics.Raycast(agent.transform.position, direction, out var hit, 20f))
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
        updateObservations();
    }
}