using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public void LoadGame()
    {
        GameController.instance.StartGame();
        //disable start button
        GameObject.Find("StartButton").gameObject.GetComponent<Button>().interactable = false;
        //enable reset button
        GameObject.Find("ResetButton").gameObject.GetComponent<Button>().interactable = true;

    }

    public void ResetGame()
    {
        GameController.instance.Reset();
        //disable reset button
        GameObject.Find("ResetButton").gameObject.GetComponent<Button>().interactable = false;
        //enable start button
        GameObject.Find("StartButton").gameObject.GetComponent<Button>().interactable = true;
    }


}
