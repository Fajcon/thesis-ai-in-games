using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Arena : MonoBehaviour
{
    public TextMeshPro cumulativeRewardText;
    public GameObject food;
    public GameObject badFood;
    public GameObject[] agents;
    public SimpleAgent[] simpleAgents;
    [NonSerialized] public int RemainingFood;
    public int numberOfFood = 20;
    public int numberOfBadFood = 5;

    private List<GameObject> _foodList;
    private List<GameObject> _badFoodList;
    private const int Range = 60;
    
    /**
     * type == -1 for bad food and type == 1 for normal
     */
    public void RemoveSpecificFood(GameObject foodObject, int type)
    {
        switch (type)
        {
            case -1:
                _badFoodList.Remove(foodObject);
                break;
            case 1:
                _foodList.Remove(foodObject);
                RemainingFood--;
                break;
        }
        Destroy(foodObject);
    }

    private void PlaceAgents()
    {
        foreach (var agent in agents)
        {
            var rigidbody = agent.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            var position = new Vector3(Random.Range(-Range, Range), 4,
                                           Random.Range(-Range, Range)) + transform.position;
            var rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            agent.transform.SetPositionAndRotation(position, rotation);

        }
    }

    private void RemoveAllFood()
    {
        if (_foodList != null)
        {
            foreach (var t in _foodList)
            {
                Destroy(t);
            }
        }
        _foodList = new List<GameObject>();
        
        if (_badFoodList != null)
        {
            foreach (var t in _badFoodList)
            {
                Destroy(t);
            }
        }
        _badFoodList = new List<GameObject>();
    }

    private void SpawnFood(int count, GameObject type, ICollection<GameObject> list)
    {
        for (var i = 0; i < count; i++)
        {
            var foodObject = Instantiate(type, new Vector3(Random.Range(-Range, Range), 1f, 
                    Random.Range(-Range, Range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            
            foodObject.transform.SetParent(transform);
            list.Add(foodObject);
        }
        RemainingFood = _foodList.Count;
    }

    public void ResetArea()
    {
        RemoveAllFood();
        SpawnFood(numberOfFood, food, _foodList);
        SpawnFood(numberOfBadFood, badFood, _badFoodList);
        PlaceAgents();
    }
}
