using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Scripts.Character;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] Text playerNameText = null;
    [SerializeField] bool control = false;

    void Start()
    {
        //move to awake?
        photonView.ObservedComponents.Add(GetComponentInChildren<PhotonAnimatorView>());

        if(control)
        {
            GetComponentInChildren<PlayerController>().input.ControlsEnabled = true;
            return;
        }

        //local client
        if (photonView.IsMine)
        {
            //GetComponentInChildren<PlayerController>().input.ControlsEnabled = true;
        }
        //remote
        else
        {
            playerNameText.text = photonView.Owner.NickName;
            //GetComponentInChildren<PlayerController>().input.ControlsEnabled = false;
        }
    }    
}
