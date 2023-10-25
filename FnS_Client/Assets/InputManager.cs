using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private bool[] moveInputs;

    private bool shooting, pickingUp, dashing;

    private ushort activeSlots = 0;

    private static InputManager _singleton;

    public static InputManager Singleton
    {
        get => _singleton;

        private set
        {
            if(_singleton == null)
                _singleton = value;
            else if(_singleton != value)
            {
                Debug.Log($"{nameof(InputManager)} instance already exists");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        moveInputs = new bool[4];
    }

    private void Update()
    {
        float xIn = Input.GetAxisRaw("Horizontal");

        float yIn = Input.GetAxisRaw("Vertical");

        moveInputs[0] = yIn > 0; //Up
        moveInputs[1] = yIn < 0; //Down
        moveInputs[2] = xIn > 0; //Right
        moveInputs[3] = xIn < 0; //Left

        shooting = Input.GetButton("Fire1");

        pickingUp = Input.GetButton("Pickup");

        dashing = Input.GetKey(KeyCode.Space);

        ushort old = activeSlots;

        for(ushort i = 0; i < 4; i++)
        {
            if(Input.GetButton("Input" + i.ToString()))
            {
                activeSlots = i;
            }
        }

        if(activeSlots != old)
        {
            bool didSwitch = WeaponManager.Singleton.SendSwitch();

            if(!didSwitch) activeSlots = old;
        } 
    }

    public ushort GetActiveSlot() => activeSlots;

    public bool[] GetMoveInput() => moveInputs;

    public bool Shooting() => shooting;

    public bool Pickup() => pickingUp;
    
    public bool Dashing() => dashing;

}
