using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodProcessing : MonoBehaviour
{
    public foodInfoObj foodOrder;
    public foodInfoObj[] stackFoodItems;
    public bool useDisassembly = true;
    
    // Dictionary to store the final processed values
    private Dictionary<string, float> processedValues = new Dictionary<string, float>();
    
    // Scores for each category (0-10)
    private Dictionary<string, float> categoryScores = new Dictionary<string, float>();
    
    // Overall score
    private float overallScore = 0f;

    public void SubmitOrder(List<foodInfoObj> inputItems)
    {
        stackFoodItems = inputItems.ToArray();
        ProcessOrder();
        CalculateScores();
        DisplayResults();
    }

    private void ProcessOrder()
    {
        // Initialize dictionaries with all possible categories
        ResetDictionaries();
        
        // First pass: Sum all raw values
        foreach (var foodItem in stackFoodItems)
        {
            AddToDictionary(processedValues, "cheeseValue", foodItem.cheeseValue);
            AddToDictionary(processedValues, "meatValue", foodItem.meatValue);
            AddToDictionary(processedValues, "birdMeatValue", foodItem.birdMeatValue);
            AddToDictionary(processedValues, "seafoodValue", foodItem.seafoodValue);
            AddToDictionary(processedValues, "cakeValue", foodItem.cakeValue);
            AddToDictionary(processedValues, "breadValue", foodItem.breadValue);
            AddToDictionary(processedValues, "sauceValue", foodItem.sauceValue);
            AddToDictionary(processedValues, "drinkValue", foodItem.drinkValue);
            AddToDictionary(processedValues, "desertValue", foodItem.desertValue);
            AddToDictionary(processedValues, "fruitValue", foodItem.fruitValue);
            AddToDictionary(processedValues, "vegitableValue", foodItem.vegitableValue);
        }
        
        // Second pass: Apply disassembly if enabled
        if (useDisassembly)
        {
            ApplyDisassembly();
        }
    }

    private void ApplyDisassembly()
    {
        // For each category, reduce excess based on disassembly values
        foreach (var category in new List<string>(processedValues.Keys))
        {
            float orderValue = GetOrderValue(category);
            float currentValue = processedValues[category];

if (currentValue > orderValue)            {
                float excess = currentValue - orderValue;
                float totalDisassemblyPotential = 0f;
                List<foodInfoObj> contributingItems = new List<foodInfoObj>();

                foreach (var foodItem in stackFoodItems)
                {
                    float itemValue = GetFoodItemValue(foodItem, category);
                    if (itemValue > 0)
                    {
                        contributingItems.Add(foodItem);
                        totalDisassemblyPotential += foodItem.disassembleValue * itemValue;
                    }
                }

                float disassemblyUsed = Mathf.Min(totalDisassemblyPotential, excess);

                foreach (var foodItem in contributingItems)
                {
                    float itemValue = GetFoodItemValue(foodItem, category);
                    float itemPotential = foodItem.disassembleValue * itemValue;
                    float ratio = itemPotential / totalDisassemblyPotential;
                    float disassemblyAmount = ratio * disassemblyUsed;
                    float reductionRatio = disassemblyAmount / itemValue;

                    // Reduce from all categories
                    foreach (var key in new List<string>(processedValues.Keys))
                    {
                        float v = GetFoodItemValue(foodItem, key);
                        processedValues[key] -= v * reductionRatio;
                    }
                }

                // Clamp the target category to order value (or zero)
                processedValues[category] = Mathf.Max(orderValue, processedValues[category]);
            }
        }
    }

    private void CalculateScores()
    {
        overallScore = 0f;
        int scoredCategories = 0;
        
        foreach (var category in processedValues.Keys)
        {
            float orderValue = GetOrderValue(category);
            float submittedValue = processedValues[category];
            
            if (orderValue > 0)
            {
                // This category is relevant for the order
                float score;
                
                if (submittedValue <= 0)
                {
                    score = 0f; // No contribution at all
                }
                else if (submittedValue >= orderValue)
                {
                    // Perfect score for meeting or exceeding requirement
                    score = 10f;
                }
                else
                {
                    // Partial score based on percentage of requirement met
                    score = (submittedValue / orderValue) * 10f;
                }
                
                categoryScores[category] = score;
                overallScore += score;
                scoredCategories++;
            }
            else
            {
                // Category not in order - check if we have unwanted contributions
                if (submittedValue > 0)
                {
                    // Penalize for unwanted ingredients
                    categoryScores[category] = -submittedValue * 2f; // Penalty factor
                    overallScore += categoryScores[category];
                }
                else
                {
                    categoryScores[category] = 0f;
                }
            }
        }
        
        // Calculate average score if we have scored categories
        if (scoredCategories > 0)
        {
            overallScore /= scoredCategories;
            // Clamp to 0-10 range in case of penalties
            overallScore = Mathf.Clamp(overallScore, 0f, 10f);
        }
    }

    private void DisplayResults()
    {
        Debug.Log("=== Order Evaluation ===");
        Debug.Log($"Overall Score: {overallScore:F1}/10");
        
        foreach (var category in categoryScores.Keys)
        {
            float orderValue = GetOrderValue(category);
            if (orderValue > 0 || processedValues[category] > 0)
            {
                Debug.Log($"{category}: " +
                          $"Ordered: {orderValue:F1}, " +
                          $"Submitted: {processedValues[category]:F1}, " +
                          $"Score: {categoryScores[category]:F1}");
            }
        }
    }

    #region Helper Methods
    private void ResetDictionaries()
    {
        processedValues.Clear();
        categoryScores.Clear();
        
        // Initialize all possible categories
        processedValues.Add("cheeseValue", 0f);
        processedValues.Add("meatValue", 0f);
        processedValues.Add("birdMeatValue", 0f);
        processedValues.Add("seafoodValue", 0f);
        processedValues.Add("cakeValue", 0f);
        processedValues.Add("breadValue", 0f);
        processedValues.Add("sauceValue", 0f);
        processedValues.Add("drinkValue", 0f);
        processedValues.Add("desertValue", 0f);
        processedValues.Add("fruitValue", 0f);
        processedValues.Add("vegitableValue", 0f);
        
        // Initialize score dictionary similarly
        foreach (var key in processedValues.Keys)
        {
            categoryScores.Add(key, 0f);
        }
    }

    private void AddToDictionary(Dictionary<string, float> dict, string key, float value)
    {
        if (dict.ContainsKey(key))
        {
            dict[key] += value;
        }
    }

    private float GetOrderValue(string category)
    {
        switch (category)
        {
            case "cheeseValue": return foodOrder.cheeseValue;
            case "meatValue": return foodOrder.meatValue;
            case "birdMeatValue": return foodOrder.birdMeatValue;
            case "seafoodValue": return foodOrder.seafoodValue;
            case "cakeValue": return foodOrder.cakeValue;
            case "breadValue": return foodOrder.breadValue;
            case "sauceValue": return foodOrder.sauceValue;
            case "drinkValue": return foodOrder.drinkValue;
            case "desertValue": return foodOrder.desertValue;
            case "fruitValue": return foodOrder.fruitValue;
            case "vegitableValue": return foodOrder.vegitableValue;
            default: return 0f;
        }
    }

    private float GetFoodItemValue(foodInfoObj item, string category)
    {
        switch (category)
        {
            case "cheeseValue": return item.cheeseValue;
            case "meatValue": return item.meatValue;
            case "birdMeatValue": return item.birdMeatValue;
            case "seafoodValue": return item.seafoodValue;
            case "cakeValue": return item.cakeValue;
            case "breadValue": return item.breadValue;
            case "sauceValue": return item.sauceValue;
            case "drinkValue": return item.drinkValue;
            case "desertValue": return item.desertValue;
            case "fruitValue": return item.fruitValue;
            case "vegitableValue": return item.vegitableValue;
            default: return 0f;
        }
    }
    #endregion
}