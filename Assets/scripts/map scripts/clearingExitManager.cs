using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clearingExitManager : MonoBehaviour
{
    public LayerMask playerLayer;

    public Transform cameraPos;

    public clearingExitManager connectedPoint;

    public Transform exitPoint;

    /// <summary>
    /// x value - = left + = right
    /// y value - = down, + = up
    /// </summary>
    public Vector2 direction;

    public MeshRenderer barrierMesh;
    public GameObject doorWall;
    public RoomBehavior Scriptarea;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setCanGo(bool canGo){
        if(canGo){
            barrierMesh.material = areaMoveTransitionCode.AMTC.canGoMat;
            doorWall.SetActive(false);

        }
        else{
            barrierMesh.material = areaMoveTransitionCode.AMTC.cantGoMat;

        }
    }
    private void OnTriggerEnter(Collider other) {
                if (( playerLayer & (1 << other.gameObject.layer)) != 0){
                    Debug.Log("player entered exit zone");
                    if(areaMoveTransitionCode.AMTC.getCanDoStuff() && connectedPoint!=null){
                        areaMoveTransitionCode.AMTC.startMovement(this, connectedPoint, direction);
                    }
                }

    }
}
