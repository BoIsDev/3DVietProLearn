using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelRoomLobby : MonoBehaviour
{
    public Button btnBack;
    public GameObject PanelLobby;

    [SerializeField] protected ItemRoom itemRoom;
    public ItemRoom Itemroom => itemRoom;
    private static PanelRoomLobby instance;
    public static PanelRoomLobby Instance { get => instance; }
    void Start()
    {
        if (PanelRoomLobby.instance != null) Debug.LogError("Only 1 PanelRoomLobby allow to exist");
        PanelRoomLobby.instance = this;
        LoadItemRoom();
        //ClickBackButton();
    }
    public virtual void ClickBackButton()
    {
        btnBack.onClick.AddListener(() => ClickBackButton());
        PanelLobby.SetActive(false);
        PanelCreateRoom.Instance.PanelCreat.SetActive(true);
    }

    protected virtual void LoadItemRoom()
    {
        if (this.itemRoom != null) return;
        this.itemRoom = transform.GetComponentInChildren<ItemRoom>();
        Debug.Log(transform.name + ": LoadItemRoom", gameObject);
    }


}
