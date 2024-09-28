
using UnityEngine;

public class SpiritSeparator : Interactable
{
    private bool used = false;
    public Vector3 splitOffset;
    public float splitTime;
    
    protected override void Awake()
    {
        base.Awake();
        CanPlayerInteract = false;
        CanSpiritInteract = false;
    }

    public override void Interact(User user)
    {
        if (used) return;
        Player player = (Player)user;
        Spirit spirit = player.spirit;
        spirit.transform.position = player.transform.position;
        LeanTween.move(spirit.gameObject, player.transform.position + splitOffset, splitTime)
            .setEaseLinear();
        spirit.gameObject.SetActive(true);
        CanPlayerInteract = false;
        used = true;
    }
    
    public void SetEnabled(bool enabled)
    {
        if(used) return;
        CanPlayerInteract = enabled;
    }
}