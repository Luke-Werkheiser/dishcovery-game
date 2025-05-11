using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class UIRecipeManager : MonoBehaviour
{
    [Header("References")]
    public TMP_Text recipeText;
    
    [Header("Colors")]
    public Color defaultColor = Color.white;
    public Color completedColor = Color.green;
    public Color extraColor = Color.yellow;
    public Color missingColor = Color.red;
    public Color foodNameColor = new Color(0.6f, 0.8f, 1f); // Light blue
    public Color foodCountColor = new Color(0.8f, 0.8f, 0.8f); // Light gray

    private Dictionary<foodInfoObj.foodParts, int> requiredIngredients = new Dictionary<foodInfoObj.foodParts, int>();
    private Dictionary<foodInfoObj.foodParts, int> playerIngredients = new Dictionary<foodInfoObj.foodParts, int>();
    private List<foodInfoObj> currentOrderItems = new List<foodInfoObj>();

    private void OnEnable()
    {
        orderProcessing.OnOrderChanged += UpdateOrderRequirements;
        pickupscript.OnInventoryChanged += UpdatePlayerIngredients;
    }

    private void OnDisable()
    {
        orderProcessing.OnOrderChanged -= UpdateOrderRequirements;
        pickupscript.OnInventoryChanged -= UpdatePlayerIngredients;
    }

    public void UpdateOrderRequirements(List<foodInfoObj> currentOrder)
    {
        requiredIngredients.Clear();
        currentOrderItems = new List<foodInfoObj>(currentOrder);

        foreach (var food in currentOrder)
        {
            foreach (var ingredient in food.ingredients)
            {
                if (requiredIngredients.ContainsKey(ingredient))
                    requiredIngredients[ingredient]++;
                else
                    requiredIngredients[ingredient] = 1;
            }
        }
        
        UpdateDisplay();
    }

    public void UpdatePlayerIngredients(List<foodScript> playerFoodStack)
    {
        playerIngredients.Clear();
        
        foreach (var food in playerFoodStack)
        {
            foreach (var ingredient in food.ingredients)
            {
                if (playerIngredients.ContainsKey(ingredient))
                    playerIngredients[ingredient]++;
                else
                    playerIngredients[ingredient] = 1;
            }
        }
        
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        StringBuilder sb = new StringBuilder();
        
        // Display order header with food names
        sb.AppendLine($"<b><color=#{ColorUtility.ToHtmlStringRGB(foodNameColor)}>ORDER:</color></b>");
        foreach (var food in currentOrderItems)
        {
            sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(foodNameColor)}>• {food.foodName}</color> " +
                          $"<color=#{ColorUtility.ToHtmlStringRGB(foodCountColor)}>[{food.ingredients.Length} items]</color>");
        }
        sb.AppendLine();

        // Required ingredients section
        sb.AppendLine($"<b><color=#{ColorUtility.ToHtmlStringRGB(defaultColor)}>NEEDED INGREDIENTS:</color></b>");
        if (requiredIngredients.Count == 0)
        {
            sb.AppendLine("<i>No requirements</i>");
        }
        else
        {
            foreach (var kvp in requiredIngredients)
            {
                int needed = kvp.Value;
                playerIngredients.TryGetValue(kvp.Key, out int have);

                string status;
                Color color;
                
                if (have >= needed)
                {
                    status = $"✓ ({have}/{needed})";
                    color = completedColor;
                }
                else if (have > 0)
                {
                    status = $"{have}/{needed}";
                    color = Color.Lerp(missingColor, completedColor, (float)have/needed);
                }
                else
                {
                    status = $"0/{needed}";
                    color = missingColor;
                }

                sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(color)}> - {kvp.Key}: {status}</color>");
            }
        }
        sb.AppendLine();

        // Extra ingredients section
        sb.AppendLine($"<b><color=#{ColorUtility.ToHtmlStringRGB(defaultColor)}>EXTRA ITEMS:</color></b>");
        bool hasExtras = false;
        
        foreach (var kvp in playerIngredients)
        {
            if (!requiredIngredients.ContainsKey(kvp.Key))
            {
                sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(extraColor)}> - {kvp.Key}: {kvp.Value} (unneeded)</color>");
                hasExtras = true;
            }
            else if (kvp.Value > requiredIngredients[kvp.Key])
            {
                int extra = kvp.Value - requiredIngredients[kvp.Key];
                sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(extraColor)}> - {kvp.Key}: +{extra} extra</color>");
                hasExtras = true;
            }
        }

        if (!hasExtras)
        {
            sb.AppendLine("<i>No extra ingredients</i>");
        }

        recipeText.text = sb.ToString();
    }
}