using Unity.MLAgents;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    public static void EatFood(Agent agent, GameObject foodObject, Arena arena)
    {
        agent.AddReward(1f);
        arena.RemoveSpecificFood(foodObject, 1);
        if (arena.remainingFood <= 0)
        {
            agent.EndEpisode();
        }
    }
    
    public static void EatBadFood(Agent agent, GameObject foodObject, Arena arena)
    {
        agent.AddReward(-0.5f);
        arena.RemoveSpecificFood(foodObject, -1);
        if (arena.remainingFood <= 0)
        {
            agent.EndEpisode();
        }
    }
}