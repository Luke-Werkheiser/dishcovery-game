using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class orderProcessing : MonoBehaviour
{
    public foodInfoObj foodOrder;
    public foodInfoObj.foodParts[][] ingridients; // Each array is a group of ingredients from a food item
    public bool useDisassembly = true;
    public Transform spawnPoint;
    public int lastScore;

    public void fillOrder(List<foodInfoObj.foodParts[]> foodItems)
    {
        ingridients = foodItems.ToArray();
        lastScore = EvaluateOrder();
        Debug.Log("Order Score: " + lastScore);
        if(foodOrder.prop !=null) Instantiate(foodOrder.prop, spawnPoint.position, quaternion.identity);
    }

    private int EvaluateOrder()
    {
        // Count required ingredients
        Dictionary<foodInfoObj.foodParts, int> required = CountIngredients(foodOrder.ingredients);

        // If disassembly is enabled, remove food items that contribute nothing
        List<foodInfoObj.foodParts[]> filtered = new List<foodInfoObj.foodParts[]>();

        foreach (var item in ingridients)
        {
            bool contributes = false;
            foreach (var ing in item)
            {
                if (required.ContainsKey(ing))
                {
                    contributes = true;
                    break;
                }
            }

            if (contributes || !useDisassembly)
            {
                filtered.Add(item);
            }
        }

        // Count supplied ingredients
        Dictionary<foodInfoObj.foodParts, int> supplied = new Dictionary<foodInfoObj.foodParts, int>();

        foreach (var item in filtered)
        {
            foreach (var ing in item)
            {
                if (!supplied.ContainsKey(ing))
                    supplied[ing] = 0;

                supplied[ing]++;
            }
        }

        // Compare and log results
        int totalRequired = 0;
        int totalMatched = 0;
        int totalExtra = 0;

        Debug.Log("=== ORDER ANALYSIS ===");

        foreach (var kvp in required)
        {
            foodInfoObj.foodParts ingredient = kvp.Key;
            int needed = kvp.Value;
            totalRequired += needed;

            supplied.TryGetValue(ingredient, out int suppliedCount);

            if (suppliedCount == needed)
            {
                Debug.Log($"✅ Matched: {ingredient} x{needed}");
                totalMatched += needed;
            }
            else if (suppliedCount < needed)
            {
                Debug.Log($"🔺 Too little: {ingredient} (Needed {needed}, Got {suppliedCount})");
                totalMatched += suppliedCount;
            }
            else
            {
                int matched = needed;
                int extra = suppliedCount - needed;
                Debug.Log($"✅ Matched: {ingredient} x{matched}");
                Debug.Log($"❌ Too much: {ingredient} (+{extra})");
                totalMatched += matched;
                totalExtra += extra;
            }
        }

        foreach (var kvp in supplied)
        {
            if (!required.ContainsKey(kvp.Key))
            {
                Debug.Log($"❌ Unneeded: {kvp.Key} x{kvp.Value}");
                totalExtra += kvp.Value;
            }
        }

        Debug.Log("======================");

        if (totalRequired == 0) return 1;

        float matchRatio = (float)totalMatched / totalRequired;
        int score = Mathf.RoundToInt(matchRatio * 10f);

        score -= totalExtra;
        return Mathf.Clamp(score, 1, 10);
    }

    private Dictionary<foodInfoObj.foodParts, int> CountIngredients(foodInfoObj.foodParts[] ingredients)
    {
        Dictionary<foodInfoObj.foodParts, int> count = new Dictionary<foodInfoObj.foodParts, int>();

        foreach (var ing in ingredients)
        {
            if (!count.ContainsKey(ing))
                count[ing] = 0;

            count[ing]++;
        }

        return count;
    }
}
