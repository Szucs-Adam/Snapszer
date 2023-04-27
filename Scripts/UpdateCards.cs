using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCards : MonoBehaviour
{

    public Sprite cardFace;
    public Sprite cardBack;

    private Logic logic;
    private SpriteRenderer spriteRenderer;
    private KartyaSpec kartyaSpec;
    private PlayerInput input;

    // Start is called before the first frame update
    void Start()
    {
        List<string> pakli = Logic.PakliGenerator();
        Logic logic = FindObjectOfType<Logic>();
        input = FindObjectOfType<PlayerInput>();

        int i = 0;
        foreach (string kartya in pakli)
        {
            if (this.name == kartya)
            {
                cardFace = logic.OsszesLap[i];
                break;
            }
            i++;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        kartyaSpec = GetComponent<KartyaSpec>();
    }

    // Update is called once per frame
    void Update()
    {
        if (kartyaSpec.faceUp == true)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }

        if(input.selectedCard)
        {
            if (name == input.selectedCard.name)
            {
                spriteRenderer.color = Color.green;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
        else if (input.playedCard)
        {
            if (name == input.playedCard.name) 
            {
                spriteRenderer.color = Color.green;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }

    }


}