using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class areaMoveTransitionCode : MonoBehaviour
{
    public static areaMoveTransitionCode AMTC {get; private set;}
    public PlayerClearingTransition pct;
    public bool playerIsMoving = false;
    public bool cameraIsMoving = false;

    public bool isTransitioningScene;

    public cameraMover cm;

    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        if(AMTC != null) Destroy(this.gameObject);
        AMTC=this;

        cm.finishedMovingCamera.AddListener(finishTransition);
        pct.onTransitionComplete.AddListener(transitionIsFinished);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startMovement(clearingExitManager startPoint, clearingExitManager endPoint, Vector2 direction){
        player.GetComponent<playerBasicMovementScript>().freezeMovement();
        playerIsMoving=true;
        isTransitioningScene=true;
        cameraIsMoving=true;
        StartCoroutine(startTransition(startPoint, endPoint, direction));
        Debug.Log("Starting Movement");
    }
    public IEnumerator startTransition(clearingExitManager startPoint, clearingExitManager endPoint, Vector2 direction){
        pct.StartTransition(direction, endPoint, player.transform);
        yield return new WaitForSeconds(.5f);
        cm.MoveFromTo(startPoint.cameraPos.transform, endPoint.cameraPos.transform);
        yield return null;
    }
    public void finishTransition(){
        cameraIsMoving=false;
        pct.ContinueMovement();
    }

    public void transitionIsFinished(){
        player.GetComponent<playerBasicMovementScript>().unfreezeMovement();
        playerIsMoving=false;
        isTransitioningScene=false;
    }
    public bool getCanDoStuff(){
        return !isTransitioningScene && !cameraIsMoving && !playerIsMoving;
    }
}
