using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PageManager : MonoBehaviour
{
    public Transform HoldMapLevel;
    public List<Transform> lstMapLevel = new List<Transform>();
    public int initPage = 9;
    public Text txtNumberPage;
    private static PageManager instance;
    public static PageManager Instance => instance;
    public void Start()
    {
        if (instance != null)
        {
            Debug.Log("Only one PanelMap!!");
        }
        else
        {
            PageManager.instance = this;
        }
        LoadHoldMapLevel();
    }
    public void LoadHoldMapLevel()
    {
        if (this.lstMapLevel.Count > 0) return;
        foreach (Transform objMaplevel in HoldMapLevel)
        {
            this.lstMapLevel.Add(objMaplevel);
        }
        HideObject();
        SetPage(0);
    }
    protected virtual void HideObject()
    {
        foreach(Transform objMaplevel in lstMapLevel)
        {
            objMaplevel.gameObject.SetActive(false);
        }
    }

    public virtual void SetPage(int numberPage)
    {
        int a = (numberPage-1) * initPage;
        var currentPage = lstMapLevel.Skip(a).Take(initPage);
        HideObject();
        foreach (Transform objMapLevel in currentPage)
        {
            objMapLevel.gameObject.SetActive(true);
        }
    }
}
