using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [HideInInspector] public bool visited;
    public int index;
    public Transform spawnLocation, spiritSpawnLocation;
    public Transform shouldResetChildren;
    private List<GameObject> children;

    void Awake() {
        Debug.Assert(GetComponent<Collider2D>() != null, "Room must have a Collider2D component.");
        children = new List<GameObject>();
        if (shouldResetChildren == null) {
            shouldResetChildren = spawnLocation;
            Debug.LogWarning($"Room {index} does not have shouldResetChildren set. Using spawnLocation instead.");
        }
        foreach (Transform child in shouldResetChildren) {
            if (!child.gameObject.activeSelf)
                continue;
            children.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }

    public void Activate() {
        // Destroy previous children that are active
        foreach (Transform child in shouldResetChildren) {
            if (child.gameObject.activeSelf)
                Destroy(child.gameObject);
        }
        // Instantiate new children
        foreach (GameObject child in children) {
            Instantiate(child, child.transform.position, Quaternion.identity, shouldResetChildren).SetActive(true);
        }
    }

    void Start() {
        visited = false;
    }
}
