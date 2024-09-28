using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactable
{
    public GameObject[] doors;
    private bool activated = false;

    public override void Interact(User user)
    {
        activated = !activated;
        foreach (GameObject door in doors)
            door.SetActive(!activated);
    }
}
