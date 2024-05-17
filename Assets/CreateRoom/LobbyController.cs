using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour
{
    public Transform Content;
    public GameObject ItemRoom;
    private static LobbyController instance;
    public static LobbyController Instance => instance;
    private int roomCounter = 1;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Only one LobbyController");
            return;
        }
        instance = this;
    }

    void Start()
    {
        CreateRoom("First Room", "");
        CreateRoom("Second Room", "123");
        CreateRoom("Tesst Room", "1");
    }

    public void CreateRoom(string name, string password)
    {
        GameObject roomSpawn = Instantiate(ItemRoom, Content, false);
        ItemRoom itemRoomScript = roomSpawn.GetComponent<ItemRoom>();
        itemRoomScript.SetData(name, password, roomCounter++);
        roomSpawn.name = name;
        roomSpawn.SetActive(true);
    }

    public void JoinRoom()
    {
        Debug.Log("Welcome to the room!");
    }
}