using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSucker : MonoBehaviour
{
    public static ItemSucker itemSucker { get; private set; }

    [Header("Suck Settings")]
    public List<GameObject> itemsToSuck;
    public List<foodInfoObj.foodParts[]> foodBits = new List<foodInfoObj.foodParts[]>();
    public Transform peakPoint;
    public Transform targetPoint;
    public Transform distancePoint;
    public float maxDistance = 10;
    public float travelDuration = 0.5f;
    public float delayBetweenItems = 0.1f;

    public orderProcessing orderDoer;
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public List<foodInfoObj> foodObjHolder = new List<foodInfoObj>();

    void Start()
    {
        itemSucker = this;
    }

    public void StartSucking()
    {
        StartCoroutine(SuckItemsRoutine());
    }

    public void tryStartSuck(List<GameObject> items)
    {
        if (Vector3.Distance(areaMoveTransitionCode.AMTC.player.transform.position, distancePoint.position) < maxDistance)
        {
            itemsToSuck = items;
            StartSucking();
        }
    }

    private IEnumerator SuckItemsRoutine()
    {
        for (int i = itemsToSuck.Count - 1; i >= 0; i--)
        {
            GameObject item = itemsToSuck[i];
            StartCoroutine(MoveItemAlongCurve(item));
            yield return new WaitForSeconds(delayBetweenItems);
        }

        yield return new WaitForSeconds(travelDuration);

        if (foodBits.Count > 0)

        {
            List<foodInfoObj.foodParts> bits = new List<foodInfoObj.foodParts>();
            foreach(foodInfoObj.foodParts[] parts in foodBits){
                foreach(foodInfoObj.foodParts part in parts){
                    bits.Add(part);
                }
            }
            orderDoer.fillOrder(bits);
        }

        // Cleanup & Reset
        foreach (var item in itemsToSuck)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
            pickupscript.instance.wipeStack();

        itemsToSuck.Clear();
        foodBits.Clear();
        foodObjHolder.Clear();
    }

    private IEnumerator MoveItemAlongCurve(GameObject item)
    {
        item.transform.SetParent(null);
        Vector3 startPoint = item.transform.position;
        Vector3 controlPoint = peakPoint.position;
        Vector3 endPoint = targetPoint.position;

        float elapsed = 0f;
        while (elapsed < travelDuration)
        {
            float t = elapsed / travelDuration;
            t = movementCurve.Evaluate(t);

            Vector3 pos = Mathf.Pow(1 - t, 2) * startPoint +
                          2 * (1 - t) * t * controlPoint +
                          Mathf.Pow(t, 2) * endPoint;

            item.transform.position = pos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (item.GetComponent<foodScript>() != null)
        {
            foodBits.Add(item.GetComponent<foodScript>().ingredients);
        }

        item.transform.position = endPoint;
    }
}
