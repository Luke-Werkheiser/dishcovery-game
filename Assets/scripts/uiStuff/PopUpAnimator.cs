using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PopUpAnimator : MonoBehaviour
{
        public Image[] stars;

    public AnimationCurve opacityCurve;
    public AnimationCurve scaleCurve;
    public AnimationCurve heightCurve;

    private TextMeshProUGUI tmp;
    private float time;
    private Vector3 origin;
    public Transform starholder;


    private void Awake()
    {
        //tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        origin = transform.position;
        time = 0;
    }

    private void Update()
    {
        //tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, opacityCurve.Evaluate(time));
        foreach(Image star in stars){
            star.color = new Color(star.color.r, star.color.g, star.color.b, opacityCurve.Evaluate(time));
        }
        starholder.transform.localScale = Vector3.one * scaleCurve.Evaluate(time);
        transform.position = origin + new Vector3(0, 1 + heightCurve.Evaluate(time), 0);
        time += Time.deltaTime;
    }

    public void ResetAnimation()
    {
        time = 0; // Reset animation progress
        origin = transform.position; // Reset position origin
        starholder.localScale=Vector3.one;
    }

public void setStars(int reviewScore)
{
    // First hide all stars
    foreach (var star in stars)
    {
        star.gameObject.SetActive(false);
    }

    // Calculate how many stars should be shown (each star represents 2 points)
    float starCount = reviewScore / 2f;
    
    // Determine how many full and half stars we need
    int fullStars = Mathf.FloorToInt(starCount);
    bool hasHalfStar = (starCount - fullStars) >= 0.5f;

    // Activate and set fill for each star
    for (int i = 0; i < stars.Length; i++)
    {
        if (i < fullStars)
        {
            // Full star
            stars[i].gameObject.SetActive(true);
            stars[i].fillAmount = 1f;
        }
        else if (i == fullStars && hasHalfStar)
        {
            // Half star
            stars[i].gameObject.SetActive(true);
            stars[i].fillAmount = 0.5f;
        }
        else if (i < starCount)
        {
            // This handles cases where reviewScore is odd (like 5/10 = 2.5 stars)
            stars[i].gameObject.SetActive(true);
            stars[i].fillAmount = 0.5f;
        }
        else
        {
            // Star not needed - leave it inactive
            stars[i].gameObject.SetActive(false);
        }
    }
}
}
