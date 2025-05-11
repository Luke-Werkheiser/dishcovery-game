using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class orderProcessing : MonoBehaviour
{
    public List<foodInfoObj> currentOrder = new List<foodInfoObj>(); // What the customer wants
    public List<foodInfoObj.foodParts> submittedIngredients = new List<foodInfoObj.foodParts>(); // What player gave
    public bool useDisassembly = true;
    public Transform spawnPoint;
    public int lastScore;
    public static event System.Action<List<foodInfoObj>> OnOrderChanged;
    // Call this to set up a new order (from OrderManager)
    public void SetNewOrder(List<foodInfoObj> order)
    {
        currentOrder = order;
        OnOrderChanged?.Invoke(currentOrder);
        foreach (var food in order)
        {
            if(food.prop != null) 
                Instantiate(food.prop, spawnPoint.position + Vector3.up * food.foodItemHeight, Quaternion.identity);
        }
        Debug.Log($"New order received with {order.Count} items");
    }

    // Player submits their ingredients here
    public void fillOrder(List<foodInfoObj.foodParts> playerIngredients)
    {
        submittedIngredients = playerIngredients;
        lastScore = EvaluateOrder();
        Debug.Log("Order Score: " + lastScore);
        OrderManager.Instance.AddCompletedOrder(lastScore);
    }

    private int EvaluateOrder()
    {
        // Get all required ingredients from current order
        List<foodInfoObj.foodParts> allRequired = new List<foodInfoObj.foodParts>();
        foreach (var food in currentOrder)
        {
            allRequired.AddRange(food.ingredients);
        }

        Dictionary<foodInfoObj.foodParts, int> required = CountIngredients(allRequired.ToArray());
        Dictionary<foodInfoObj.foodParts, int> supplied = CountIngredients(submittedIngredients.ToArray());

        // Compare and calculate score
        int totalRequired = 0;
        int totalMatched = 0;
        int totalExtra = 0;

        Debug.Log("=== ORDER ANALYSIS ===");

        foreach (var kvp in required)
        {
            var ingredient = kvp.Key;
            int needed = kvp.Value;
            totalRequired += needed;

            supplied.TryGetValue(ingredient, out int suppliedCount);

            if (suppliedCount == needed)
            {
                Debug.Log($"✅ Perfect: {ingredient} x{needed}");
                totalMatched += needed;
            }
            else if (suppliedCount < needed)
            {
                Debug.Log($"⚠️ Missing: {ingredient} (Need {needed}, got {suppliedCount})");
                totalMatched += suppliedCount;
            }
            else
            {
                int extra = suppliedCount - needed;
                Debug.Log($"✅ Matched: {ingredient} x{needed}");
                Debug.Log($"❌ Extra: {ingredient} (+{extra})");
                totalMatched += needed;
                totalExtra += extra;
            }
        }

        // Check for unneeded ingredients
        foreach (var kvp in supplied)
        {
            if (!required.ContainsKey(kvp.Key))
            {
                Debug.Log($"❌ Unwanted: {kvp.Key} x{kvp.Value}");
                totalExtra += kvp.Value;
            }
        }

        Debug.Log("======================");

        if (totalRequired == 0) return 0;

        float accuracy = (float)totalMatched / totalRequired;
        int score = Mathf.RoundToInt(accuracy * 10f) - totalExtra;
        return Mathf.Clamp(score, 0, 10);
    }

    private Dictionary<foodInfoObj.foodParts, int> CountIngredients(foodInfoObj.foodParts[] ingredients)
    {
        Dictionary<foodInfoObj.foodParts, int> count = new Dictionary<foodInfoObj.foodParts, int>();
        foreach (var ing in ingredients)
        {
            if (count.ContainsKey(ing)) count[ing]++;
            else count[ing] = 1;
        }
        return count;
    }
}