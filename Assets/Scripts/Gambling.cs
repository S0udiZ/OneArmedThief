using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class Gamling : MonoBehaviour
{
    [SerializeField] private SymbolList symbols;
    [SerializeField] private int smallPotOdds = 4;
    [SerializeField] private SpriteRenderer slot1;
    [SerializeField] private SpriteRenderer slot2;
    [SerializeField] private SpriteRenderer slot3;
    [SerializeField] private List<Sprite> jackpotIcons;
    [SerializeField] private TextMeshProUGUI text;
    private Symbol winningSymbol;
    private List<Symbol> reverseList;

    [SerializeField] private List<Animator> animations;

    void Start()
    {
        reverseList = new List<Symbol>(symbols.symbols);
        reverseList.Reverse();
        text.text = "0";
        foreach (Animator animator in animations)
        {
            animator.speed = 0;
            animator.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public bool BigPot()
    {
        float number = Random.value;
        foreach (Symbol symbol in reverseList)
        {
            if (number <= 1.0f / symbol.odds)
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
            if (number <= 1.0f / symbol.odds * smallPotOdds)
            {
                winningSymbol = symbol;
                return true;
            }
        }
        return false;
    }

    public IEnumerator SetRolls(Sprite sprite1, Sprite sprite2, Sprite sprite3, int winning)
    {
        var spriteSetList = new List<SpriteRenderer> { slot1, slot2, slot3 };
        var spriteList = new List<Sprite> { sprite1, sprite2, sprite3 };

        foreach (SpriteRenderer sprite in spriteSetList)
        {
            var index = spriteSetList.IndexOf(sprite);
            var animation = animations[index];
            animation.speed = 0;
            animation.GetComponent<SpriteRenderer>().enabled = false;
            sprite.sprite = spriteList[index];
            yield return new WaitForSeconds(0.2f);
        }
        text.text = winning.ToString();
    }

    public IEnumerator Roll()
    {
        // Resets the slots to the default state
        slot1.sprite = null;
        slot2.sprite = null;
        slot3.sprite = null;
        foreach (Animator animator in animations)
        {
            animator.speed = 1;
            animator.GetComponent<SpriteRenderer>().enabled = true;
        }
        yield return new WaitForSeconds(3);
        if (BigPot())
        {
            if (!winningSymbol.isJackpot)
            {
                StartCoroutine(SetRolls(winningSymbol.symbolIcon, winningSymbol.symbolIcon, winningSymbol.symbolIcon, winningSymbol.bigWin));
            }
            else
            {
                StartCoroutine(SetRolls(jackpotIcons[0], jackpotIcons[1], jackpotIcons[2], winningSymbol.bigWin));
            }
        }
        else if (SmallPot())
        {
            if (!winningSymbol.isJackpot)
            {
                var randomSprite = winningSymbol.symbolIcon;
                while (randomSprite == winningSymbol.symbolIcon)
                {
                    randomSprite = reverseList[Random.Range(0, reverseList.Count)].symbolIcon;
                }
                StartCoroutine(SetRolls(winningSymbol.symbolIcon, winningSymbol.symbolIcon, randomSprite, winningSymbol.smallWin));
            }
            else
            {
                var randomSprite = winningSymbol.symbolIcon;
                while (randomSprite == winningSymbol.symbolIcon)
                {
                    randomSprite = reverseList[Random.Range(0, reverseList.Count)].symbolIcon;
                }
                StartCoroutine(SetRolls(jackpotIcons[0], jackpotIcons[1], randomSprite, winningSymbol.smallWin));
            }
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
            var correctSymbolList = new List<Sprite>();
            foreach (Symbol symbol in randomList)
            {
                if (!symbol.isJackpot)
                {
                    correctSymbolList.Add(symbol.symbolIcon);
                }
                else
                {
                    var index = randomList.IndexOf(symbol);
                    correctSymbolList.Add(jackpotIcons[index]);
                }
            }
            StartCoroutine(SetRolls(correctSymbolList[0], correctSymbolList[1], correctSymbolList[2], 0));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Roll());
        }
    }
}
