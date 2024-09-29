using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : Interactable
{
    public UnityAction enable;
    public UnityAction disable;
    public Sprite activeSprite, inactiveSprite;

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
}
