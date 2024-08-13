using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerSelection : MonoBehaviour
{
    [SerializeField] Button PlayerSelectButton = null;
    public string[] selectablePlayers;
    public int PlayerSelectionNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        ActivatePlayer();
    }

    public void SelectPlayer()
    {
        PlayerSelectionNumber++;
        if (PlayerSelectionNumber >= selectablePlayers.Length)
        {
            PlayerSelectionNumber = 0;
        }

        ActivatePlayer();
    }

    //public void PreviousPlayer()
    //{
    //    PlayerSelectionNumber--;
    //    if (PlayerSelectionNumber < 0)
    //    {
    //        PlayerSelectionNumber = selectablePlayers.Length - 1;
    //    }

    //    ActivatePlayer();
    //}

    public void ActivatePlayer()
    {
        PlayerSelectButton.GetComponentInChildren<Text>().text = selectablePlayers[PlayerSelectionNumber] + " Ship";

        //selectablePlayers[x].SetActive(true);

        //setting up player selection custom property
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable() { { "ShipSelection", PlayerSelectionNumber } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
    }
}
