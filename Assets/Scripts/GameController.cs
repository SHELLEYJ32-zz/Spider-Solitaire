using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    private List<CardController> stock;
    private ColumnController[] tableau;
    private List<CardController> movables;
    public CardController cardPrefab;

    void Awake()
    {

        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        CreateStock();
        CreatTableauPlaceholder();
    }

    void Update()
    {
        Vector3 columnInitPos = GameObject.Find("ColumnPlaceholder").GetComponent<Transform>().position;

        GameObject[] cards = GameObject.FindGameObjectsWithTag("card");
        for (int i = 0; i < 104; i++)
        {
            CardController card = cards[i].GetComponent<CardController>();
            if (card.myRow != -1 && card.myColumn != -1 && !card.finished)
            {
                card.transform.position = new Vector3(columnInitPos.x + 1.55f * card.myColumn, columnInitPos.y - 0.3f * (card.myRow - 1), columnInitPos.z - 0.1f * Mathf.Floor(card.myRow));
            }
            else if (card.finished)
            {
                Vector3 placeholderPos = GameObject.Find("FinishedPlaceholder").GetComponent<Transform>().position;
                if (card.num == 13)
                    card.transform.position = new Vector3(placeholderPos.x, placeholderPos.y, -1);
                else
                    card.transform.position = new Vector3(placeholderPos.x, placeholderPos.y, 0);

            }
        }

        for (int i = 0; i < 10; i++)
        {
            if (tableau[i].CheckComplete())
                tableau[i].Clear();
        }

        if (CheckEndGame())
            Reset();
    }

    void CreatTableauPlaceholder()
    {
        tableau = new ColumnController[10];
        Vector3 columnInitPos = GameObject.Find("ColumnPlaceholder").GetComponent<Transform>().position;
        for (int i = 0; i < 10; i++)
        {
            tableau[i] = new ColumnController();
            CardController placeholder = Instantiate(cardPrefab, new Vector3(columnInitPos.x + 1.55f * i, columnInitPos.y, 1), Quaternion.identity);
            placeholder.myColumn = i;
            placeholder.myRow = 0;
            placeholder.tag = "Untagged";
            placeholder.GetComponent<SpriteRenderer>().enabled = false;
            tableau[i].column.Add(placeholder);
        }

    }

    public void StartGame()
    {
        CreateTableau();
        GameObject.Find("StockPlaceholder").GetComponent<StockController>().isActive = true;
    }

    //Spider One Suit version with 8 sets of Spades
    List<CardController> CreateStock()
    {
        stock = new List<CardController>();
        Vector3 stockPos = GameObject.Find("StockPlaceholder").GetComponent<Transform>().position;
        Sprite[] spades = Resources.LoadAll<Sprite>("Sprites/playingCard");
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                CardController newCard = Instantiate(cardPrefab, new Vector3(stockPos.x, stockPos.y, 0), Quaternion.identity);
                newCard.suit = "spade";
                newCard.num = j + 1;
                newCard.frontSprite = spades[j + 1];
                stock.Add(newCard);
            }
        }

        Shuffle();
        return stock;
    }

    //shuffle, a reasonable amount of times
    void Shuffle()
    {

        for (int i = 0; i < 104; i++)
        {
            int randomIdx1 = UnityEngine.Random.Range(0, 104);
            int randomIdx2 = UnityEngine.Random.Range(0, 104);
            CardController randomCard1 = stock[randomIdx1];
            CardController randomCard2 = stock[randomIdx2];
            stock[randomIdx1] = randomCard2;
            stock[randomIdx2] = randomCard1;
        }
    }

    void CreateTableau()
    {
        //display 54 cards in the initial tableau
        for (int i = 0; i < 54; i++)
        {
            CardController newCard = stock[0];
            stock.RemoveAt(0);
            newCard.myColumn = i % 10;
            newCard.myRow = tableau[i % 10].column.Count;
            tableau[i % 10].column.Add(newCard);
            newCard.active |= i >= 44; //if i>=44

        }
    }

    public void AddNewRow()
    {
        //deselect everything
        for (int i = 0; i < tableau.Length; i++)
        {
            if (tableau[i].isMovingOut)
            {
                tableau[i].DisableMovables();
            }
        }
        //add a row
        for (int i = 0; i < 10; i++)
        {
            CardController newCard = stock[0];
            stock.RemoveAt(0);
            newCard.myColumn = i % 10;
            newCard.myRow = tableau[i % 10].column.Count;
            newCard.active = true;
            tableau[i % 10].column.Add(newCard);
        }

        if (stock.Count == 0)
        {
            GameObject.Find("StockPlaceholder").GetComponent<SpriteRenderer>().enabled = false;
            GameObject.Find("StockPlaceholder").GetComponent<BoxCollider2D>().enabled = false;
        }


    }

    public void ProcessClickOnCard(CardController newCard)
    {

        bool isAnyMovingOut = false;
        int prevCol = -1;
        for (int i = 0; i < tableau.Length; i++)
        {
            if (tableau[i].isMovingOut)
            {
                isAnyMovingOut = true;
                prevCol = i;
            }
        }

        if (!isAnyMovingOut || tableau[newCard.myColumn].isMovingOut)
        {
            if (tableau[newCard.myColumn].CheckMovables(newCard.myRow))
                movables = tableau[newCard.myColumn].PeekMovables();
        }
        else
        {
            if (tableau[newCard.myColumn].CheckNewOrder(movables[0]))
            {
                tableau[prevCol].DetachMovables();
                tableau[newCard.myColumn].AttachMovables(movables, newCard.myColumn);
            }

        }

    }

    bool CheckEndGame()
    {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("card");
        for (int i = 0; i < 104; i++)
        {
            CardController card = cards[i].GetComponent<CardController>();
            if (!card.finished)
                return false;
        }
        return true;
    }

    public void Reset()
    {
        //destroy all cards
        GameObject[] cards = GameObject.FindGameObjectsWithTag("card");
        for (int i = 0; i < 104; i++)
        {
            Destroy(cards[i]);
        }
        GameObject.Find("StockPlaceholder").GetComponent<SpriteRenderer>().enabled = true;
        GameObject.Find("StockPlaceholder").GetComponent<BoxCollider2D>().enabled = true;
        GameObject.Find("StockPlaceholder").GetComponent<StockController>().isActive = false;
        CreateStock();
        CreatTableauPlaceholder();
    }
}
