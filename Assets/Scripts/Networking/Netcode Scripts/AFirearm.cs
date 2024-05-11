using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Abstract class for a Netcode integrated firearm, intended as a base class
/// for any firearm (rifle, pistol, SAW) that we might need.
/// </summary>
public abstract class AFirearm : NetworkBehaviour
{
    
    public int damage;
    public float maxRange = 100f;
    public float fireRatePerSec = 60f;

    public int fireMode;
    public LayerMask hittableLayer;

    public ParticleSystem fireEffect;
    public ParticleSystem hitEffect;

    [SerializeField]
    private Transform firePoint;
    public bool fireToggle = false;

    private float _lastFireTime;

    void Update(){
        if(fireToggle && IsOwner){
            FireServerRpc();
        }
    }

    /// <summary>
    /// Local generic fire action to be called on clients or servers.
    /// </summary>
    public void Fire(){
        //fully automatic
        if(Time.time < _lastFireTime + 1/fireRatePerSec)
                return;
            
            _lastFireTime = Time.time;
            
            fireEffect.Play();

            if(Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, maxRange, hittableLayer)){
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                if(IsServer){
                    if(hit.transform.TryGetComponent(out ObjectHealth health)){
                        health.TakeDamageServerRpc(damage);
                    }
                }
            }
        
    }
    /// <summary>
    /// Fire may be implemented in each individual weapons child class
    /// </summary>
    [ClientRpc]
    public void FireClientRpc(){
        //check so that we don't fire twice on host.
        if(!IsHost)
        //client side raycast just to play effects.
            Fire();    
    }

    /// <summary>
    /// Server rpc so that clients can play fire effects for other players, while keeping hit detection server side.
    /// </summary>
    [ServerRpc]
    public void FireServerRpc(){
        FireClientRpc();

        //Server side raycast.
        Fire();
    }
    /// <summary>
    /// Toggle Fire method to handle VRTK trigger boolean only firing once on activation.
    /// </summary>
    public void toggleFire(){
        fireToggle = !fireToggle;
    }
    /// <summary>
    /// For clients to be able to claim ownership of the weapon and modify 
    /// it's transform locally.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void SetOwnerServerRPC(ulong clientID){
        GetComponent<NetworkObject>().ChangeOwnership(clientID);
    }

     [ServerRpc(RequireOwnership = false)]
    public void ReleaseOwnerServerRPC(){
        GetComponent<NetworkObject>().RemoveOwnership();
        GetComponent<Rigidbody>().useGravity = true;
    }

    public void SetOwner(){
        SetOwnerServerRPC(NetworkManager.Singleton.LocalClientId);
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    /// <summary>
    /// To release ownership of the weapon so that the server
    /// handles physics when not in player hand.
    /// </summary>
    public void ReleaseOwner(){
        ReleaseOwnerServerRPC();
    }
}
