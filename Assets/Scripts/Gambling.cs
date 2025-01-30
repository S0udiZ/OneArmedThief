using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamling : MonoBehaviour
{
    [SerializeField] private List<Symbol> symbols;
    [SerializeField] private int smallPotOdds = 4;
    private Symbol winningSymbol;
    private List<Symbol> reverseList;

    void Start() {
        reverseList = new List<Symbol>(symbols);
        reverseList.Reverse();
    }

    public bool BigPot()
    {
        float number = Random.value;
        Debug.Log(number);
        foreach (Symbol symbol in reverseList)
        {
            if (number < 1.0f / symbol.odds)
            {
                winningSymbol = symbol;
                return true;
            }
        }
        return false;
    }

    public bool SmallPot()
    {
        float number = Random.value;
        Debug.Log(number);
        foreach (Symbol symbol in reverseList)
        {
            if (number < 1.0f / symbol.odds * smallPotOdds)
            {
                winningSymbol = symbol;
                return true;
            }
        }
        return false;
    }

    public void Roll()
    {
        if (BigPot())
        {
            Debug.Log("You won the big pot! " + winningSymbol.bigWin + " coins!");
        }
        else if (SmallPot())
        {
            Debug.Log("You won the small pot! " + winningSymbol.smallWin + " coins!");
        }
        else
        {
            Debug.Log("You lost!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Roll();
        }
    }
}
