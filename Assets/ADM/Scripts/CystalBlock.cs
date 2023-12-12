using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent( typeof( SpriteRenderer ) )]
[RequireComponent( typeof( BoxCollider2D ) )]
public class CystalBlock : MonoBehaviour {

    private SWITCH_STATES blockState;

    private CrystalSwitch cs;
    private UnityAction stateListener;

    private BoxCollider2D box;
    private SpriteRenderer spriteRender;

    [SerializeField]
    [Header( "BLOCK SETUP" )]
    private SWITCH_STATES thisBlockColor;

    public Sprite spriteBlockUp, spriteBlockDown;

    [Header( "THE SWITCH" )]
    public GameObject GoverningSwitch;


    private void Awake () {

        box = GetComponent<BoxCollider2D>();
        spriteRender = GetComponent<SpriteRenderer>();

        stateListener = new UnityAction( SwapBlockStates );


    }


    void Start () {
        blockState = GoverningSwitch.GetComponent<CrystalSwitch>().currentState;

        //Find out if this block is the same color as the swich, if not invert.
        //i.e If the switch starts blue and this block is red...
        if ( thisBlockColor == blockState ) {
            if ( blockState == SWITCH_STATES.BLUE ) {
                spriteRender.sprite = spriteBlockDown;
                box.enabled = false;
            }

            if ( blockState == SWITCH_STATES.PURPLE ) {
                spriteRender.sprite = spriteBlockUp;
                box.enabled = true;
            }
        } else {
            SwapBlockStates();
        }
    }


    //Swaps the blocks sprites and turns on/off the box collider
    void SwapBlockStates () {
        if ( blockState == SWITCH_STATES.BLUE ) {

            spriteRender.sprite = spriteBlockUp;
            box.enabled = true;
            blockState = SWITCH_STATES.PURPLE;
        } else {

            spriteRender.sprite = spriteBlockDown;
            box.enabled = false;
            blockState = SWITCH_STATES.BLUE;
        }


    }
    //When the block gameobject is enabled, register to listen to its Crystal Switch
    void OnEnable () {
        CrystalSwitch.StartListening( CrystalSwitch.EVENT_CRYSTAL_SWITCH_NOTIFICATION, stateListener );
    }

    //When the block gameobject is disabled, unregister the listener
    void OnDisable () {
        CrystalSwitch.StopListening( CrystalSwitch.EVENT_CRYSTAL_SWITCH_NOTIFICATION, stateListener );
    }
}
