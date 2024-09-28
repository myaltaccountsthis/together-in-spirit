using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactable
{
    public GameObject door;
    private bool activated = false;

    public override void Interact()
    {
        activated = !activated;
        door.SetActive(!activated);
    }
}
