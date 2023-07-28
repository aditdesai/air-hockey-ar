using System;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public TMP_Text statusText;

    public Transform spawnTransform1;
    public Transform spawnTransform2;
    public Transform spawnTransformPuck;

    public GameObject playerPrefab;
    public GameObject puckPrefab;

    int localPlayerNumber = Int32.MaxValue, remotePlayerNumber = Int32.MaxValue;

    public GameObject tableGameObject;

    public Transform minX, maxX, minZ, maxZ;

    private const byte SpawnPlayerEventCode = 1;
    private const byte SpawnPuckEventCode = 2;


    private void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == SpawnPlayerEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            Vector3 receivedPosition = (Vector3)data[0];

            GameObject player = Instantiate(playerPrefab, receivedPosition + tableGameObject.transform.position, Quaternion.identity);

            player.GetComponent<PhotonView>().ViewID = (int)data[1];
        }


        if (photonEvent.Code == SpawnPuckEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            Vector3 receivedPosition = (Vector3)data[0];

            GameObject puck = Instantiate(puckPrefab, receivedPosition + tableGameObject.transform.position, Quaternion.identity);

            puck.GetComponent<PhotonView>().ViewID = (int)data[1];
        }
    }


    public override void OnJoinedRoom()
    {
        statusText.text = PhotonNetwork.LocalPlayer.NickName + " (" + localPlayerNumber +  ") joined the room";

        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
                SpawnPuck();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        statusText.text = newPlayer.NickName + " (" + remotePlayerNumber + ") joined the room";
    }


    private void SpawnPlayer()
    {
        localPlayerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        if (PhotonNetwork.PlayerListOthers.Length > 0)
            remotePlayerNumber = PhotonNetwork.PlayerListOthers[0].ActorNumber;

        GameObject playerGameObject;
        if (localPlayerNumber < remotePlayerNumber) // local joined first
        {
            statusText.text = "Local - " + localPlayerNumber + ", Remote - " + remotePlayerNumber;
            playerGameObject = Instantiate(playerPrefab, spawnTransform1.position, Quaternion.identity);
        }
        else
        {
            statusText.text = "Local - " + localPlayerNumber + ", Remote - " + remotePlayerNumber;
            playerGameObject = Instantiate(playerPrefab, spawnTransform2.position, Quaternion.identity);
        }


        MalletController malletController = playerGameObject.GetComponent<MalletController>();
        malletController.minX = minX;
        malletController.maxX = maxX;
        malletController.minZ = minZ;
        malletController.maxZ = maxZ;

        PhotonView playerPhotonView = playerGameObject.GetComponent<PhotonView>();


        if (PhotonNetwork.AllocateViewID(playerPhotonView))
        {

            object[] data = new object[]
            {
                playerGameObject.transform.position - tableGameObject.transform.position,
                playerPhotonView.ViewID
            };

            // raise event to send this data to other players
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };


            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(SpawnPlayerEventCode, data, raiseEventOptions, sendOptions);
        }
        else
        {
            Destroy(playerGameObject);
        }
    }


    private void SpawnPuck()
    {
        GameObject puckGameObject = Instantiate(puckPrefab, spawnTransformPuck.position, Quaternion.identity);

        PhotonView puckPhotonView = puckGameObject.GetComponent<PhotonView>();

        if (PhotonNetwork.AllocateViewID(puckPhotonView))
        {

            object[] data = new object[]
            {
                puckGameObject.transform.position - tableGameObject.transform.position,
                puckPhotonView.ViewID
            };

            // raise event to send this data to other players
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };


            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(SpawnPuckEventCode, data, raiseEventOptions, sendOptions);
        }
        else
        {
            Destroy(puckGameObject);
        }
    }
}
