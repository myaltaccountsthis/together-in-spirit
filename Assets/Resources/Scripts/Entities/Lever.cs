using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactable
{
    public GameObject[] doors;
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
        foreach (GameObject door in doors)
            door.SetActive(!activated);
        UpdateSprite();
    }
}
