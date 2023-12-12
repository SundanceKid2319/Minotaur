using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public SignalGame context;
    public bool playerInRange;



    public void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !other.isTrigger) {
            context.Raise();
            playerInRange = true;
        }
    }
    public void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player") && !other.isTrigger) {
            context.Raise();
            playerInRange = false;
        }
    }
}
