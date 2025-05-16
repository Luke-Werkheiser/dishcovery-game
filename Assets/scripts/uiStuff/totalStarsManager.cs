using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class totalStarsManager : MonoBehaviour
{
    public Image[] stars;
    
    public float currentScore = 5;
    public float tempScore;
    int roundedScore;
    public float lerpSpeed;

    public float holder;

    public bool debugDoScore = false;

    public TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!debugDoScore)
        currentScore=OrderManager.Instance.AverageScore/2;
        if(tempScore != currentScore){
            tempScore=Mathf.MoveTowards(tempScore, currentScore, Time.deltaTime/lerpSpeed);
        }
        float scoreT = Mathf.Round(tempScore * 10.0f) * 0.1f;
        if(scoreT>5) scoreT=5;
        scoreText.text = $"{scoreT}/5.0";
        updateFill();
    }

    public void updateFill(){
        updateScore();
        for(int i = 0; i<stars.Length; i++){
            holder=tempScore-1f;
                
            stars[i].fillAmount=((tempScore)-(i));
                
            
        }
    }
    void updateScore(){
        roundedScore = (int)Mathf.Ceil(tempScore);

    }

    
}
