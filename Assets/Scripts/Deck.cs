using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject jugador;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;

    public int[] values = new int[52];
    int cardIndex = 0;

    //------------------------------------------------------------
    public GameObject cartaNormal;
    public string[] nombres = new string[52];


    //-------------------------------------------------------------

    //-------------------------------------------------------------
    public Text puntosJugador;
    public Text puntosDealer;
    //-------------------------------------------------------------
    public List<GameObject> BarajaInicial = new List<GameObject>();
    public List<GameObject> BarajaAleatoria = new List<GameObject>();
    public List<GameObject> BarajaProbabilidades = new List<GameObject>();

    private void Awake()
    {
        InitCardValues();

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */

        //Asignar los valores de un palo
        int[] valoresPalo = new int[13];
        int valoresPaloIndice = 0;

        valoresPalo[0] = 11;
        for (int i = 1; i <= valoresPalo.Length - 1; i++)
        {
            if (i < 10)
            {
                valoresPalo[i] = i + 1;
            }
            else
            {
                valoresPalo[i] = 10;
            }

        }

        //Asgnar un valor a los elementos de values
        for (int i = 0; i <= values.Length - 1; i++)
        {
            values[i] = valoresPalo[valoresPaloIndice];

            valoresPaloIndice++;
            if (valoresPaloIndice == 13)
            {
                valoresPaloIndice = 0;
            }
        }

        //Baraja inicial como GameObject
        for (int i = 0; i <= faces.Length - 1; i++)
        {
            GameObject carta = Instantiate(cartaNormal);
            carta.name = nombres[i];
            carta.GetComponent<CardModel>().value = values[i];
            carta.GetComponent<CardModel>().front = faces[i];

            BarajaInicial.Add(carta);
        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */

        BarajaAleatoria.Clear();

        //Creación y copia de baraja en una baraja Auxiliar
        List<GameObject> BarajaAux = new List<GameObject>();
        foreach (GameObject carta in BarajaInicial)
        {
            BarajaAux.Add(carta);
        }

        //Baraja aleatoria
        for (int i = 0; i <= 51; i++)
        {
            int indiceAleatorio = Random.Range(0, BarajaAux.Count - 1);
            GameObject carta = BarajaAux[indiceAleatorio];
            BarajaAux.RemoveAt(indiceAleatorio);
            BarajaAleatoria.Add(carta);
        }

        //Baraja para calcular probabilidades
        foreach (GameObject carta in BarajaAleatoria)
        {
            BarajaProbabilidades.Add(carta);
        }
        BarajaProbabilidades.Add(BarajaAleatoria[1]);
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */

            //Hay 3 tres casos para al acabar una partida
            //Caso 1: Empatar al empezar partida
            if (dealer.GetComponent<CardHand>().points == 21 && jugador.GetComponent<CardHand>().points == 21)
            {
                //Mensaje de empate
                finalMessage.text = "Empate";
                finalMessage.color = Color.yellow;

                //Desactivar botones
                hitButton.interactable = false;
                stickButton.interactable = false;

                //Mostrar puntuación dealer
                puntosDealer.enabled = true;
                puntosDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
                dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

                //Apuesta
                this.gameObject.GetComponent<Bet>().empatarApuesta();
            }

            //Caso 2: Ganar si player tiene 21 y el dealer tiene menos de 21 al empezar partida
            else if (jugador.GetComponent<CardHand>().points == 21 || dealer.GetComponent<CardHand>().points > 21)
            {
                //Mensaje de victoria
                finalMessage.text = "Has ganado";
                finalMessage.color = Color.green;

                //Desactivar botones
                hitButton.interactable = false;
                stickButton.interactable = false;

                //Mostrar puntuación dealer
                puntosDealer.enabled = true;
                puntosDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
                dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

                //Apuesta
                this.gameObject.GetComponent<Bet>().ganarApuesta();
            }

            //Caso 3: Perder si dealer tiene 21 y el player tiene menos de 21 al empezar partida
            else if (dealer.GetComponent<CardHand>().points == 21 || jugador.GetComponent<CardHand>().points > 21)
            {
                //Mensaje de derrota
                finalMessage.text = "Has perdido";
                finalMessage.color = Color.red;

                //Desactivar botones
                hitButton.interactable = false;
                stickButton.interactable = false;

                //Mostrar puntuación dealer
                puntosDealer.enabled = true;
                puntosDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
                dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

                //Apuesta
                this.gameObject.GetComponent<Bet>().perderApuesta();
            }
        }
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */

        //PROBABILIDAD 1: Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador

        double cartasProb1 = 0;
        double Prob1 = 0;
        foreach (GameObject carta in BarajaProbabilidades)
        {
            if (carta.GetComponent<CardModel>().value + BarajaAleatoria[3].gameObject.GetComponent<CardModel>().value > jugador.gameObject.GetComponent<CardHand>().points)
            {
                cartasProb1++;
            }
        }
        Prob1 = (cartasProb1 / BarajaProbabilidades.Count) * 100;
        probMessage.text = "El dealer tiene más puntuación: " + string.Format("{0:0.00}", Prob1) + "% \n";

        //PROBABILIDAD 2: Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta

        double cartasProb2 = 0;
        double Prob2 = 0;
        foreach (GameObject carta in BarajaProbabilidades)
        {
            if (carta.GetComponent<CardModel>().value + jugador.gameObject.GetComponent<CardHand>().points <= 21 && carta.GetComponent<CardModel>().value + jugador.gameObject.GetComponent<CardHand>().points >= 17)
            {
                cartasProb2++;
            }
        }
        Prob2 = (cartasProb2 / BarajaProbabilidades.Count) * 100;
        probMessage.text += "Obtener entre 17 y 21: " + string.Format("{0:0.00}", Prob2) + "% \n";

        //PROBABILIDAD 3: Probabilidad de que el jugador obtenga más de 21 si pide una carta          

        double cartasProb3 = 0;
        double Prob3 = 0;
        foreach (GameObject carta in BarajaProbabilidades)
        {
            if (carta.GetComponent<CardModel>().value + jugador.gameObject.GetComponent<CardHand>().points > 21)
            {
                cartasProb3++;
            }
        }
        Prob3 = (cartasProb3 / BarajaProbabilidades.Count) * 100;
        probMessage.text += "Obtener mas de 21: " + string.Format("{0:0.00}", Prob3) + "%";
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        BarajaProbabilidades.Remove(BarajaAleatoria[cardIndex]);

        dealer.GetComponent<CardHand>().Push(BarajaAleatoria[cardIndex].GetComponent<CardModel>().front,
            BarajaAleatoria[cardIndex].GetComponent<CardModel>().value);

        cardIndex++;
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        BarajaProbabilidades.Remove(BarajaAleatoria[cardIndex]);

        jugador.GetComponent<CardHand>().Push(BarajaAleatoria[cardIndex].GetComponent<CardModel>().front,
           BarajaAleatoria[cardIndex].GetComponent<CardModel>().value);

        cardIndex++;
        CalculateProbabilities();
    }

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */

    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        jugador.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }

}
