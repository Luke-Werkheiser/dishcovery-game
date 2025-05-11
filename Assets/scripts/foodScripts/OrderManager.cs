using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [SerializeField] private List<foodInfoObj> allFoodItems = new List<foodInfoObj>();
    private float totalScore = 0f;
    private int totalOrdersCompleted = 0;
    public float AverageScore { get; private set; } = 0f;

    [Header("Order Size Settings")]
    [SerializeField] private int maxOrderSize = 5;
    [SerializeField] private float baseSingleOrderChance = 0.7f;
    [SerializeField] private float additionalItemPenalty = 0.3f;

    void Awake()
    {
        Instance=this;
    }
    void Start()
    {
        GenerateNewOrder();
    }
    public void AddCompletedOrder(int score)
    {
        totalScore += score;
        totalOrdersCompleted++;
        AverageScore = totalScore / totalOrdersCompleted;
        GenerateNewOrder();
    }

    public void GenerateNewOrder()
    {
        int orderSize = CalculateOrderSize();
        List<foodInfoObj> order = new List<foodInfoObj>();
        
        for (int i = 0; i < orderSize; i++)
        {
            order.Add(SelectFoodByRarity());
        }

        FindObjectOfType<orderProcessing>().SetNewOrder(order);
    }
    private int CalculateOrderSize()
    {
        float roll = Random.value;
        float cumulativeProbability = baseSingleOrderChance;
        int size = 1;

        for (; size < maxOrderSize; size++)
        {
            if (roll < cumulativeProbability)
            {
                break;
            }
            cumulativeProbability += baseSingleOrderChance * Mathf.Pow(additionalItemPenalty, size);
        }

        return Mathf.Min(size, maxOrderSize);
    }

    private foodInfoObj SelectFoodByRarity()
    {
        // Calculate total rarity weight
        float totalRarityWeight = 0f;
        foreach (var food in allFoodItems)
        {
            totalRarityWeight += food.rarity;
        }

        // Select random food based on rarity
        float randomValue = Random.Range(0f, totalRarityWeight);
        float currentWeight = 0f;

        foreach (var food in allFoodItems)
        {
            currentWeight += food.rarity;
            if (randomValue <= currentWeight)
            {
                return food;
            }
        }

        return allFoodItems[0]; // Fallback
    }

    public void RegisterFoodItem(foodInfoObj foodItem)
    {
        if (!allFoodItems.Contains(foodItem))
        {
            allFoodItems.Add(foodItem);
        }
    }

    public void SetFoodItems(List<foodInfoObj> foodItems)
    {
        allFoodItems = new List<foodInfoObj>(foodItems);
    }
}