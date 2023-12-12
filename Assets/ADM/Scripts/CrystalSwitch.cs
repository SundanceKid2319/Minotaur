using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum SWITCH_STATES {
    PURPLE, BLUE
};

//YOU NEED AT LEAST ONE BOXCOLLIDER AS A TRIGGER. 
//YOU CAN HAVE ANOTHER BOX COLLIDER IF YOU DON'T WANT ANOTHER GAMEOBJECT TO PASS THROUGH IT.
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class CrystalSwitch : MonoBehaviour {


    //EVENT STRING USED TO BROADCAST MESSAGES MADE STATIC AND PUBLIC 
    //SO IT CAN BE ACCESS FROM OTHER CLASSES WITHOUT HAVING TO TYPE OUT THE BROADCAST STRING
    public static string EVENT_CRYSTAL_SWITCH_NOTIFICATION = "EVENT_CRYSTAL_SWITCH_NOTIFICATION";

    //A LIST OF ALL THE BLOCKS THAT LISTEN FOR THE EVENTS
    private Dictionary<string, UnityEvent> blockDictionary;

    private static CrystalSwitch switchManager;
    private SpriteRenderer spriteRender;
    private BoxCollider2D box;

    [Header("On/Off Sprites")]
    public Sprite spritePurple;
    public Sprite spriteBlue;


    [Header("Start State")]
    [Tooltip("What color should be active/up?")]
    public SWITCH_STATES initialState;


    [HideInInspector]
    public SWITCH_STATES currentState;


    private void Awake() {
        spriteRender = GetComponent<SpriteRenderer>();

        //Auto setup the collider
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
    }

    private void Start() {

        if (initialState == SWITCH_STATES.BLUE) {
            spriteRender.sprite = spriteBlue;
        }

        if (initialState == SWITCH_STATES.PURPLE) {
            spriteRender.sprite = spritePurple;
        }

        currentState = initialState;

        //Let all the blocks know which state they need to start in
        TriggerEvent(EVENT_CRYSTAL_SWITCH_NOTIFICATION);
    }


    /// <summary>
    /// Call this to switch the crystal state and image
    /// </summary>
    private void SwapStates() {

        if (currentState == SWITCH_STATES.BLUE) {
            spriteRender.sprite = spritePurple;
            currentState = SWITCH_STATES.PURPLE;
        } else if (initialState == SWITCH_STATES.PURPLE) {
            spriteRender.sprite = spriteBlue;
            currentState = SWITCH_STATES.BLUE;
        }
    }

    /// <summary>
    /// This looks for a gameobject tagged 'Player' that is touching the switch.
    /// You could take a gameobject to be a sword, or bomb or bullet etc..
    /// </summary>
    /// <param name="other">Other.</param>
    private void OnTriggerEnter2D(Collider2D other) {

        if (other.gameObject.CompareTag("Player")) {
            SwapStates();
            TriggerEvent(EVENT_CRYSTAL_SWITCH_NOTIFICATION);
            //If you want to add a soundfx here is the place you should do it.
        }
    }


    /// <summary>
    /// EVENT TRIGGERING
    /// 
    /// YOU DO NOT HAVE TO EDIT ANYTING BELOW IF YOU DO NOT WANT TO.
    /// THIS BROADCASTS TO ALL LISTENING BLOCKS THE STATE THEY SHOULD GO IN
    ///
    /// </summary>
    /// <value>The instance of the switch trigger.</value>

    public static CrystalSwitch instance {
        get {
            if (!switchManager) {
                switchManager = FindObjectOfType(typeof(CrystalSwitch)) as CrystalSwitch;

                if (!switchManager) {
                    Debug.LogError("There needs to be one active CrystalSwitch script on a GameObject in your scene.");
                } else {
                    switchManager.Init();
                }
            }

            return switchManager;
        }
    }

    void Init() {
        if (blockDictionary == null) {
            blockDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    public static void StartListening(string eventName, UnityAction listener) {
        UnityEvent thisEvent = null;
        if (instance.blockDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.AddListener(listener);
        } else {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.blockDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction listener) {
        if (switchManager == null) return;
        UnityEvent thisEvent = null;
        if (instance.blockDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName) {
        UnityEvent thisEvent = null;
        if (instance.blockDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.Invoke();
        }
    }
}

