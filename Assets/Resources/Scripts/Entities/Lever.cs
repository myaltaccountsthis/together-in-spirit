using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : Interactable
{
    public UnityEvent enable;
    public UnityEvent disable;
    public Sprite activeSprite, inactiveSprite;
    public AudioClip flipSound;
    private SpriteRenderer spriteRenderer;
    private bool activated = false;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (activeSprite == null)
            activeSprite = spriteRenderer.sprite;
        if (inactiveSprite == null)
            inactiveSprite = spriteRenderer.sprite;
    }

    private void UpdateSprite() {
        spriteRenderer.sprite = activated ? activeSprite : inactiveSprite;
    }

    public override void Interact(User user)
    {
        activated = !activated;
        AudioSource.PlayClipAtPoint(flipSound, transform.position);
        if(activated)
        {
            enable.Invoke();
        }
        else
        {
            disable.Invoke();
        }
        UpdateSprite();
    }

    public void Unbind()
    {
        CanPlayerInteract = false;
        CanSpiritInteract = false;
    }
}
