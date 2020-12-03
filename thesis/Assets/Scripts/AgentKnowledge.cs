using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using UnityEngine;

public class AgentKnowledge : MonoBehaviour
{
    public HashSet<GameObject> observedFoods = new HashSet<GameObject>();
    public int sizeOfMemory = 10;
    
    public StatsRecorder m_Recorder;
    
    public IEnumerable<Vector2> getObservedFoodsPositions()
    {
        GameObject toRemove = null;
        List<Vector2> foodPositions = new List<Vector2> {};
        foreach (var observedFood in observedFoods)
        {
            if (observedFood != null)
            {
                foodPositions.Add(new Vector2(observedFood.transform.localPosition.x, observedFood.transform.localPosition.z));
            }
            else
            {
                toRemove = observedFood;
            }
        }

        observedFoods.Remove(toRemove);

        while(foodPositions.Count < sizeOfMemory/2){
            foodPositions.Add(Vector2.zero); 
        }
        return foodPositions;
    }
    
}
