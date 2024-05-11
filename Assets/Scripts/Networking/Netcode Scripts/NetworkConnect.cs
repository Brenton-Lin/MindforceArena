using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Wrapper class that exposes Netcode connection methods for use with Unity GUI/events
/// basic at the moment, can be extended with player lobbys and such, ask Brenton.
/// </summary>
public class NetworkConnect : MonoBehaviour
{
   
    public  void Create()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        
    }
    
}
