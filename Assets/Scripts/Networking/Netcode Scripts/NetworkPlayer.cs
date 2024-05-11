using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using VRArmIK;
using System;
/// <summary>
/// Responsible for taking VR tracking references from the scene and syncing them over netcode
/// </summary>
public class NetworkPlayer : NetworkBehaviour
{

    public Transform root;
    public Transform hmd;
    public Transform leftHand;
    public Transform rightHand;

    public PoseManager networkPoseManager;

    public NetworkVariable<float> playerHeightHmd;
    public NetworkVariable<float> playerWidthWrist;
    public NetworkVariable<float> playerWidthShoulders;

    public bool noOwnerRender;
    // Start is called before the first frame update
    public GameObject[] componentsToDisable;


    [SerializeField]
    private float avatarReferenceHeight = 1.8f;

    //References for VRarmIK scripts

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();//
        
        playerHeightHmd.OnValueChanged += CalibrateNetworkArmIk;
        playerWidthShoulders.OnValueChanged += CalibrateNetworkArmIk;
        playerWidthWrist.OnValueChanged += CalibrateNetworkArmIk;
        
        
        if (IsOwner)
        {
            
            // Vector3 spawnPosition;
            // Quaternion spawnRotation;
            // SpawnManager.instance.GetPlayerSpawnPosition(out spawnPosition, out spawnRotation);
            // GameObject localPlayerRig = GameObject.Find("OVRCameraRigInteraction");
            // localPlayerRig.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            // gameObject.transform.SetPositionAndRotation(spawnPosition,spawnRotation);

            //if owner, get scale now, either here or, OnClientConnected callback.

            //Network Player's transform is client auth, so I can get scale here and set for owner.
            

            PoseManager localPoseManager = GameObject.FindGameObjectWithTag("LocalPoseManager").GetComponent<PoseManager>();

            //Update body size network attributes so all instances VRarmIK's know how to function.
            UpdateBodyAttributesServerRpc(localPoseManager.playerHeightHmd, localPoseManager.playerWidthWrist, localPoseManager.playerWidthShoulders);

            //Body scaling. Arm scaling handled above.
            Vector3 newScale = Vector3.one * (localPoseManager.playerHeightHmd/avatarReferenceHeight);
            gameObject.transform.localScale = newScale;

            //Should the player's avatar be rendered locally?
            if(noOwnerRender){
                foreach (var item in componentsToDisable)
                {
                    if(item != null)
                    {
                        item.SetActive(false);
                    }
                }
            }
           
            
        }
        if(IsServer){
            
            //How to wait for network variable update before invoking arm calibration locally?
        }

        networkPoseManager.playerHeightHmd = playerHeightHmd.Value;
        networkPoseManager.playerWidthWrist = playerWidthWrist.Value;
        networkPoseManager.playerWidthShoulders = playerWidthShoulders.Value;
        //networkPoseManager.networkPlayerSizeChanged();
        
    }

    private void CalibrateNetworkArmIk(float previousValue, float newValue)
    {
        networkPoseManager.playerHeightHmd = playerHeightHmd.Value;
        networkPoseManager.playerWidthWrist = playerWidthWrist.Value;
        networkPoseManager.playerWidthShoulders = playerWidthShoulders.Value;
        Debug.Log("In Calibrate NetworkArmIK Delegat");
        networkPoseManager.networkPlayerSizeChanged();
    }

    // Update is called once per frame
    void Update()
    {
        //only update your spawned player if you own the object.
        if (IsOwner)
        {
            //sync root position and rotation
            root.SetPositionAndRotation(VRTrackingReferences.instance.root.position, VRTrackingReferences.instance.root.rotation);

            hmd.SetPositionAndRotation(VRTrackingReferences.instance.hmd.position, VRTrackingReferences.instance.hmd.rotation);

            leftHand.SetPositionAndRotation(VRTrackingReferences.instance.leftHand.position, VRTrackingReferences.instance.leftHand.rotation);
            
            rightHand.SetPositionAndRotation(VRTrackingReferences.instance.rightHand.position, VRTrackingReferences.instance.rightHand.rotation);
        }
    }

    [ServerRpc]
    void UpdateBodyAttributesServerRpc(float height, float widthWrist, float widthShoulder){
        playerHeightHmd.Value = height;
        playerWidthWrist.Value = widthWrist;
        playerWidthShoulders.Value = widthShoulder;

    }

    void Awake(){
        //These variables have to be initialized, VRarmIK script might work too fast...?
        playerHeightHmd.Value = 1.7f;
        playerWidthShoulders.Value = 0.2f;
        playerWidthWrist.Value = 1.39f;
    }
}
