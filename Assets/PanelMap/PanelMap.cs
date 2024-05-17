using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelMap : MonoBehaviour
{
    public Button btnPrePage;
    public Button btnNextPage;
    public Text txtNumberPage;
    public Transform holdMapLevel;
    public Button btnUnLockLevel;
    public Button btnLockLevel;
    public Button btnNowLevel;
    public int maxMap = 100;
    public int initNumberPage = 1;
    private static PanelMap instance;
    public static PanelMap Instance => instance;
    public void Start()
    {
        if (instance != null)
        {
            Debug.Log("Only one PanelMap!!");
        }
        else
        {
            PanelMap.instance = this;
        }
        CreateMap();
        OnclickButton();
    }
    public void CreateMap()
    {
        for(int i = 1; i <= maxMap; i++)
        {
            int currentMap = 20;
            int openLevel = i;
            if(i< currentMap)
            {
                Button btnMapLevel = Instantiate(btnUnLockLevel, holdMapLevel);
                btnMapLevel.name = i.ToString();
                btnMapLevel.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
                btnMapLevel.onClick.AddListener(() => OpenLevelMap(openLevel));
            }
            else if( i == currentMap)
            {
                Button btnMapLevel = Instantiate(btnNowLevel, holdMapLevel);
                btnMapLevel.name = i.ToString();
                btnMapLevel.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
                btnMapLevel.onClick.AddListener(() => OpenLevelMap(openLevel));

            }
            else
            {
                Button btnMapLevel = Instantiate(btnLockLevel, holdMapLevel);
                btnMapLevel.name = i.ToString();
                btnMapLevel.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
            }
        }
    }

    public void Page(int numberPage)
    {
        initNumberPage += numberPage;
        int maxPage;
        if (maxMap %9 ==0)
        {
            maxPage = maxMap / 9;
        }
        else
        {
            maxPage = (maxMap /9)+1;
        }
        if (initNumberPage <=0) 
        {
            initNumberPage = maxPage;
            txtNumberPage.text = " Page " + initNumberPage;
            PageManager.Instance.SetPage(initNumberPage);
        }
        else if(initNumberPage > maxPage)
        {
            initNumberPage = 1;
            txtNumberPage.text = " Page " + initNumberPage;
            PageManager.Instance.SetPage(initNumberPage);
        }
        else
        {
            txtNumberPage.text = " Page " + initNumberPage;
            PageManager.Instance.SetPage(initNumberPage);
        }
    }

    public void OpenLevelMap(int level)
    {
        Debug.Log("Open Map :" + level);
    }    
    public void OnclickButton()
    {
        btnPrePage.onClick.AddListener(() => Page(-1));
        btnNextPage.onClick.AddListener(() => Page(1));
    }


}
