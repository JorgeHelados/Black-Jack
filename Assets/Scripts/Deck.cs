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

    public GameObject cartaNormal;
    public string[] nombres = new string[52];

    public Text puntosJugador;
    public Text puntosDealer;

    public List<GameObject> BarajaInicial = new List<GameObject>();
    public List<GameObject> BarajaAleatoria = new List<GameObject>();
    public List<GameObject> BarajaProbabilidades = new List<GameObject>();

    //-----------------------------------------------------------------------

    private void Awake()
    {
        InitCardValues();

    }

    private void Update()
    {
        puntosJugador.text = "Puntuación: " + jugador.GetComponent<CardHand>().points.ToString();
    }

    private void InitCardValues()
    {


        //Asignar los valores de un palo
        int[] valoresPalo = new int[13];
        int valoresPaloIndice = 0;

        valoresPalo[0] = 11;
        for (int i = 1; i <= valoresPalo.Length - 1; i++) //asignar valor a las cartas
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

        //Pasar cartas a GameObjects
        for (int i = 0; i <= faces.Length - 1; i++)
        {
            GameObject carta = Instantiate(cartaNormal);
            carta.name = nombres[i];
            carta.GetComponent<CardModel>().value = values[i];
            carta.GetComponent<CardModel>().front = faces[i];

            BarajaInicial.Add(carta);
        }
    }

    public void ShuffleCards()
    {
        BarajaAleatoria.Clear();

        //Creación y copia de baraja en una baraja Auxiliar
        List<GameObject> BarajaAux = new List<GameObject>();    //Crea baraja auxiliar
        foreach (GameObject carta in BarajaInicial)
        {
            BarajaAux.Add(carta);   //Copia baraja inicial
        }

        //Baraja aleatoria
        for (int i = 0; i <= 51; i++)
        {
            int indiceAleatorio = Random.Range(0, BarajaAux.Count - 1); //elige  un numero aleatoria entre 1-51
            GameObject carta = BarajaAux[indiceAleatorio];  //elige la carta aleatoria
            BarajaAux.RemoveAt(indiceAleatorio);    //Saca la carta de la baraja auxiliar
            BarajaAleatoria.Add(carta); //La mete en la baraja 
        }

        //Baraja para calcular probabilidades
        foreach (GameObject carta in BarajaAleatoria)
        {
            BarajaProbabilidades.Add(carta);
        }
        BarajaProbabilidades.Add(BarajaAleatoria[1]);
    }

    public void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            //Dar cartas
            PushPlayer();
            PushDealer();


            //------------------------------Empatar al empezar partida-------------------------------------------
            if (dealer.GetComponent<CardHand>().points == 21 && jugador.GetComponent<CardHand>().points == 21)
            {
                finalMessage.text = "Empate";   //mensaje de empate
                finalMessage.color = Color.yellow;
                //Desactivar botones de plantar y pedir
                hitButton.interactable = false;
                stickButton.interactable = false;
                //Mostrar puntuación dealer
                puntosDealer.enabled = true;
                puntosDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
                dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);    //ver cartas ocultas de Dealer
                //Dar resultado de la apuesta
                this.gameObject.GetComponent<Bet>().empatarApuesta();
            }

            //----------Ganar si player tiene 21 y el dealer tiene menos de 21 al empezar partida----------------
            else if (jugador.GetComponent<CardHand>().points == 21 || dealer.GetComponent<CardHand>().points > 21)
            {
                finalMessage.text = "Has ganado";   //Mensaje de victoria
                finalMessage.color = Color.green;
                //Desactivar botones de plantar y pedir
                hitButton.interactable = false;
                stickButton.interactable = false;
                //Mostrar puntuación dealer
                puntosDealer.enabled = true;
                puntosDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
                dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);    //ver cartas ocultas de Dealer
                //Dar resultado de la apuesta
                this.gameObject.GetComponent<Bet>().ganarApuesta();
            }

            //Perder si dealer tiene 21 y el player tiene menos de 21 al empezar partida
            else if (dealer.GetComponent<CardHand>().points == 21 || jugador.GetComponent<CardHand>().points > 21)
            {
                //Mensaje de derrota
                finalMessage.text = "Has perdido";
                finalMessage.color = Color.red;
                //Desactivar botones de plantar y pedir
                hitButton.interactable = false;
                stickButton.interactable = false;
                //Mostrar puntuación dealer
                puntosDealer.enabled = true;
                puntosDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
                dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);    //ver cartas ocultas de Dealer
                //Dar resultado de la apuesta
                this.gameObject.GetComponent<Bet>().perderApuesta();
            }
        }
    }

    private void CalculateProbabilities()
    {
        //-----------------------------Probabilidad de que el dealer tenga más puntuación que el jugador----------------------------------------
        double cartasProb1 = 0;
        double Prob1 = 0;
        foreach (GameObject carta in BarajaProbabilidades)  //Valor carta + valor carta aleatoria > puntuacion actual
        {
            if (carta.GetComponent<CardModel>().value + BarajaAleatoria[3].gameObject.GetComponent<CardModel>().value > jugador.gameObject.GetComponent<CardHand>().points)
            {
                cartasProb1++;
            }
        }
        //Calcular y escribir porcentaje
        Prob1 = (cartasProb1 / BarajaProbabilidades.Count) * 100;
        probMessage.text = "El dealer tiene más puntuación: " + string.Format("{0:0.00}", Prob1) + "% \n";

        //-----------------------------Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta--------------------------------
        double cartasProb2 = 0;
        double Prob2 = 0;
        foreach (GameObject carta in BarajaProbabilidades)  //Valo carta + puntos del jugador <= 21 y Valo carta + puntos del jugador >= 17
        {
            if (carta.GetComponent<CardModel>().value + jugador.gameObject.GetComponent<CardHand>().points <= 21 && carta.GetComponent<CardModel>().value + jugador.gameObject.GetComponent<CardHand>().points >= 17)
            {
                cartasProb2++;
            }
        }
        //Calcular y escribir porcentaje
        Prob2 = (cartasProb2 / BarajaProbabilidades.Count) * 100;
        probMessage.text += "Obtener entre 17 y 21: " + string.Format("{0:0.00}", Prob2) + "% \n";

        //-----------------------------Probabilidad de que el jugador obtenga más de 21 si pide una carta---------------------------------------------        
        double cartasProb3 = 0;
        double Prob3 = 0;
        foreach (GameObject carta in BarajaProbabilidades)  //Valor carta + puntos del jugador > 21
        {
            if (carta.GetComponent<CardModel>().value + jugador.gameObject.GetComponent<CardHand>().points > 21)
            {
                cartasProb3++;
            }
        }
        //Calcular y escribir porcentaje
        Prob3 = (cartasProb3 / BarajaProbabilidades.Count) * 100;
        probMessage.text += "Obtener mas de 21: " + string.Format("{0:0.00}", Prob3) + "%";
    }

    void PushDealer()
    {
        //Vaciar la baraja que calcula probabilidad
        BarajaProbabilidades.Remove(BarajaAleatoria[cardIndex]);
        //Dar cartas
        dealer.GetComponent<CardHand>().Push(BarajaAleatoria[cardIndex].GetComponent<CardModel>().front,
            BarajaAleatoria[cardIndex].GetComponent<CardModel>().value);
        //suma index de la baraja en uso
        cardIndex++;
    }

    void PushPlayer()
    {
        //Vaciar la baraja que calcula probabilidad
        BarajaProbabilidades.Remove(BarajaAleatoria[cardIndex]);
        //Dar cartas
        jugador.GetComponent<CardHand>().Push(BarajaAleatoria[cardIndex].GetComponent<CardModel>().front,
           BarajaAleatoria[cardIndex].GetComponent<CardModel>().value);
        //suma index de la baraja en uso
        cardIndex++;
        CalculateProbabilities();
    }

    public void Hit()
    {
        //Repartimos carta al jugador
        PushPlayer();
        //--------------------------------Perder si player tiene mas de 21 al pedir una carta---------------------------------------------------
        if (jugador.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "Has perdido";  //Mensaje de derrota
            finalMessage.color = Color.red;
            //Desactivar botones de pedir y quedarse
            hitButton.interactable = false;
            stickButton.interactable = false;
            //Mostrar puntuación dealer
            puntosDealer.enabled = true;
            puntosDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
            //Dar apuesta
            this.gameObject.GetComponent<Bet>().perderApuesta();
        }

        //-----------------------------------Ganar si player tiene 21 al pedir una carta--------------------------------------------
        else if (jugador.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "Has ganado";   //Mensaje de victoria
            finalMessage.color = Color.green;
            //Desactivar botones
            hitButton.interactable = false;
            stickButton.interactable = false;
            //Mostrar puntuación dealer
            puntosDealer.enabled = true;
            puntosDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
            //Dar apuesta
            this.gameObject.GetComponent<Bet>().ganarApuesta();
        }

    }

    public void Stand()
    {
        //Desactivar botones
        hitButton.interactable = false;
        stickButton.interactable = false;
        //Girar 1ª carta del dealer
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
        //Si tiene menos de 17 el dealer pide cartas
        while (dealer.GetComponent<CardHand>().points < 17)
        {
            PushDealer();
        }
        //--------------------------------Empatar cuando el dealer y player tienen la misma puntuación----------------------------------------
        if (dealer.GetComponent<CardHand>().points == jugador.GetComponent<CardHand>().points)
        {
            finalMessage.text = "Empate";   //Mensaje de empate
            finalMessage.color = Color.yellow;
            //Desactivar botones
            hitButton.interactable = false;
            stickButton.interactable = false;
            //Mostrar puntuación dealer
            puntosDealer.enabled = true;
            puntosDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
            // Dar apuesta
            this.gameObject.GetComponent<Bet>().empatarApuesta();
        }

        //-----------------------------------------Ganar cuando el dealer se ha pasado de 21--------------------------------------------------------
        else if (dealer.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "Has ganado";   //Mensaje de victoria
            finalMessage.color = Color.green;
            //Desactivar botones
            hitButton.interactable = false;
            stickButton.interactable = false;
            //Mostrar puntuación dealer
            puntosDealer.enabled = true;
            puntosDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
            //Dar apuesta
            this.gameObject.GetComponent<Bet>().ganarApuesta();
        }

        //------------------------Ganar cuando la puntuacion del player es mayor a la del dealer al plantarse----------------------------------------
        else if (jugador.GetComponent<CardHand>().points > dealer.GetComponent<CardHand>().points)
        {
            finalMessage.text = "Has ganado";   //Mensaje de victoria
            finalMessage.color = Color.green;
            //Desactivar botones
            hitButton.interactable = false;
            stickButton.interactable = false;
            //Mostrar puntuación dealer
            puntosDealer.enabled = true;
            puntosDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
            //Dar apuesta
            this.gameObject.GetComponent<Bet>().ganarApuesta();
        }

        //-----------------------------Perder cuando la puntuacion del player es menor a la del dealer al plantarse---------------------------------
        else if (jugador.GetComponent<CardHand>().points < dealer.GetComponent<CardHand>().points)
        {
            finalMessage.text = "Has perdido";  //Mensaje de derrota
            finalMessage.color = Color.red;
            //Desactivar botones
            hitButton.interactable = false;
            stickButton.interactable = false;
            //Mostrar puntuación dealer
            puntosDealer.enabled = true;
            puntosDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
            //Dar apuesta
            this.gameObject.GetComponent<Bet>().perderApuesta();
        }
    }

    public void PlayAgain()
    {
        //Botones Interactuables
        hitButton.interactable = false;
        stickButton.interactable = false;

        //Reseteos
        finalMessage.text = "";
        jugador.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        BarajaProbabilidades.Clear();
        probMessage.text = "";
        cardIndex = 0;
        puntosDealer.enabled = false;
        //Activar Botones de Apuesta
        this.gameObject.GetComponent<Bet>().activarBotonesApostar();
    }

}
