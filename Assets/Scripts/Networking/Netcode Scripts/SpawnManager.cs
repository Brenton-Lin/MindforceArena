using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Components;
using Unity.VisualScripting;


public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager instance;
    public Transform[] spawnLocations;
    public GameObject localRig;
    public NetworkVariable<int> spawnIterator;
    private void Awake()
    {
        // NetworkManager.ConnectionApprovalCallback = ConnectionApprovalCallback;
        
        instance = this;
       
    }
    //may need this eventually: https://docs-multiplayer.unity3d.com/netcode/current/components/networkmanager/
    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        if(IsHost){
            spawnIterator.Value = 0;
           
        }
        //for our use case, the player rig is client authoritative. This script moves the local player, increments to the next spawn point on the server,
        //then relies on the server to update it's position to the other client.
        if(IsClient){
            Vector3 position = spawnLocations[spawnIterator.Value].position;
            Quaternion rotation = spawnLocations[spawnIterator.Value].rotation;
            localRig.transform.SetPositionAndRotation(position, rotation);
            serverIteratorUpdateServerRpc(1);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void serverIteratorUpdateServerRpc(int value){
        spawnIterator.Value += value;
        if(spawnIterator.Value >= spawnLocations.Length){
            spawnIterator.Value = 0;
        }
    }

    //called when a client joins the server.

    //Not needed but keeping for future reference.
    // void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    // {
    //     response.Approved = true;
    //     response.CreatePlayerObject = true;

    //     Vector3 spawnPosition;
    //     Quaternion spawnRotation;
    //     GetPlayerSpawnPosition(out spawnPosition, out spawnRotation);
    //     response.Position = spawnPosition;
    //     response.Rotation = spawnRotation;
    // }


    // public void GetPlayerSpawnPosition(out Vector3 position, out Quaternion rotation)
    // {
    //         //Loop through list of spawn positions
            
    //         int local = spawnIterator.Value;
    //         //if a valid position is found, return these values
    //         if(spawnLocations[local] is not null){
    //             position = spawnLocations[local].position;
    //             rotation = spawnLocations[local].rotation;
    //             serverIteratorUpdateServerRpc(1);
    //         }
    //         //else just fallback to ogs
    //         else{
    //             position = Vector3.zero;
    //             rotation = Quaternion.identity;
    //         }
    //         //call serverRPC to increment spawnIterator.

        
            
                
    // }
    

}
