using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public InputField playerNameInput;

    [Header("Connecting Info Panel")]
    public GameObject ConnectingInfoUIPanel;

    //[Header("Creating Room Info Panel")]
    //public GameObject CreatingRoomInfoUIPanel;

    [Header("GameOptions  Panel")]
    public GameObject GameOptionsUIPanel;


    [Header("Create Room Panel")]
    //public GameObject CreateRoomUIPanel;
    //public InputField roomNameInputField;
    private string GameMode;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomUIPanel;
    public Text roomInfoText;
    public GameObject playerListPrefab;
    public GameObject playerListContent;
    //public GameObject startGameButton;
    public Text gameModeText;
    //public Image panelBackGround;
    //public Sprite racingBackGround;
    //public Sprite deathRaceBackGround;
    public GameObject[] playerSelectionUIGameObjs;
    //public DeathRacePlayer[] deathRacePlayers;
    //public RacingPlayer[] racingPlayers;

    //[Header("Join Random Room Panel")]
    //public GameObject JoinRandomRoomUIPanel;

    private Dictionary<int, GameObject> playerListGameObjects;

    [SerializeField] private float startMatchTimer = 30f;
    [SerializeField] private bool isMatchStarting = false;

    #region Unity Methods
    private void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            //PhotonNetwork.SendRate = 40;
            //PhotonNetwork.SerializationRate = 16;
        }
    }

    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
           
        }
        else
            ActivatePanel(LoginUIPanel.name);        
    }

    private void Update()
    {
        StartMatch();
    }

    private void StartMatch()
    {
        if (!isMatchStarting || !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        startMatchTimer -= Time.deltaTime;

        if (startMatchTimer <= 0 || Input.GetKeyDown(KeyCode.P))
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
            {
                PhotonNetwork.CurrentRoom.CustomProperties["PlayersInLobbyRoom"] = PhotonNetwork.CurrentRoom.PlayerCount;
                PhotonNetwork.LoadLevel("GameScene");
                isMatchStarting = false;
                return;
            }
        }
    }

    #endregion

    #region UI Callback Methods

    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;
        ActivatePanel(ConnectingInfoUIPanel.name);

        if (!PhotonNetwork.IsConnected)
        {
            if (!string.IsNullOrEmpty(playerName))
                PhotonNetwork.LocalPlayer.NickName = playerName;
            else
                PhotonNetwork.LocalPlayer.NickName = "player#" + Random.Range(1, 1000);

            PhotonNetwork.ConnectUsingSettings();            
        }
    }

    public void OnJoinRandomButtonClicked()
    {
        ExitGames.Client.Photon.Hashtable expectedCustomRoomProps = new ExitGames.Client.Photon.Hashtable() { { "gm", "deathMatch" } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProps, 0);
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnBackButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    //public void OnCreateRoomButtonClicked()
    //{
    //    ActivatePanel(CreatingRoomInfoUIPanel.name);

    //    if (GameMode != null)
    //    {
    //        string roomName = roomNameInputField.text;

    //        if (string.IsNullOrEmpty(roomName))
    //        {
    //            roomName = "Room " + Random.Range(100, 1000);
    //        }

    //        RoomOptions roomOpts = new RoomOptions();
    //        roomOpts.MaxPlayers = 6;

    //        string[] roomPropsinLobby = { "gm" }; //gm = game mode

    //        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", GameMode } };

    //        roomOpts.CustomRoomPropertiesForLobby = roomPropsinLobby;
    //        roomOpts.CustomRoomProperties = customRoomProperties;

    //        PhotonNetwork.CreateRoom(roomName, roomOpts);
    //    }
    //}

    //public void OnStartGameButtonClicked()
    //{
    //    if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
    //    {
    //        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
    //        {
    //            //racing game mode
    //            PhotonNetwork.LoadLevel("RacingScene");
    //        }
    //        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
    //        {
    //            //death race game mode
    //            PhotonNetwork.LoadLevel("DeathRaceScene");
    //        }
    //    }
    //}

    public void OnCancelButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    #endregion

    #region Photon Callback

    public override void OnConnected()
    {
        print("connected to internet");
    }

    public override void OnConnectedToMaster()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " connected to photon");
        ActivatePanel(GameOptionsUIPanel.name);
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        print("joined lobby");
        //PhotonNetwork.JoinRandomRoom();
    }

    public override void OnCreatedRoom()
    {
        print(PhotonNetwork.CurrentRoom.Name + " is created");
        PhotonNetwork.CurrentRoom.IsOpen = true;
        if(PhotonNetwork.IsMasterClient)
        {
            isMatchStarting = true;
        }
    }

    public override void OnJoinedRoom()
    {
        print(PhotonNetwork.LocalPlayer.NickName + " joined " + PhotonNetwork.CurrentRoom.Name + "player count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        ActivatePanel(InsideRoomUIPanel.name);

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            roomInfoText.text = PhotonNetwork.CurrentRoom.Name +
                 " Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" +
                 PhotonNetwork.CurrentRoom.MaxPlayers;

            //gameModeText.text = "Death Match";

            if (playerListGameObjects == null)
            {
                playerListGameObjects = new Dictionary<int, GameObject>();
            }

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                GameObject playerListGameObject = Instantiate(playerListPrefab);
                playerListGameObject.transform.SetParent(playerListContent.transform);
                playerListGameObject.transform.localScale = Vector3.one;
                playerListGameObject.GetComponent<PlayerListEntryInitializer>().Initialize(player.ActorNumber, player.NickName);

                //object isPlayerReady;
                //if (player.CustomProperties.TryGetValue("isPlayerReady", out isPlayerReady))
                //{
                //    playerListGameObject.GetComponent<PlayerListEntryInitializer>().SetPlayerReady((bool)isPlayerReady);
                //}

                playerListGameObjects.Add(player.ActorNumber, playerListGameObject);
            }
        }

        //startGameButton.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print("entered from on player entered room");

        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name +
          " Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" +
          PhotonNetwork.CurrentRoom.MaxPlayers;

        GameObject playerListGameObject = Instantiate(playerListPrefab);
        playerListGameObject.transform.SetParent(playerListContent.transform);
        playerListGameObject.transform.localScale = Vector3.one;
        playerListGameObject.GetComponent<PlayerListEntryInitializer>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListGameObjects.Add(newPlayer.ActorNumber, playerListGameObject);

        //startGameButton.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name +
          " Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" +
          PhotonNetwork.CurrentRoom.MaxPlayers;

        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);

        //startGameButton.SetActive(CheckPlayersReady());
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(GameOptionsUIPanel.name);
        foreach (GameObject playerListGO in playerListGameObjects.Values)
        {
            Destroy(playerListGO);
        }
        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            //startGameButton.SetActive(CheckPlayersReady());
        }
    }

    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //GameObject playerListGameObj;

        //if (playerListGameObjects.TryGetValue(target.ActorNumber, out playerListGameObj))
        //{
            //object isPlayerReady;
            //if (changedProps.TryGetValue("isPlayerReady", out isPlayerReady))
            //{
            //    playerListGameObj.GetComponent<PlayerListEntryInitializer>().SetPlayerReady((bool)isPlayerReady);
            //}
        //}

        //startGameButton.SetActive(CheckPlayersReady());
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print(message);
 
        string roomName = "Room " + Random.Range(100, 1000);

        RoomOptions roomOpts = new RoomOptions();
        roomOpts.MaxPlayers = 8;

        string[] roomPropsinLobby = { "gm" }; //gm = game mode
        
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "gm", "deathMatch" }, { "PlayersInLobbyRoom", 0 } };

        roomOpts.CustomRoomPropertiesForLobby = roomPropsinLobby;
        roomOpts.CustomRoomProperties = customRoomProperties;
        roomOpts.IsOpen = true;

        PhotonNetwork.CreateRoom(roomName, roomOpts);

    }

    #endregion

    #region Public Methods
    public void ActivatePanel(string panelNameToBeActivated)
    {
        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelNameToBeActivated));
        ConnectingInfoUIPanel.SetActive(ConnectingInfoUIPanel.name.Equals(panelNameToBeActivated));
        //CreatingRoomInfoUIPanel.SetActive(CreatingRoomInfoUIPanel.name.Equals(panelNameToBeActivated));
        //CreateRoomUIPanel.SetActive(CreateRoomUIPanel.name.Equals(panelNameToBeActivated));
        GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelNameToBeActivated));
        //JoinRandomRoomUIPanel.SetActive(JoinRandomRoomUIPanel.name.Equals(panelNameToBeActivated));
        InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelNameToBeActivated));
    }

    public void SetGameMode(string gameMode)
    {
        GameMode = gameMode;
    }
    #endregion

    #region Private Methods

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (player.CustomProperties.TryGetValue("isPlayerReady", out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    #endregion
}
