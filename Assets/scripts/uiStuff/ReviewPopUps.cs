using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReviewPopUps : MonoBehaviour
{
    public static ReviewPopUps current;
    public GameObject prefab;
    public float baseSize = 10f;
    public int maxPopups = 20; // Maximum number of active popups
    public Transform starPos;

    private Queue<GameObject> popupPool = new Queue<GameObject>(); // Pool of inactive popups
    private List<GameObject> activePopups = new List<GameObject>(); // List of currently active popups
    public float randomSpawnRange = 1f;

    void Awake()
    {
        current = this;

        // Preload popups into the pool
        for (int i = 0; i < maxPopups; i++)
        {
            GameObject popup = Instantiate(prefab, transform);
            popup.SetActive(false);
            popupPool.Enqueue(popup);
        }
    }

public void CreateDamagePopUp( int score)
{
    Vector3 position = starPos.position;
    GameObject popup;
    if (popupPool.Count > 0)
    {
        popup = popupPool.Dequeue(); // Reuse an inactive popup
    }
    else
    {
        popup = activePopups[0]; // Recycle the oldest popup
        activePopups.RemoveAt(0);
    }

    // Apply slight random offset to prevent overlap
    Vector3 randomOffset = new Vector3(Random.Range(-randomSpawnRange, randomSpawnRange), Random.Range(-randomSpawnRange, randomSpawnRange), Random.Range(-randomSpawnRange, randomSpawnRange));
    popup.transform.position = position + randomOffset;
    
    popup.SetActive(true);

    TextMeshProUGUI tmp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    //tmp.text = createDamageString(damage, isCrit);
    //tmp.color = color;

    // Reset the animation component
    PopUpAnimator animation = popup.GetComponent<PopUpAnimator>();
    animation.setStars(score);
    if (animation != null)
    {
        animation.ResetAnimation();
    }

    activePopups.Add(popup);
    StartCoroutine(ReturnToPool(popup, 1f));
}
    private IEnumerator ReturnToPool(GameObject popup, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (activePopups.Contains(popup))
        {
            activePopups.Remove(popup);
        }

        popup.SetActive(false);
        popupPool.Enqueue(popup); // Return the popup to the pool
    }

    private string createDamageString(float review)
    {
        return"";
    }
}
