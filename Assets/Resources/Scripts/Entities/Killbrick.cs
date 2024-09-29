using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killbrick : Interactable
{
    public override void Interact(User user)
    {
        user.TakeDamage(1000);
    }
}
