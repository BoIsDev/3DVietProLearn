using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class PanelRaycast : MonoBehaviour
{

    public Button btnItem1;
    public Button btnItem2;
    public Button btnItem3;

    public void ClickHoldItem(GameObject btn)
    {
        Debug.Log(btn.name);
        Controller.Instance.GetObjectItem(btn);
    }

    public void Start()
    {
        ClickBtn();
    }

    public void ClickBtn()
    {
        btnItem1.onClick.AddListener(() => ClickHoldItem(btnItem1.gameObject));
        btnItem2.onClick.AddListener(() => ClickHoldItem(btnItem2.gameObject));
        btnItem3.onClick.AddListener(() => ClickHoldItem(btnItem3.gameObject));
    }


}
