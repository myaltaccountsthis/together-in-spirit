using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : Interactable
{
    private SpriteRenderer spriteRenderer;
    public AudioClip rotateSound;

    private bool rotatedInternal;
    /// <summary>
    /// F: top-left to bottom-right | T: top-right to bottom-left
    /// </summary>
    public bool Rotated { get => rotatedInternal; private set {
        rotatedInternal = value;
        spriteRenderer.flipX = !value;
    } }
    // Might just set this with a function in LineReflector to avoid 2 references
    [HideInInspector] public MirrorLevel lineReflector;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        Rotated = Random.value > .5f;
    }

    public override void Interact(User user)
    {
        Rotated = !Rotated;
        AudioSource.PlayClipAtPoint(rotateSound, transform.position);
        lineReflector.UpdateLine();
    }
}
