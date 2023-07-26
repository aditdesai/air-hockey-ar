using UnityEngine;
using Photon.Pun;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField nameInputField;
    public TMP_Text statusText;

    public GameObject joinButtonGameObject;


    #region UI Callback Methods
    public void OnJoinGameButtonClicked()
    {
        string playerName = nameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;

                PhotonNetwork.ConnectUsingSettings();
            }
        }

        
    }
    #endregion


    #region PHOTON Callback Methods
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
        statusText.text = "Connected to Internet";
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon");
        statusText.text = "Connected to Photon";

        SceneLoader.Instance.LoadScene("GameplayScene");
    }

    public override void OnJoinedRoom()
    {
        joinButtonGameObject.SetActive(false);
    }

    #endregion
}
