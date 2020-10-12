using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Arena : MonoBehaviour
{

    public GameObject food;
    public GameObject badFood;
    private List<GameObject> foodList;
    private List<GameObject> badFoodList;
    public GameObject[] agents;
    public int remainingFood;
    public int numberOfFood = 20;
    public int numberOfBadFood = 5;
    public TextMeshPro cumulativeRewardText;

    private int range = 70;


    public void RemoveSpecificFood(GameObject foodObject, int type)
    {
        switch (type)
        {
            case -1:
                badFoodList.Remove(foodObject);
                break;
            case 1:
                foodList.Remove(foodObject);
                remainingFood--;
                break;
        }

        Destroy(foodObject);
    }
    
    private void Start()
    {
        ResetArea();
    }

    public void ResetArea()
    {
        RemoveAllFood();
        SpawnFood(numberOfFood, food, foodList);
        SpawnFood(numberOfBadFood, badFood, badFoodList);
        PlaceAgents();
    }

    private void PlaceAgents()
    {
        foreach (GameObject agent in agents)
        {
            agent.transform.position = new Vector3(Random.Range(-range, range)-43.61f, 4f,
                                           Random.Range(-range, range)-42.9f) + transform.position;
            agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            
        }
    }

    private void RemoveAllFood()
    {
        if (foodList != null)
        {
            for (int i = 0; i < foodList.Count; i++)
            {
                if (foodList[i] != null)
                {
                    Destroy(foodList[i]);
                }
            }
        }
        foodList = new List<GameObject>();
        
        if (badFoodList != null)
        {
            for (int i = 0; i < badFoodList.Count; i++)
            {
                if (badFoodList[i] != null)
                {
                    Destroy(badFoodList[i]);
                }
            }
        }
        badFoodList = new List<GameObject>();
    }

    private void SpawnFood(int count, GameObject type, List<GameObject> list)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject foodObject = Instantiate(type, new Vector3(Random.Range(-range, range)-43.61f, 1f, 
                    Random.Range(-range, range)-42.9f) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            
            // Set the fish's parent to this area's transform
            foodObject.transform.SetParent(transform);

            // Keep track of the fish
            list.Add(foodObject);
        }

        remainingFood = foodList.Count;
    }
}
