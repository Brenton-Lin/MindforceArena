using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotHealth : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    Ragdoll ragdoll;
    // LocalBotControl botControl;

    public AiAgent agent;

    public bool dealFatalDamage;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        ragdoll = GetComponent<Ragdoll>();
        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        // botControl = GetComponent<LocalBotControl>();
        agent = GetComponent<AiAgent>();
        foreach (var rigidBody in rigidBodies)
        {
            Hitbox hitbox = rigidBody.gameObject.AddComponent<Hitbox>();
            hitbox.health = this;
        }
    }

    // Update is called once per frame
    public void DoDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !agent.isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        ragdoll.ActivateRagdoll();

        agent.isDead = true;
        GameObject.Find("BotAudit").GetComponent<BotAudit>().enemies.Remove(agent);
        agent.stateMachine.ChangeState(AiStateId.Die);
        // agent.NetworkBots(2);
        // Destroy(gameObject, 5f);
       
        // botControl.dead = true;
    }

    public void Update()
    {
        if (dealFatalDamage)
            DoDamage(currentHealth);
    }
}
