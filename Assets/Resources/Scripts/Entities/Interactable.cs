using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : Entity
{
    public bool CanPlayerInteract = true;
    public bool CanSpiritInteract = false;
    public bool AutoInteract = false;

    public abstract void Interact(User user);
}