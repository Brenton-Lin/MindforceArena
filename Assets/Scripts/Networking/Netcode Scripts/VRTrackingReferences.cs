using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRTrackingReferences : MonoBehaviour
{
    // Start is called before the first frame update
    public static VRTrackingReferences instance;

    public Transform root;
    public Transform hmd;
    public Transform leftHand;
    public Transform rightHand;



   private void Awake()
    {
        instance = this;
    }
}
