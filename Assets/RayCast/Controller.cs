using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public GameObject itemMove;
    public bool isHold;
    private Vector3 offset;
    private static Controller instance;
    public static Controller Instance => instance;
    private void Start()
    {
        if(instance != null)
        {
            Debug.Log("Only Controller instance");
        }
        else
        {
            Controller.instance = this;
        }
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            isHold = true;
            Vector3 posRaycast = RaycastPos();         
            if (itemMove != null && posRaycast != Vector3.zero)
            {
                itemMove.transform.position = posRaycast;
                //itemMove.transform.position = Vector3.MoveTowards(itemMove.transform.position, posRaycast, 30 * Time.deltaTime);
            }
        }
    }

    public Vector3 RaycastPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.Log("Raycast initiated");

        if (Physics.Raycast(ray,out hit))
        {
            Debug.Log("Hit object: " + hit.transform.name + " with tag: " + hit.transform.tag);
            Debug.DrawRay(hit.point, Vector3.forward, Color.red, 20f);

            if (hit.transform.CompareTag("Item"))
                itemMove = hit.transform.gameObject;
            if (hit.transform.CompareTag("Ground"))
            {
                return hit.point;
            }
            return Vector3.zero;
        }
        return Vector3.zero;
    }
    public void GetObjectItem(GameObject btnObj)
    {
        itemMove = btnObj;
    }    
}
