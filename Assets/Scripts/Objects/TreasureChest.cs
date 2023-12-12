using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureChest : Interactable
{
    [Header("Contents")]
    public Item contents;
    public Inventory playerInventory;
    public bool isOpen;
    public BoolValue storedOpen;

    [Header("Signals and Dialog")]
    public SignalGame raiseItem;
    public GameObject dialogBox;
    public Text dialogText;

    [Header("Animation")]
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        isOpen = storedOpen.RuntimeValue;
        if (isOpen) {
            anim.SetBool("opened", true);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("attack") && playerInRange) {
            if (!isOpen) {
                //Open the chest
                OpenChest();
            }else{
                //Chest is already open
                ChestAlreadyOpen();
            }
        }
    }
    public void OpenChest() {
        //Dialog window on
        dialogBox.SetActive(true);
        //dialog text = contents text
        dialogText.text = contents.itemDescription;
        //add contents to the inventory
        playerInventory.AddItem(contents);
        playerInventory.currentItem = contents;
        //Raise the signal to the player to animate
        raiseItem.Raise();
        //raise the context clue
        context.Raise();
        //set the chest to opened
        isOpen = true;
        anim.SetBool("opened", true);
        storedOpen.RuntimeValue = isOpen;
    }
    public void ChestAlreadyOpen() {
        //dialog off
        dialogBox.SetActive(false);
        //raise the signal to the player to stop animating
        raiseItem.Raise();
    }

    public new void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !other.isTrigger && !isOpen) {
            context.Raise();
            playerInRange = true;
        }
    }
    public new void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player") && !other.isTrigger && !isOpen) {
            context.Raise();
            playerInRange = false;
        }
    }
}