using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zinnia.Action;

public class OVRInputButtonAction : BooleanAction
{
    public OVRInput.Controller controller = OVRInput.Controller.Active;
    public OVRInput.Button button;

    // Update is called once per frame
    protected virtual void Update()
    {
        Receive(OVRInput.Get(button, controller));
    }
}
