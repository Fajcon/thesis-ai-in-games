using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentKnowledge : MonoBehaviour
{
    public HashSet<GameObject> observedFoods = new HashSet<GameObject>();

    public IEnumerable<float> getObservedFoodsPositions()
    {
        GameObject toRemove = null;
        List<float> foodPositions = new List<float> {};
        foreach (var observedFood in observedFoods)
        {
            if (observedFood != null)
            {
                foodPositions.Add(observedFood.transform.position.x);
                foodPositions.Add(observedFood.transform.position.z);
            }
            else
            {
                toRemove = observedFood;
            }
        }

        observedFoods.Remove(toRemove);

        for (int i = observedFoods.Count+1; i <= 20; i++)
        {
            foodPositions.Add(new float());
            foodPositions.Add(new float());
        }

        return foodPositions;
    }
}
