using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Difficulty
{
    Easy,
    Hard
}

public class Logic : MonoBehaviour
{

    public Sprite[] OsszesLap;   // Kartya megjelenítéshez kell 

    public GameObject kartyaPref;
    public GameObject kartyaPrefAI;
    public GameObject[] EnemyH; 
    public GameObject[] PlayerH;
    public GameObject[] OutC; 

    public GameObject[] PlayerPlayedP; // // Pozició palyer
    public GameObject[] EnemyPlayedP; // // Pozició palyer

    public static string[] Jel = new string[] { "M", "P", "T", "Z" };
    public static string[] Ertek = new string[] { "9", "10", "A", "F", "K", "Á" };


    public List<string>[] AIHand = new List<string>[8];
    public List<string>[] Out = new List<string>[8];
    public List<string>[] PlayerHand = new List<string>[8];


    public List<string>[] PlayerPlayed = new List<string>[1];
    public List<string>[] EnemyPlayed = new List<string>[1];


    private GameObject AIChoosen;


    public List<string> Pakli;

    public List<string> Playerwon;
    public List<string> AIwon;

    public Difficulty difficulty;

    private PlayerInput input;
    private KartyaSpec KartyaSpec;

    private int osztas;

    public bool EllenfelHivottEloszor;


    void Start()
    {


        input = FindObjectOfType<PlayerInput>();
        KartyaSpec = FindObjectOfType<KartyaSpec>();

        for (int i = 0; i < 8; i++)
        {
            AIHand[i] = new List<string>();
            Out[i] = new List<string>();
            PlayerHand[i] = new List<string>();
        }

        osztas = input.resetCounter;

        Debug.Log(osztas + "- kör");

        PlayCards();


        if (osztas %2 == 1)
        {
            input.EnemyFirstMove();
            input.EnemyFirst = true;
        }
        else
        {

            input.EnemyFirst = false;
            input.PlayerTurn = true;
            input.enemyHivott = false;
            input.firstplayed = true;
        }
    }

    void Update()
    {
        
    }

    public void PlayCards()
    {
        Pakli = PakliGenerator();

        Keveres(Pakli);

        Osztas();

        KartyaKiosztás();
    }



    public static List<string> PakliGenerator()
    {

        List<string> ujPakli = new List<string>();
        foreach (string k in Ertek)
        {
            foreach (string p in Jel)
            {
                ujPakli.Add(p + k);
            }
        }

        return ujPakli;
    }

    void Keveres<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    void KartyaKiosztás()
    {


        for (int i = 0; i < 8; i++)
        {
            foreach (string kartya in AIHand[i])
            {
                GameObject newKartya = Instantiate(kartyaPrefAI, new Vector3(EnemyH[i].transform.position.x, EnemyH[i].transform.position.y,
                                                    EnemyH[i].transform.position.z), Quaternion.identity, EnemyH[i].transform);
                newKartya.name = kartya;
                newKartya.GetComponent<KartyaSpec>().faceUp = false;
                newKartya.GetComponent<KartyaSpec>().inDeckPile = true;


            }
        }

        for (int i = 0; i < 8; i++)
        {
            foreach (string kartya in PlayerHand[i])
            {
                GameObject newKartya = Instantiate(kartyaPref, new Vector3(PlayerH[i].transform.position.x, PlayerH[i].transform.position.y,
                                                    PlayerH[i].transform.position.z - 1), Quaternion.identity, PlayerH[i].transform);
                newKartya.name = kartya;
                newKartya.GetComponent<KartyaSpec>().faceUp = true;
                newKartya.GetComponent<KartyaSpec>().inDeckPile = true;


            }
        }

    }

    void Osztas()
    {
        SortCardstoEnemy();
        SortCardsOut();
        SortCardToPlayer();
    }

    void SortCardToPlayer()
    {
        for (int i = 0; i < 8; i++)
        {
            if (Pakli.Count > 0)
            {
                PlayerHand[i].Add(Pakli.Last<string>());
                Pakli.RemoveAt(Pakli.Count - 1);
            }
        }
    }

    void SortCardstoEnemy()
    {
        for (int i = 0; i < 8; i++)
        {
            if (Pakli.Count > 0)
            {
                AIHand[i].Add(Pakli.Last<string>());
                Pakli.RemoveAt(Pakli.Count - 1);
            }
        }
    }

    void SortCardsOut()
    {
        for (int i = 0; i < 8; i++)
        {
            if (Pakli.Count > 0)
            {
                Out[i].Add(Pakli.Last<string>());
                Pakli.RemoveAt(Pakli.Count - 1);
            }
        }
    }


    public GameObject GetARandom()
    {
        List<GameObject> aiHandObjects = new List<GameObject>();

        for (int i = 0; i < 8; i++)
    {
        foreach (Transform child in EnemyH[i].transform)
        {
            if (child.CompareTag("AIHand") && child != null)
            {
                aiHandObjects.Add(child.gameObject);
            }
        }
    }

        if (aiHandObjects.Count == 0)
        {
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, aiHandObjects.Count);
        return aiHandObjects[randomIndex];
    }


    public GameObject GetNewRandom()
    {
        List<GameObject> aiHandObjects = new List<GameObject>();

        GameObject[] aiHandObjs = GameObject.FindGameObjectsWithTag("AIHand");
        foreach (GameObject aiHandObj in aiHandObjs)
        {
            aiHandObjects.Add(aiHandObj);
        }

        if (aiHandObjects.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < aiHandObjects.Count; i++)
        {
            GameObject temp = aiHandObjects[i];
            int randomIndex = UnityEngine.Random.Range(i, aiHandObjects.Count);
            aiHandObjects[i] = aiHandObjects[randomIndex];
            aiHandObjects[randomIndex] = temp;
        }

        return aiHandObjects[0];
    }


    public GameObject GetHighestInEnemyHand()
    {
        int ertek = 0;
        int highestErtek = 0;

        string highestErtekString = "";
        string jel = "";
        string ertekString = "";


        GameObject highestCardObject = null;

        List<GameObject> aiHandObjects = new List<GameObject>();

        for (int i = 0; i < 8; i++)
        {
            foreach (Transform child in EnemyH[i].transform)
            {
                if (child.CompareTag("AIHand") && child != null)
                {
                    aiHandObjects.Add(child.gameObject);
                }
            }
        }

        for (int i = 0; i < aiHandObjects.Count; i++)
        {
            jel = aiHandObjects[i].name[0].ToString();

            for (int y = 1; y < aiHandObjects[i].name.Length; y++)
            {
                char c = aiHandObjects[i].name[y];
                ertekString = ertekString + c.ToString();
            }

            ertek = KartyaSpec.GetErtek(ertekString);

            ertekString = "";

            if (ertek > highestErtek)
            {

                highestErtek = ertek;
                highestErtekString = jel + ertekString;
                highestCardObject = aiHandObjects[i];
            }

        }
        return highestCardObject;
    }


    public GameObject GetLowestInEnemyHand()
    {
        int ertek = 0;
        int lowestErtek = 11;

        string lowestErtekString = "";
        string jel = "";
        string ertekString = "";

        GameObject lowestCardObject = null;

        List<GameObject> aiHandObjects = new List<GameObject>();

        for (int i = 0; i < 8; i++)
        {
            foreach (Transform child in EnemyH[i].transform)
            {
                if (child.CompareTag("AIHand") && child != null )
                {
                    aiHandObjects.Add(child.gameObject);
                }
            }
        }

        for (int i = 0; i < aiHandObjects.Count; i++)
        {
            jel = aiHandObjects[i].name[0].ToString();

            for (int y = 1; y < aiHandObjects[i].name.Length; y++)
            {
                char c = aiHandObjects[i].name[y];
                ertekString = ertekString + c.ToString();
            }

            ertek = KartyaSpec.GetErtek(ertekString);

            ertekString = "";

            if (ertek < lowestErtek)
            {
                lowestErtek = ertek;
                lowestErtekString = jel + ertekString;
                lowestCardObject = aiHandObjects[i];
            }
        }
        return lowestCardObject;
    }
}


