using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelCreateRoom : MonoBehaviour
{
    public GameObject PanelCreat;
    public Button btnCreatRoom;
    public InputField txtInputNameRoom;
    public InputField txtInputPassWordRoom;
    protected string inputCreateRoom; // Đưa biến này ra ngoài để truy cập trong các method khác
    protected string inputPassWordRoom;

    private static PanelCreateRoom instance;
    public static PanelCreateRoom Instance { get => instance; }

    void Start()
    {
        if (PanelCreateRoom.instance != null)
        {
            Debug.LogError("Only 1 PanelCreateRoom allow to exist");
        }
        PanelCreateRoom.instance = this;

        if (txtInputNameRoom == null || btnCreatRoom == null) return;

            ClickCreateRoom();
    }
    private void OnSubmitPressed()
    {
        inputCreateRoom = txtInputNameRoom.text; // Lấy text từ InputField
        inputPassWordRoom = txtInputPassWordRoom.text; // Lấy text từ InputField
    }

    public void ClickCreateRoom()
    {
        btnCreatRoom.onClick.AddListener(delegate
        {
            OnSubmitPressed();
            //PanelRoomLobby.Instance.PanelLobby.SetActive(true);
            //PanelCreat.SetActive(false);
            if (LobbyController.Instance != null)
            {
                LobbyController.Instance.CreateRoom(inputCreateRoom, inputPassWordRoom); // Sử dụng biến này sau khi đã được gán giá trị
            }
        });
    }
}
