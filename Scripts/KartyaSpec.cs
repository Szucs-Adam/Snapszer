using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartyaSpec : MonoBehaviour
{
    public bool faceUp = false;
    public bool inDeckPile = false;

    public bool played = false;

    public string jel;
    public int ertek;


    private string ertekString;


    void Start()
    {
        if (CompareTag("PlayerHand") || CompareTag("AIHand"))
        {
            jel = transform.name[0].ToString();

            for (int i = 1; i < transform.name.Length; i++)
            {
                char c = transform.name[i];
                ertekString = ertekString + c.ToString();
            }

            ertek = GetErtek(ertekString);
        }
    }

    void Update()
    {

    }
    // Day One
    public int GetErtek(string ertekString)
    {

        int ertek = 0;

        if (ertekString == "9")
        {
            ertek = 0;
        }

        if (ertekString == "10")
        {
            ertek = 10;
        }

        if (ertekString == "A")
        {
            ertek = 2;
        }

        if (ertekString == "F")
        {
            ertek = 3;
        }

        if (ertekString == "K")
        {
            ertek = 4;
        }
        if (ertekString == "Á")
        {
            ertek = 11;
        }

        return ertek;
    }


}
