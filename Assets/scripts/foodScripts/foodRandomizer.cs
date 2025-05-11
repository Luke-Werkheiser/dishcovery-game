using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class foodItemSpawn{
    public GameObject foodItem;
    public spawnType spawn = spawnType.ground;
    public float rarity = 10f;
}
public class foodRandomizer : MonoBehaviour
{
    public static foodRandomizer fRandom {get; private set;}
    public foodItemSpawn[] foodStuffs;

    void Awake()
    {
        fRandom=this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
public foodItemSpawn getRandomFood(spawnType[] types)
{
    List<foodItemSpawn> validFoods = new List<foodItemSpawn>();

    bool includesAll = System.Array.Exists(types, t => t == spawnType.all);

    foreach (var food in foodStuffs)
    {
        if (includesAll || System.Array.Exists(types, t => t == food.spawn))
        {
            validFoods.Add(food);
        }
    }

    if (validFoods.Count == 0)
        return null;

    // Rarity-based selection
    float totalRarity = 0f;
    foreach (var food in validFoods)
        totalRarity += food.rarity;

    float roll = Random.Range(0f, totalRarity);
    float cumulative = 0f;

    foreach (var food in validFoods)
    {
        cumulative += food.rarity;
        if (roll <= cumulative)
            return food;
    }

    return validFoods[Random.Range(0, validFoods.Count)];
}    public bool doesHaveFood(float chance){
        float num = Random.Range(0, 100);
        return num<chance;
    }
}
    public enum spawnType{
        all,
        ground,
        tree,
        underground,
        bush
    }
