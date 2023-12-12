using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

//Can be extened to include just one direction or mulitple!
public enum BlockDirections {
    UP_DOWN,
    LEFT_RIGHT

}

public enum TargetAction {
    SHOW,
    HIDE
}


[RequireComponent(typeof(BoxCollider2D))]
public class Block : MonoBehaviour {

    private BoxCollider2D box;
    private Rigidbody2D body;
    private bool wasMoved = false; //After block is moved, prevents moving again
    private bool isBeingInteractedWith = false; //flag to check the player is interacting
    private bool startMoving = false; //flag to begin moving the block
    private float currentTime; //used to check how long the user has been pushing
    private Vector3 startingLocation; //Where the block is located
    private Vector3 toLocation = Vector3.zero; //where the block will be moved too

    [Header("INTERACTION")]
    [Tooltip("Can the player currently move the block?")]
    public bool isMoveable = true;

    [Tooltip("Which direction can the block move?")]
    public BlockDirections moveDirection;

    [Range(0.25f, 1.0f)]
    [Tooltip("How long should the player push before it moves?")]
    public float waitTime = 0.75f;

    [Range(0.25f, 1.0f)]
    [Tooltip("How fast should the block slide?")]
    public float moveSpeed = 0.75f;


    [Header("TARGET")]
    [Tooltip("What GameObject should be shown or hidden?")]
    public GameObject targetObject;
    public TargetAction visibility;

#if UNITY_EDITOR
    [Header("EDITOR ONLY")]
    [Tooltip("Draws a line from this block to its target.")]
    public bool showConnection = true;
    public Color lineColor = Color.magenta;
#endif


    private void Awake() {
        box = GetComponent<BoxCollider2D>();

        if (box == null) {
            Debug.LogAssertion("ERROR - BoxCollider2D is required on object at: " + transform.position);
            HighlightObject();
        }

        body = gameObject.AddComponent<Rigidbody2D>();
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.bodyType = RigidbodyType2D.Kinematic;

        startingLocation = transform.position;
        currentTime = waitTime;
    }

    private void Start() {
        if (targetObject == null) {
            Debug.LogWarning("WARNING - A Target GameObject has not been set.");
            HighlightObject();
        }

        //SET INITIAL HIDE/SHOW THE TARGET
        //IF THE TARGET ACTION IS SHOW WE WANT TO HIDE THE TARGET GAMEOBJECT
        //SO IT CAN BE REVEALED BY THE PUSH BLOCK. 

        switch (visibility) {
            case TargetAction.SHOW:
                targetObject.SetActive(false);
                break;
            case TargetAction.HIDE:
                targetObject.SetActive(true);
                break;
        }
    }

    private void Update() {
        if (startMoving) {
            transform.position = Vector3.MoveTowards(transform.position, toLocation, moveSpeed * Time.deltaTime);

            if (transform.position.Equals(toLocation)) {
                startMoving = false;
                wasMoved = true;
                isMoveable = false;

                switch (visibility) {
                    case TargetAction.SHOW:
                        targetObject.SetActive(true);
                        break;
                    case TargetAction.HIDE:
                        targetObject.SetActive(false);
                        break;
                }
            }
        }
    }

    private void HighlightObject() {
        SpriteRenderer r = GetComponent<SpriteRenderer>();
        r.color = Color.red;
    }

    private void MoveLeftRight(Collision2D collision) {

        if (collision.relativeVelocity.x < 0) {
            //Pushing left
            currentTime -= Time.deltaTime;

            if (currentTime < 0) {
                isBeingInteractedWith = false;
                body.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                toLocation = new Vector2(startingLocation.x - 1, startingLocation.y);
                startMoving = true;
            }

        } else if (collision.relativeVelocity.x > 0) {
            //Pushing right
            currentTime -= Time.deltaTime;

            if (currentTime < 0) {
                isBeingInteractedWith = false;
                body.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                toLocation = new Vector2(startingLocation.x + 1, startingLocation.y);
                startMoving = true;
            }

        }

    }

    private void MoveUpDown(Collision2D collision) {
        if (collision.relativeVelocity.y > 0) {
            //Pushing up
            currentTime -= Time.deltaTime;

            if (currentTime < 0) {
                isBeingInteractedWith = false;
                body.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                toLocation = new Vector2(startingLocation.x, startingLocation.y + 1);
                startMoving = true;
            }

        } else if (collision.relativeVelocity.y < 0) {
            //Pushing Down
            currentTime -= Time.deltaTime;

            if (currentTime < 0) {
                isBeingInteractedWith = false;
                body.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                toLocation = new Vector2(startingLocation.x, startingLocation.y - 1);
                startMoving = true;
            }

        }

    }


    private void OnCollisionEnter2D(Collision2D collision) {
        //check if the player, see if the block is in a state to be moved and is not previously moved and isn't currenly moving.
        if (collision.gameObject.CompareTag("Player") && isMoveable & !wasMoved && !startMoving) {
            isBeingInteractedWith = true;
            currentTime = waitTime;
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {

        //if the player starts pushing then stops, but is still touching the block, reset time
        if (collision.relativeVelocity == Vector2.zero) {
            currentTime = waitTime;
        }

        //Checks if object is the player and if so, is the player interacting with block
        if (collision.gameObject.CompareTag("Player") && isBeingInteractedWith) {

            switch (moveDirection) {
                case BlockDirections.LEFT_RIGHT:
                    MoveLeftRight(collision);
                    break;
                case BlockDirections.UP_DOWN:
                    MoveUpDown(collision);
                    break;
            }

        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player") && isBeingInteractedWith) {
            isBeingInteractedWith = false;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected() {

        if (targetObject != null && showConnection) {
            Gizmos.color = lineColor;
            Gizmos.DrawLine(transform.position, targetObject.transform.position);

            Gizmos.DrawWireSphere(transform.position, 0.5f);

            Gizmos.DrawWireSphere(targetObject.transform.position, 0.5f);
        }
    }
#endif
}
