using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    private Vector3 selectedCardOriginalPosition;

    public TextMeshProUGUI PlayerPont;
    public TextMeshProUGUI AiPont;
    public TextMeshProUGUI PlayerFogas;
    public TextMeshProUGUI AIFogas;
       
    public GameObject playedCard;
    public GameObject selectedCard;

    public GameObject EllenfelHivas;
    public GameObject JatekoslHivas;

    private Logic logic;
    private UpdateCards updateCards;
    private SpriteRenderer spriteRenderer2;
    private KartyaSpec KartyaSpec;

    public GameObject PlayerChoosenCard; // // Pozició 
    public GameObject PlayerPlayedCard; // // Pozició 
    public GameObject AIChoosenCard; // poziyio
    public GameObject AIPlayedCard; // poziyio

    private Vector3 PlayerChoosenCardPosition;
    private Vector3 AIChoosenCardPosition;

    public int PlayerScore;
    public int EnemyScore;

    public int PillanatnyiPontPlayer;
    public int PillanatnyiPontAI;

    public int JatekosFogas = 0;
    public int EllenfelFogas = 0;

    public bool EnemyFirst = false;

    public bool firstplayed = false;

    public bool PlayerTurn;

    public bool enemyHivott;

    private int kartyamaradtJatekos;
    private int kartyamaradtEllenfel;

    public int NeedfortheWin = 5;

    public int resetCounter = 0;

    void Start()
    {

        resetCounter = SaveManager.LoadInt("UjraOsztasSzamlalo");
        logic = FindObjectOfType<Logic>();
        KartyaSpec = FindObjectOfType<KartyaSpec>();
        spriteRenderer2 = FindObjectOfType<SpriteRenderer>();

        if (EnemyFirst == false)
        {
            selectedCard = this.gameObject;
            print("te kezdessz");
        }
        else
        {
            playedCard = this.gameObject;
            print("hivas");
            PlayerTurn = true;
            enemyHivott = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Jatek();

        PlayerPont.text = PlayerScore.ToString();
        AiPont.text = EnemyScore.ToString();
        AIFogas.text = "Ellenfel"+EllenfelFogas.ToString() + "/" + NeedfortheWin.ToString();
        PlayerFogas.text = "Játékos"+JatekosFogas.ToString() + "/" + NeedfortheWin.ToString();

        kartyamaradtJatekos = JatekosKartyainakSzama(logic.PlayerH);
        kartyamaradtEllenfel = JatekosKartyainakSzama(logic.EnemyH);

        if(kartyamaradtJatekos == 0)
        {
            JatekosFogas = JatekosFogas +  PlayerScore / 33;
            EllenfelFogas = EllenfelFogas + EnemyScore / 33;

            SaveManager.SaveInt("JatekosFogas", JatekosFogas);
            SaveManager.SaveInt("EllenfelFogas", EllenfelFogas);


            if (kartyamaradtJatekos == 0)
            {
                resetCounter++;
                Ujraosztas();
                SaveManager.SaveInt("UjraOsztasSzamlalo", resetCounter);
            }

        }

        JatekosFogas = SaveManager.LoadInt("JatekosFogas");
        EllenfelFogas = SaveManager.LoadInt("EllenfelFogas");

        if (JatekosFogas >= NeedfortheWin)
        {
            if (EllenfelFogas != JatekosFogas)
            { print("A játékos megnyerte a játékot");
                JatekosFogas = 0;
                EllenfelFogas = 0;
                resetCounter = 0;
            }
            else
            {
                NeedfortheWin = NeedfortheWin + 5;
            }
                
        }
        else if(EllenfelFogas >= NeedfortheWin)
        {
            if (JatekosFogas != EllenfelFogas)
            {
                print("Az ellenfél megnyerte a játékot");
                JatekosFogas = 0;
                EllenfelFogas = 0;
                resetCounter = 0;
            }
            else
            {
                NeedfortheWin = NeedfortheWin + 5;
            }
        }
    }
    // Day One
    
    void Ujraosztas()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Jatek()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            //// ----------------------------------------------------Az ellenfél kezdett------------------------------------------------
            if (hit)
            {
                if (hit.collider.CompareTag("PlayerHand") && PlayerTurn == true && EnemyFirst == true && enemyHivott == true)
                {
                    SelectCarIfValasz(hit.collider.gameObject);  // A játékos választ egy kártyát melyet kijelölük
                }
                else if (hit.collider.CompareTag("PlayerPlayed") && PlayerTurn == true && EnemyFirst == true && enemyHivott == true)
                {
                    if (Playable(selectedCard) == true)
                    {
                        ValszLepes(selectedCard);
                        PlayerTurn = false;

                        if (Osszehasonlit(AIPlayedCard, PlayerPlayedCard))
                        {
                            PillanatnyiPontAI = GetErtekOfCard(AIPlayedCard) + GetErtekOfCard(PlayerPlayedCard);

                            print("Az ellenfél nyert az ellenfél hivott");

                            Invoke("ResetAsztalIdore1", 0.5f);

                            Invoke("EllenfelKartyatHiv", 2f);


                            EnemyFirst = false;
                            PlayerTurn = true;
                        }
                        else
                        {

                            PillanatnyiPontPlayer = GetErtekOfCard(AIPlayedCard) + GetErtekOfCard(PlayerPlayedCard);

                            print("A Jatekos nyert az ellenfél hivott");
                            enemyHivott = false;
                            PlayerTurn = true;
                            EnemyFirst = false;

                            ResetAsztalIdore1();

                        }

                        EnemyScore = PillanatnyiPontAI + EnemyScore;
                        PlayerScore = PillanatnyiPontPlayer + PlayerScore;

                        PillanatnyiPontAI = 0;
                        PillanatnyiPontPlayer = 0;
                    }
                }
                //----------------------------------------------------ENEMY hivott--------------------------------------------------------------
                else if (hit.collider.CompareTag("PlayerHand") && PlayerTurn == true && EnemyFirst == false && enemyHivott == true)
                {
                    SelectCarIfValasz(hit.collider.gameObject);  // A játékos választ egy kártyát melyet kijelölük
                }
                else if (hit.collider.CompareTag("PlayerPlayed") && PlayerTurn == true && EnemyFirst == false && enemyHivott == true)
                {
                    if (Playable(selectedCard) == true)
                    {
                        ValszLepes(selectedCard);
                        PlayerTurn = false;

                        if (Osszehasonlit(AIPlayedCard, PlayerPlayedCard))
                        {
                            PillanatnyiPontAI = GetErtekOfCard(AIPlayedCard) + GetErtekOfCard(PlayerPlayedCard);

                            print("Az ellenfél nyert az ellenfél hivott");

                            Invoke("ResetAsztalIdore1", 0.5f);

                            Invoke("EllenfelKartyatHiv", 2f);

                            PlayerTurn = true;
                        }
                        else
                        {

                            PillanatnyiPontPlayer = GetErtekOfCard(AIPlayedCard) + GetErtekOfCard(PlayerPlayedCard);

                            print("A Jatekos nyert az ellenfél hivott");
                            enemyHivott = false;
                            PlayerTurn = true;

                            ResetAsztalIdore1();
                        }

                        EnemyScore = PillanatnyiPontAI + EnemyScore;
                        PlayerScore = PillanatnyiPontPlayer + PlayerScore;

                        PillanatnyiPontAI = 0;
                        PillanatnyiPontPlayer = 0;
                    }
                }
                // --------------------------------- A játékos hivott-----------------------------------------------------------------
                else if (hit.collider.CompareTag("PlayerHand") && PlayerTurn == true && EnemyFirst == false && enemyHivott == false && firstplayed == false)
                {
                    SelectCardIfHivas(hit.collider.gameObject);
                    print(playedCard);
                }
                else if (hit.collider.CompareTag("PlayerPlayed") && PlayerTurn == true  && EnemyFirst == false && enemyHivott == false && firstplayed == false )
                {
                    Hivas(playedCard);
                    PlayerTurn = false;
                    if (GetPlayableForAI(PlayerPlayedCard) != null)
                    {
                        AIPlayedCard = GetPlayableForAI(PlayerPlayedCard);
                        EllenfelValasz(AIPlayedCard);
                        print(AIPlayedCard);
                    }
                    else if(MegfeleloJeluKartya(PlayerPlayedCard) != null)
                    {
                        AIPlayedCard = MegfeleloJeluKartya(PlayerPlayedCard);
                        EllenfelValasz(AIPlayedCard);
                        print(AIPlayedCard);
                    }   
                    else
                    {
                        EllenfelValszHaNincsKijatszhato();
                        print(AIPlayedCard);
                    }

                    if (Osszehasonlit(PlayerPlayedCard, AIPlayedCard))
                    {

                        PillanatnyiPontPlayer = GetErtekOfCard(AIPlayedCard) + GetErtekOfCard(PlayerPlayedCard);

                        print("Az jatekos nyert és a jatekos hivott");
                        PlayerTurn = true;

                        Invoke("ResetAsztalIdore2", 1f);
                        //ResetAsztal(PlayerPlayedCard, AIPlayedCard);
                    }
                    else
                    {
                        PillanatnyiPontAI = GetErtekOfCard(AIPlayedCard) + GetErtekOfCard(PlayerPlayedCard);

                        print("Az ellenfel nyert a jatekos hivott");
                        PlayerTurn = false;
                        enemyHivott = true;

                        Invoke("ResetAsztalIdore2", 1f);
                        //ResetAsztal(PlayerPlayedCard, AIPlayedCard);


                        Invoke("EllenfelKartyatHiv", 2f);
                        Debug.Log("Az AI ezt hivna  " + AIPlayedCard);
                        Debug.Log("Az ellenfél hivott nagyot");
                    }
                    EnemyScore = PillanatnyiPontAI + EnemyScore;
                    PlayerScore = PillanatnyiPontPlayer + PlayerScore;
                    PillanatnyiPontAI = 0;
                    PillanatnyiPontPlayer = 0;

                }
                // --------------------------------- A játékos kezd-----------------------------------------------------------------
                else if (hit.collider.CompareTag("PlayerHand") && PlayerTurn == true && EnemyFirst == false && enemyHivott == false && firstplayed == true)
                {
                    SelectCardIfHivas(hit.collider.gameObject);
                    print(playedCard);
                }
                else if (hit.collider.CompareTag("PlayerPlayed") && PlayerTurn == true && EnemyFirst == false && enemyHivott == false && firstplayed == true)
                {
                    Hivas(playedCard);
                    PlayerTurn = false;
                    if (GetPlayableForAI(PlayerPlayedCard) != null)
                    {
                        AIPlayedCard = GetPlayableForAI(PlayerPlayedCard);
                        EllenfelValasz(AIPlayedCard);
                        print(AIPlayedCard);
                    }
                    else if (MegfeleloJeluKartya(PlayerPlayedCard) != null)
                    {
                        AIPlayedCard = MegfeleloJeluKartya(PlayerPlayedCard);
                        EllenfelValasz(AIPlayedCard);
                        print(AIPlayedCard);
                    }
                    else
                    {
                        EllenfelValszHaNincsKijatszhato();
                        print(AIPlayedCard);
                    }

                    if (Osszehasonlit(PlayerPlayedCard, AIPlayedCard))
                    {

                        PillanatnyiPontPlayer = GetErtekOfCard(AIPlayedCard) + GetErtekOfCard(PlayerPlayedCard);

                        print("Az jatekos nyert és a jatekos hivott");
                        PlayerTurn = true;

                        Invoke("ResetAsztalIdore2", 1f);
                    }
                    else
                    {
                        PillanatnyiPontAI = GetErtekOfCard(AIPlayedCard) + GetErtekOfCard(PlayerPlayedCard);

                        print("Az ellenfel nyert a jatekos hivott");
                        PlayerTurn = false;
                        enemyHivott = true;

                        Invoke("ResetAsztalIdore2", 1f);


                        Invoke("EllenfelKartyatHiv", 2f);
                        Debug.Log("Az AI ezt hivna  " + AIPlayedCard);
                        Debug.Log("Az ellenfél hivott nagyot");
                    }
                    EnemyScore = PillanatnyiPontAI + EnemyScore;
                    PlayerScore = PillanatnyiPontPlayer + PlayerScore;
                    PillanatnyiPontAI = 0;
                    PillanatnyiPontPlayer = 0;

                }
            }
        }
    }

    //-----------------------------------------------Játékos lépései--------------------------------------------

    void SelectCardIfHivas(GameObject card)
    {
        if (playedCard == this.gameObject)
        {
            playedCard = card;
            selectedCard = playedCard;
        }

        else if (playedCard != card)
        {
            playedCard = card;
            selectedCard = playedCard;
        }
    }
    void SelectCarIfValasz(GameObject card)
    {
        if (selectedCard == this.gameObject)
        {
            selectedCard = card;
        }

        else if (selectedCard != card)
        {
            selectedCard = card;
        }
    }

    void Hivas(GameObject card)
    {
        card = playedCard;
        PlayerPlayedCard = playedCard;
        print("hivni akarok");
        card.transform.position = new Vector3(PlayerChoosenCardPosition.x,
                        PlayerChoosenCardPosition.y - 1f, PlayerChoosenCardPosition.z + 1f);
        PlayerChoosenCard.SetActive(false);
        PlayerTurn = false;
        if (playedCard = this.gameObject)
        {
            spriteRenderer2.color = Color.white;
        }
    }

    void ValszLepes(GameObject card)
    {
        card = selectedCard;
        PlayerPlayedCard = selectedCard;
        card.transform.position = new Vector3(PlayerChoosenCardPosition.x,
                        PlayerChoosenCardPosition.y - 1f, PlayerChoosenCardPosition.z + 1f);
        PlayerChoosenCard.SetActive(false);
        print("valasz lepni akarok");
        PlayerTurn = false;
        if (selectedCard = this.gameObject)
        {
            spriteRenderer2.color = Color.white;
        }
    }

   //----------------------------------------ELLENFÉL LÉPÉSEI --------------------------------------------------------


    public void EnemyFirstMove()
    {
        AIChoosenCard.SetActive(false);
        AIPlayedCard = logic.GetHighestInEnemyHand();
        AIPlayedCard.transform.position = new Vector3(AIChoosenCardPosition.x,
                        AIChoosenCardPosition.y + 1f, AIChoosenCardPosition.z - 1f);

        AIPlayedCard.GetComponent<KartyaSpec>().faceUp = true;
        firstplayed = false;
        PlayerTurn = true;
    }


    public void EllenfelKartyatHiv()
    {
        AIPlayedCard = logic.GetHighestInEnemyHand();
        AIPlayedCard.transform.position = new Vector3(AIChoosenCardPosition.x,
                        AIChoosenCardPosition.y + 1f, AIChoosenCardPosition.z + 1f);

        AIPlayedCard.GetComponent<KartyaSpec>().faceUp = true;
        AIChoosenCard.SetActive(false);
        firstplayed = false;
        PlayerTurn = true;
    }

    public void EllenfelValasz(GameObject card)
    {
        AIChoosenCard.SetActive(false);
        AIPlayedCard = card;
        AIPlayedCard.transform.position = new Vector3(AIChoosenCardPosition.x,
                        AIChoosenCardPosition.y + 1f, AIChoosenCardPosition.z - 1f);

        AIPlayedCard.GetComponent<KartyaSpec>().faceUp = true;
        firstplayed = false;
        PlayerTurn = true;
    }

    public void EllenfelValszHaNincsKijatszhato()
    {
        AIChoosenCard.SetActive(false);
        AIPlayedCard = logic.GetLowestInEnemyHand();
        AIPlayedCard.transform.position = new Vector3(AIChoosenCardPosition.x,
                        AIChoosenCardPosition.y + 1f, AIChoosenCardPosition.z - 1f);

        AIPlayedCard.GetComponent<KartyaSpec>().faceUp = true;
        firstplayed = false;
        PlayerTurn = true;
    }

    // ----------------------------------------------- Játék lépé logikája------------------------------------------------

    public bool Playable(GameObject card)
    {
        KartyaSpec firstCard = AIPlayedCard.GetComponent<KartyaSpec>();
        KartyaSpec secondCard;

        if (selectedCard != null)
        {
            secondCard = selectedCard.GetComponent<KartyaSpec>();
        }
        else
        {
            secondCard = playedCard.GetComponent<KartyaSpec>();
        }

        if (firstplayed == true)
        {
            firstplayed = false;
            return true;
        }
        else if (firstplayed == false && ThereIsPlayableForThePlayer(AIPlayedCard))
        {

            if (secondCard.jel == firstCard.jel)
            {
                if (secondCard.ertek > firstCard.ertek)
                {
                    return true;
                }
                else if (secondCard.ertek < firstCard.ertek)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else if (!ThereIsPlayableForThePlayer(AIPlayedCard))
        {
            if (secondCard.jel == firstCard.jel)
            {
                return true;
            }
            else if (ThereIsJelInHand(AIPlayedCard))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    public bool ThereIsPlayableForThePlayer(GameObject playedCard)
    {
        int ertek = 0;
        string jel = "";
        string ertekString = "";

        KartyaSpec playedCardKartyaSpec = playedCard.GetComponent<KartyaSpec>();

        List<GameObject> hand = new List<GameObject>();

        for (int i = 0; i < logic.PlayerH.Length; i++)
        {
            foreach (Transform child in logic.PlayerH[i].transform)
            {
                if (child.CompareTag("PlayerHand"))
                {
                    hand.Add(child.gameObject);
                }
            }
        }
        for (int i = 0; i < hand.Count; i++)
        {
            jel = hand[i].name[0].ToString();
            for (int y = 1; y < hand[i].name.Length; y++)
            {
                char c = hand[i].name[y];
                ertekString = ertekString + c.ToString();
            }

            ertek = KartyaSpec.GetErtek(ertekString);
            ertekString = "";

            if (jel == playedCardKartyaSpec.jel)
            {
                if (ertek > playedCardKartyaSpec.ertek)
                {
                    return true;
                }
            }

        }
        
        return false;
    }

    public GameObject GetPlayableForAI(GameObject playedCard)
    {
        int ertek = 0;
        string jel = "";
        string ertekString = "";

        KartyaSpec playedCardKartyaSpec = playedCard.GetComponent<KartyaSpec>();

        List<GameObject> hand = new List<GameObject>();

        for (int i = 0; i < logic.EnemyH.Length; i++)
        {
            foreach (Transform child in logic.EnemyH[i].transform)
            {
                if (child.CompareTag("AIHand"))
                {
                    hand.Add(child.gameObject);
                }
            }
        }
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i] != null && hand[i].transform.parent != null)
            {
                jel = hand[i].name[0].ToString();
                for (int y = 1; y < hand[i].name.Length; y++)
                {
                    char c = hand[i].name[y];
                    ertekString = ertekString + c.ToString();
                }

                ertek = KartyaSpec.GetErtek(ertekString);
                ertekString = "";

                if (jel == playedCardKartyaSpec.jel)
                {
                    if (ertek > playedCardKartyaSpec.ertek)
                    {
                        return hand[i];
                    }
                }
            }
        }
        return null;
    }

    public GameObject MegfeleloJeluKartya(GameObject playedCard)
    {
        string jel = "";
        KartyaSpec playedCardKartyaSpec = playedCard.GetComponent<KartyaSpec>();
        List<GameObject> hand = new List<GameObject>();

        for (int i = 0; i < logic.EnemyH.Length; i++)
        {
            foreach (Transform child in logic.EnemyH[i].transform)
            {
                if (child.CompareTag("AIHand"))
                {
                    hand.Add(child.gameObject);
                }
            }
        }

        for (int i = 0; i < hand.Count; i++)
        {
            GameObject currentCard = hand[i];

            if (currentCard != null && currentCard.activeSelf)
            {
                jel = currentCard.name[0].ToString();

                if (jel == playedCardKartyaSpec.jel)
                {
                    return currentCard;
                }
            }
        }

        return null;
    }


    public bool ThereIsJelInHand(GameObject playedCard)
    {
        string jel = "";

        KartyaSpec playedCardKartyaSpec = playedCard.GetComponent<KartyaSpec>();

        List<GameObject> hand = new List<GameObject>();

        for (int i = 0; i < logic.PlayerH.Length; i++)
        {
            foreach (Transform child in logic.PlayerH[i].transform)
            {
                if (child.CompareTag("PlayerHand"))
                {
                    hand.Add(child.gameObject);
                }
            }
        }
        for (int i = 0; i < hand.Count; i++)
        {
            jel = hand[i].name[0].ToString();
            

            if (jel == playedCardKartyaSpec.jel)
            {
                return true;
            }

        }
        return false;
    }


    public bool Osszehasonlit(GameObject hivas, GameObject valasz)
    {
        KartyaSpec Hivo = hivas.GetComponent<KartyaSpec>();
        KartyaSpec Valasz = valasz.GetComponent<KartyaSpec>();
        if (Hivo.jel == Valasz.jel && Hivo.ertek > Valasz.ertek)
        {
            Debug.Log("a Hivo nyert");
            return true;
        } 
        else if(Hivo.jel != Valasz.jel)
        {
            print("a hivo nyert mert nem volt szine az ellenfelnek");
            return true;
        }

        return false;
    }

    void ResetAsztal(GameObject hivott, GameObject valasz)
    {
        hivott.SetActive(false);
        Destroy(hivott);
        valasz.SetActive(false);
        Destroy(valasz);
        AIChoosenCard.SetActive(true);
        PlayerChoosenCard.SetActive(true);
    }

    void ResetAsztalIdore1()
    {
        ResetAsztal(AIPlayedCard, PlayerPlayedCard);
    }

    void ResetAsztalIdore2()
    {
        ResetAsztal(PlayerPlayedCard, AIPlayedCard );
    }


    public int JatekosKartyainakSzama(GameObject[] parents)
    {
        int grandchildCount = 0;
        foreach (GameObject parent in parents)
        {
            foreach (Transform child in parent.transform)
            {
                    if (child != null)
                    {
                        grandchildCount++;
                    }
               
            }
        }
        return grandchildCount;
    }

    public int GetErtekOfCard(GameObject cardObject)
    {
        string ertekString = "";

        for (int y = 1; y < cardObject.name.Length; y++)
        {
            char c = cardObject.name[y];
            ertekString = ertekString + c.ToString();
        }

        return KartyaSpec.GetErtek(ertekString);
    }


    // -------------------------------------------Round Manager ------------------------------------ Ez felel a körökért illetve az ujraosztásná hogy megmaradjanak a már elert pontok
    public static class SaveManager
    {
        public static void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        public static int LoadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }
}


