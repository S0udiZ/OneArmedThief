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
    private bool rolling = false;

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
        rolling = false;
    }

    public IEnumerator Roll()
    {
        // Laver de sidste 3 symboler usynlige ved at fjerne deres sprites
        slot1.sprite = null;
        slot2.sprite = null;
        slot3.sprite = null;

        // for hver animation i animations listen, sætter vi deres speed til 1 og gør dem synlige
        foreach (Animator animator in animations)
        {
            animator.speed = 1;
            animator.GetComponent<SpriteRenderer>().enabled = true;
        }

        // venter 3 sekunder
        yield return new WaitForSeconds(3);

        // Roller maskinen først med BigPot, hvis den ikke rammer noget så roller den smallPot, hvis den ikke rammer noget så roller den random
        // Derefter sætter den de 3 symboler i maskinen og viser dem og beløbet man har vundet
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
            // En lille funktion der vælger 3 forskellige symboler der ikke kan være de samme, så man ikke vinder noget
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
        if (Input.GetAxis("Jump") > 0 && !rolling)
        {
            rolling = true;
            StartCoroutine(Roll());
        }
    }
}
