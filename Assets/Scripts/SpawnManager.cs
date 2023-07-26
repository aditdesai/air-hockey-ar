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

    private const byte SpawnEventCode = 1;

    
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
        if (photonEvent.Code == SpawnEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            Vector3 receivedPosition = (Vector3)data[0];

            GameObject player = Instantiate(playerPrefab, receivedPosition + tableGameObject.transform.position, Quaternion.identity);

            player.GetComponent<PhotonView>().ViewID = (int)data[1];
        }
    }


    public override void OnJoinedRoom()
    {
        statusText.text = PhotonNetwork.LocalPlayer.NickName + " (" + localPlayerNumber +  ") joined the room";

        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
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

        GameObject playerGameObject, puckGameObject;
        if (localPlayerNumber < remotePlayerNumber) // local joined first
        {
            statusText.text = "Local - " + localPlayerNumber + ", Remote - " + remotePlayerNumber;
            playerGameObject = Instantiate(playerPrefab, spawnTransform2.position, Quaternion.identity);
        }
        else
        {
            statusText.text = "Local - " + localPlayerNumber + ", Remote - " + remotePlayerNumber;
            playerGameObject = Instantiate(playerPrefab, spawnTransform1.position, Quaternion.identity);
        }

        puckGameObject = Instantiate(puckPrefab, spawnTransformPuck.position, Quaternion.identity);

        MalletController malletController = playerGameObject.GetComponent<MalletController>();
        malletController.minX = minX;
        malletController.maxX = maxX;
        malletController.minZ = minZ;
        malletController.maxZ = maxZ;

        PhotonView photonView = playerGameObject.GetComponent<PhotonView>();

        if (PhotonNetwork.AllocateViewID(photonView))
        {
            object[] data = new object[]
            {
                playerGameObject.transform.position - tableGameObject.transform.position,
                photonView.ViewID
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

            PhotonNetwork.RaiseEvent(SpawnEventCode, data, raiseEventOptions, sendOptions);
        }
        else
        {
            Destroy(playerGameObject);
        }
    }
}
