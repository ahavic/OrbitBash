
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using Scripts.Character;

public class DeathMatchGameManager : MonoBehaviourPunCallbacks
{
    public static GameObject planet = null;
    [SerializeField] GameObject[] playerPrefabs = null;

    public List<PlayerController> playerRefs = null;

    public Text TimerToStartText;
    [SerializeField] Text matchTimeText = null;
    [SerializeField] float matchTime = 300f;

    [SerializeField] GameObject MatchEndText = null;
    [SerializeField] float PostMatchTimer = 15f;

    [SerializeField] GameObject playerListPanel = null;
    [SerializeField] GameObject playersInMatchListContent = null;
    [SerializeField] GameObject playerListInfoPrefab = null;
    [SerializeField] Dictionary<int, GameObject> playerInfoDict = null;

    public bool matchStarted = false;
    public bool matchEnded = false;

    public static DeathMatchGameManager instance { get; private set; }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        instance = this;
        playerInfoDict = new Dictionary<int, GameObject>();
    }

    void Start()
    {
        playerRefs = FindObjectsOfType<PlayerController>().ToList();

        //print(PhotonNetwork.CurrentRoom.PlayerCount);

        planet = GameObject.FindGameObjectWithTag("MainWorld");

        if (PhotonNetwork.IsConnectedAndReady)
        {
            object selectionNumber;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("ShipSelection", out selectionNumber))
            {
                var player = PhotonNetwork.Instantiate(playerPrefabs[(int)selectionNumber].name, Vector3.right * 20, Quaternion.identity);
                player.transform.rotation = Quaternion.LookRotation(planet.transform.position - player.transform.position);
            }
        }
    }

    private void Update()
    {
        MatchCountDown();
    }

    public void StartMatch()
    {
        matchStarted = true;
        matchTimeText.gameObject.SetActive(true);
    }

    private void MatchCountDown()
    {
        if(matchTime <= 0)
        {
            StartCoroutine(MatchEnd());
            return;
        }

        if (matchStarted)
        {
            matchTime -= Time.deltaTime;

            float seconds = matchTime % 60;
            float minutes = matchTime / 60;
            matchTimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }

    public void DisplayPlayerList(bool isDisplay)
    {
        playerListPanel.SetActive(isDisplay);
    }

    public void UpdatePlayerInfoInList()
    {
        if (playerRefs.Count <= 0)
            return;

        foreach (var p in playerRefs)
        {
            GameObject info;
            playerInfoDict.TryGetValue(p.Pv.Owner.ActorNumber, out info);
            if(info != null)
            {
                int k = p.PlayerStats.KillCount;
                int d = p.PlayerStats.DeathCount;
                info.transform.GetChild(1).gameObject.GetComponent<Text>().text = k.ToString();
                info.transform.GetChild(2).gameObject.GetComponent<Text>().text = d.ToString();
            }
        }
        UpdateOrderOfPlayerList();
    }

    private void UpdateOrderOfPlayerList()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount <= 0 || playerRefs.Count <= 0)
        {
            print("no players!");
            return;
        }

        var list = PhotonNetwork.CurrentRoom.Players
            .OrderByDescending(t => int.Parse(playerInfoDict[t.Value.ActorNumber].transform.GetChild(1).gameObject.GetComponent<Text>().text));
        int i = 0;

        foreach (var p in list)
        {
            GameObject info;
            playerInfoDict.TryGetValue(p.Value.ActorNumber, out info);
            info.transform.SetSiblingIndex(i);
            i++;
        }        
    }

    public void AddPlayerRef(PlayerController playerController)
    {
        if (!playerRefs.Contains(playerController))
        {
            playerRefs.Add(playerController);

            var listPrefab = Instantiate(playerListInfoPrefab);
            listPrefab.transform.SetParent(playersInMatchListContent.transform);
            listPrefab.transform.localScale = Vector3.one;
            listPrefab.transform.GetChild(0).gameObject.GetComponent<Text>().text = playerController.Pv.Owner.NickName;
            listPrefab.transform.GetChild(1).gameObject.GetComponent<Text>().text = 0.ToString();
            listPrefab.transform.GetChild(2).gameObject.GetComponent<Text>().text = 0.ToString();

            playerInfoDict.Add(playerController.Pv.Owner.ActorNumber, listPrefab);
            UpdatePlayerInfoInList();
        }
    }

    public void RemovePlayerRef(PlayerController playerController, int actorNumber)
    {
        if (playerRefs.Contains(playerController))
        {
            playerRefs.Remove(playerController);
            playerInfoDict.Remove(actorNumber);
            UpdatePlayerInfoInList();
        }
    }

    IEnumerator MatchEnd()
    {
        MatchEndText.SetActive(true);
        matchEnded = true;
        for(int i = 0; i < playerRefs.Count; i++)
        {
            playerRefs[i].input.ControlsEnabled = false;
        }

        yield return Wait.WaitForSeconds(3f);
        MatchEndText.SetActive(false);
        DisplayPlayerList(true);
        StartCoroutine(PostMatchScreen());
    }

    IEnumerator PostMatchScreen()
    {
        yield return Wait.WaitForSeconds(PostMatchTimer);
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("LobbyScene");
    }

    public void OnQuitMatchButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}