using UnityEngine.UI;
using UnityEngine;

public class ItemRoom : MonoBehaviour
{
    public InputField inputRePassword;
    public Text txtRoomName;
    public Button btnJoin;
    public int RoomId { get; private set; }

    public void SetData(string name, string password, int roomId)
    {
        RoomId = roomId;
        txtRoomName.text = $"ID {RoomId} - {name}";
        btnJoin.onClick.AddListener(() => JoinRoom(password));
    }
    private void JoinRoom(string password)
    {
        if (string.IsNullOrEmpty(password) || password == inputRePassword.text)
        {
            Debug.Log("Joining room: " + txtRoomName.text);
            LobbyController.Instance.JoinRoom();
            inputRePassword.gameObject.SetActive(false);
        }
        else
        {
            inputRePassword.gameObject.SetActive(true);
        }
    }
}