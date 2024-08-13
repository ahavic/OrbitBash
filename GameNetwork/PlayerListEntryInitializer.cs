using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerListEntryInitializer : MonoBehaviour
{
    [Header("UI References")]
    public Text playerNameText;
    //public Button playerReadyButton;
    //public Image playerReadyImage;

    int shipSelection = 0;

    public void Initialize(int playerID, string playerName)
    {
        playerNameText.text = playerName;
        if (PhotonNetwork.LocalPlayer.ActorNumber != playerID)
        {
            //playerReadyButton.gameObject.SetActive(false);
        }
        else
        {
            //i am local player
            ExitGames.Client.Photon.Hashtable initialProps = new ExitGames.Client.Photon.Hashtable() { { "ShipSelection", shipSelection } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);

            //playerReadyButton.onClick.AddListener(() =>
            //{
            //    isPlayerReady = !isPlayerReady;
            //    SetPlayerReady(isPlayerReady);

            //    ExitGames.Client.Photon.Hashtable newProps = new ExitGames.Client.Photon.Hashtable() { { "isPlayerReady", isPlayerReady } };
            //    PhotonNetwork.LocalPlayer.SetCustomProperties(newProps);
            //});
        }
    }

    //public void SetPlayerReady(bool pReady)
    //{
    //    playerReadyImage.enabled = pReady;

    //    if (pReady)
    //    {
    //        playerReadyButton.GetComponentInChildren<Text>().text = "Ready!";
    //    }
    //    else
    //    {
    //        playerReadyButton.GetComponentInChildren<Text>().text = "Ready?";
    //    }
    //}
}
