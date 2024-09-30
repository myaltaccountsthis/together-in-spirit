using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePart : Interactable
{
    public int damage;

    public override void Interact(User user)
    {
        user.TakeDamage(damage);
    }
}
