using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    public string suit;
    public int num;
    public bool active; //when active, card shows the front; otherwise, it shows the back
    public bool finished; //when finished, card shows the front but is not movable
    public Sprite frontSprite;
    public Sprite backSprite;
    public int myColumn = -1;
    public int myRow = -1;

    void Update()
    {
        if (active)
            gameObject.GetComponent<SpriteRenderer>().sprite = frontSprite;
        else
            gameObject.GetComponent<SpriteRenderer>().sprite = backSprite;

    }

    public void OnMouseDown()
    {
        if (active && !finished)
        {
            GameController.instance.ProcessClickOnCard(gameObject.GetComponent<CardController>());
        }

    }

}
