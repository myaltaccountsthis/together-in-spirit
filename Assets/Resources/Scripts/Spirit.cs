using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : MonoBehaviour
{
    [SerializeField] private Player player;

    public void Die() {
        Debug.Log("Spirit should die here");
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("SpiritDanger")) {
            Die();
        }
    }
}
