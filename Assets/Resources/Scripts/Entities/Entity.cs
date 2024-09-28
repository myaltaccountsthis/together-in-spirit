using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected virtual void Awake() {}
    protected virtual void Start() {}
    protected virtual void Update() {}
    protected virtual void FixedUpdate() {}
    protected virtual void OnTriggerEnter2D(Collider2D other) {}
    protected virtual void OnTriggerExit2D(Collider2D other) {}
}
