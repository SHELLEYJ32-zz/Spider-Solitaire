using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnController : MonoBehaviour
{
    public List<CardController> column = new List<CardController>();
    public int selectedIdx = -1;
    public bool isMovingOut;


    //return true if click is valid on the same column
    public bool CheckMovables(int idx)
    {
        if (idx != 0) //exclude placeholder
        {
            if (isMovingOut)
            {
                for (int i = selectedIdx; i < column.Count; i++)
                {
                    column[i].transform.localScale /= 1.1f;
                }
            }

            if (selectedIdx != idx)
            {
                isMovingOut = true;
                if (idx + 1 != column.Count)
                {
                    for (int i = idx; i < column.Count - 1; i++)
                    {
                        if (column[i + 1].num != column[i].num - 1)
                            isMovingOut = false;
                    }
                }

                if (isMovingOut)
                {
                    for (int i = idx; i < column.Count; i++)
                    {
                        column[i].transform.localScale *= 1.1f;
                    }
                    selectedIdx = idx;
                }
            }
            else
            {
                isMovingOut = false;
                selectedIdx = -1;
            }
        }
        return isMovingOut;
    }

    public void DisableMovables()
    {
        if (isMovingOut)
        {
            for (int i = selectedIdx; i < column.Count; i++)
            {
                column[i].transform.localScale /= 1.1f;
            }

            isMovingOut = false;
            selectedIdx = -1;
        }
    }

    public List<CardController> PeekMovables()
    {
        List<CardController> tmp = new List<CardController>();
        for (int i = selectedIdx; i < column.Count; i++)
        {
            tmp.Add(column[i]);
        }
        return tmp;
    }

    public bool CheckNewOrder(CardController topCard)
    {
        if (column.Count == 1)
            return true;
        return column[column.Count - 1].num == topCard.num + 1;
    }

    public void DetachMovables()
    {
        if (column.Count > 1)
        {
            for (int i = column.Count - 1; i >= selectedIdx; i--)
            {
                column.RemoveAt(i);
            }
        }
        isMovingOut = false;
        selectedIdx = -1;
        if (!column[column.Count - 1].active)
            column[column.Count - 1].active = true;

    }

    public void AttachMovables(List<CardController> movables, int col)
    {
        for (int i = 0; i < movables.Count; i++)
        {

            movables[i].myRow = column.Count;
            movables[i].myColumn = col;
            movables[i].transform.localScale /= 1.1f;
            column.Add(movables[i]);
        }

    }

    public bool CheckComplete()
    {
        //find card 13
        int idx13 = -1;
        for (int i = 1; i < column.Count; i++)
        {
            if (column[i].active && column[i].num == 13)
            {
                idx13 = i;
            }
        }
        //if there's 13
        if (idx13 != -1)
        {
            //if there's enough cards
            if (column.Count - idx13 == 13)
            {
                //check in order
                for (int i = idx13; i < column.Count - 1; i++)
                {
                    if (column[i + 1].num != column[i].num - 1)
                        return false;
                }
                return true;
            }
            return false;
        }
        return false;
    }

    public void Clear()
    {
        //find card 13
        int idx13 = -1;
        for (int i = 1; i < column.Count; i++)
        {
            if (column[i].active && column[i].num == 13)
            {
                idx13 = i;
            }
        }
        for (int i = column.Count - 1; i >= idx13; i--)
        {
            column[i].finished = true;
            column[i].myRow = -1;
            column[i].myColumn = -1;
            column.RemoveAt(i);
        }
        if (!column[column.Count - 1].active)
            column[column.Count - 1].active = true;
    }

}
