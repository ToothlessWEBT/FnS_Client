using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool[] inputs;

    private void Start()
    {
        inputs = new bool[4];
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.W))
            inputs[0] = true;

        if(Input.GetKey(KeyCode.S))
            inputs[1] = true;
        
        if(Input.GetKey(KeyCode.A))
            inputs[2] = true;

        if(Input.GetKey(KeyCode.D))
            inputs[3] = true;
    }

    private void FixedUpdate()
    {
        SendInput();

        for(int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = false;
        }
    }

    #region Messages
    
    private void SendInput()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.input);

        message.AddBools(inputs, false);
        message.AddVector3(transform.forward);
        NetworkManager.Singleton.Client.Send(message);
    }
    #endregion
}
