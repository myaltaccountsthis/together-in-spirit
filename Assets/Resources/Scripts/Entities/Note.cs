using UnityEngine;

public class Note : Interactable
{
    // go
    public string message;

    protected override void Awake()
    {
        base.Awake();
        CanPlayerInteract = true;
        CanSpiritInteract = true;
    }

    public override void Interact(User user)
    {
        Debug.Log(message);
    }
}