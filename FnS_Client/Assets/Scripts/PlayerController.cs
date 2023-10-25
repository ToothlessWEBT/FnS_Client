using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool[] moveInputs;

    private bool dashing;

    

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        moveInputs = InputManager.Singleton.GetMoveInput();

        dashing = InputManager.Singleton.Dashing();

        SendInput();
    }

    #region Messages
    
    private void SendInput()
    {
        Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.moveInput);

        message.AddBools(moveInputs, false);
        message.AddVector3(transform.forward);
        NetworkManager.Singleton.Client.Send(message);

        if(dashing)
        {   
            message = Message.Create(MessageSendMode.unreliable, ClientToServerId.dashing);
            NetworkManager.Singleton.Client.Send(message);
        }

    }
    #endregion
}
