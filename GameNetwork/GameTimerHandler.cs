using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Scripts.Character;

public class GameTimerHandler : MonoBehaviourPunCallbacks
{
    Text timeUIText;
    [SerializeField] PlayerController playerController = null;

    [SerializeField] float initialTimeToStart = 5f;

    private void Start()
    {
        timeUIText = DeathMatchGameManager.instance.TimerToStartText;
    }

    // Update is called once per frame
    void Update()
    {
        StartMatch();        
    }

    private void StartMatch()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (initialTimeToStart >= 0)
            {                
                initialTimeToStart -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, initialTimeToStart);
            }
            else if (initialTimeToStart < 0)
            {
                photonView.RPC("EndCountDown", RpcTarget.AllBuffered);                
            }
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        if (time > 0)
        {
            if(timeUIText != null)
                timeUIText.text = time.ToString("F1");
        }
        else
        {
            timeUIText.text = "";
        }
    }
    
    [PunRPC]
    public void EndCountDown()
    {
        DeathMatchGameManager.instance.StartMatch();
        playerController.input.ControlsEnabled = true;
        this.enabled = false;
    }
}
