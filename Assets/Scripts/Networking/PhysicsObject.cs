

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ubiq.Messaging;

public class PhysicsObject : NetworkedObject
{
    public NetworkContext context;
    public bool owner;
    public bool use;
    public ParticleSystem testParticles;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
        rb = GetComponent<Rigidbody>();
        owner = ServerClientFlag.Instance.isServer;
    }

    Vector3 lastPosition;

    // Update is called once per frame
    void Update()
    {
        //Network Physics
        if (lastPosition != transform.localPosition)
        {
            
            lastPosition = transform.localPosition;
            if (owner) 
            {
                context.SendJson(new Message()
                {
                    position = transform.localPosition,
                    rotation = transform.localRotation,
                    otherOwners = false,
                    isKinematic = true,
                    use = use
                }) ;
            }
            else
            {
               
            }
            
        }
        else
        {
            
            
        }
        //NetworkedEvents
        if (use)
        {
            DoUse();
            use = false;
        }
        //Object has stopped moving, turn isKinematic off
        if (rb != null)
        {
            //can't rely on rb.velocity for the velocity of the object for some reason.
            //but we may not need to reset isKinematic, as long as only one owner is allowed and only they are not kinematic...
            //the single owner should set every other object to kinematic to recieve position from the owner.
            //VRTK interactable system sets objects to not kinematic when grabbed

            //we need a way to set control back to the server once an object has stopped moving.
        }
    }

    private struct Message
    {
        public Vector3 position;
        public Quaternion rotation;
        public bool otherOwners;
        public bool isKinematic;
        public bool use;
    }
    public void SetOwner() { owner = true; }

    public void UseObject()
    {
        use = true;
    }

    public virtual void DoUse() { }
    
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        // Parse the message
        var m = message.FromJson<Message>();

        // Use the message to update the Component
        transform.localPosition = m.position;
        transform.localRotation = m.rotation;
        owner = m.otherOwners;
        // Make sure the logic in Update doesn't trigger as a result of this message
        lastPosition = transform.localPosition;
        if(rb != null)
        {
            rb.isKinematic = m.isKinematic;
        }
        
        use = m.use;    
    }
}
