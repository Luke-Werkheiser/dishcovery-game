using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "FoodObj", menuName = "ScriptableObjects/FoodObjs", order = 1)]
public class foodInfoObj : ScriptableObject
{
    public float foodItemHeight;
    public string foodName = "";
    public float cheeseValue = 0f;
    public float meatValue = 0f;
    public float birdMeatValue =0f;
    public float seafoodValue = 0f;
    public float cakeValue = 0f;
    public float breadValue = 0f;
    public float sauceValue = 0;
    public float drinkValue = 0;
    public float desertValue = 0f;
    public float fruitValue = 0f;
    public float vegitableValue;
    public float starchValue = 0f;

    [Range(0.0f, 1.0f)]
    public float disassembleValue;


    public enum foodParts{
        carrot,
        beef,
        starch,
        chicken,
        cheese,
        apple,
        bread,
        broccoli,
        lettuce,
        tomato,
        rice,
        pasta,
        milk,
        oil,
        egg,
        pea,
        fish,
        tortilla,
        turkey,
        chocolate,
        water
        
        //ect
    }

    public foodParts[] ingredients;

    public bool canBeDisassembled = true;

    public GameObject prop;





}
