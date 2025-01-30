using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamling : MonoBehaviour
{
    [SerializeField] private SymbolList symbols;
    [SerializeField] private int smallPotOdds = 4;
    [SerializeField] private SpriteRenderer slot1;
    [SerializeField] private SpriteRenderer slot2;
    [SerializeField] private SpriteRenderer slot3;

    [SerializeField] private List<Sprite> jackpotIcons;
    private Symbol winningSymbol;
    private List<Symbol> reverseList;

    void Start()
    {
        reverseList = new List<Symbol>(symbols.symbols);
        reverseList.Reverse();
    }

    public bool BigPot()
    {
        float number = Random.value;
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
            if (!winningSymbol.isJackpot)
            {
                slot1.sprite = winningSymbol.symbolIcon;
                slot2.sprite = winningSymbol.symbolIcon;
                slot3.sprite = winningSymbol.symbolIcon;
            }
            else
            {
                slot1.sprite = jackpotIcons[0];
                slot2.sprite = jackpotIcons[1];
                slot3.sprite = jackpotIcons[2];
            }
            Debug.Log("You won the big pot! " + winningSymbol.odds + " coins!");
        }
        else if (SmallPot())
        {
            if (!winningSymbol.isJackpot)
            {
                slot1.sprite = winningSymbol.symbolIcon;
                slot2.sprite = winningSymbol.symbolIcon;
                slot3.sprite = reverseList[Random.Range(0, reverseList.Count)].symbolIcon;
            }
            else
            {
                slot1.sprite = jackpotIcons[0];
                slot2.sprite = jackpotIcons[1];
                slot3.sprite = reverseList[Random.Range(0, reverseList.Count)].symbolIcon;
            }
            Debug.Log("You won the small pot! " + winningSymbol.odds + " coins!");
        }
        else
        {
            var randomList = new List<Symbol>();
            while (randomList.Count < 3)
            {
                var randomSymbol = reverseList[Random.Range(0, reverseList.Count)];
                if (!randomList.Contains(randomSymbol))
                {
                    randomList.Add(randomSymbol);
                }
            }
            if (randomList[0].isJackpot)
            {
                slot1.sprite = jackpotIcons[0];
            }
            else
            {
                slot1.sprite = randomList[0].symbolIcon;
            }
            if (randomList[1].isJackpot)
            {
                slot2.sprite = jackpotIcons[1];
            }
            else
            {
                slot2.sprite = randomList[1].symbolIcon;
            }
            if (randomList[2].isJackpot)
            {
                slot3.sprite = jackpotIcons[2];
            }
            else
            {
                slot3.sprite = randomList[2].symbolIcon;
            }
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
