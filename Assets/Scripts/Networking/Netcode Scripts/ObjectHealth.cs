using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Generic server side health tracking script
/// </summary>
public class ObjectHealth : NetworkBehaviour
{
    /// <summary>
    /// max health this object can have
    /// </summary>
    public int maxHealth = 100;

    /// <summary>
    /// objects current health during runtime
    /// </summary>
    private int currentHealth;


    /// <summary>
    /// OnNetworkSpawn is a handy method provided by Netcode, always called when an object
    /// is spawned, server side and locally.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        //disable health tracking on clients for performance.
        base.OnNetworkSpawn();
        if(!IsOwner){
            enabled = false;
        }
        currentHealth = maxHealth;

    }
    /// <summary>
    /// Server side health management, 
    /// </summary>
    /// <param name="damage"> Damage to subtract from the hit objects health pool</param>
    [ServerRpc]
    public void TakeDamageServerRpc(int damage){
        currentHealth -= damage;
        Debug.Log("Target hit! Current health:" + currentHealth);
        if(currentHealth <= 0){
            Die();
        }

    }
    /// <summary>
    /// Handle Death, by default despawn.
    /// </summary>
    public void Die(){
        NetworkObject.Despawn();
    }
    /// <summary>
    /// public method to reset health.
    /// </summary>
    public void resetHealth(){
        currentHealth = maxHealth;
    }
}
