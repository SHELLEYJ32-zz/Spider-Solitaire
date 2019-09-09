using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockController : MonoBehaviour
{
    public bool isActive;


    void OnMouseDown()
    {
        if (isActive)
        {
            GameController.instance.AddNewRow();
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
