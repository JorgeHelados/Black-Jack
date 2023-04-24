using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bet : MonoBehaviour
{
    public double cartera;
    public double apuesta;
    public Button Plus50Button;
    public Button Plus100Button;
    public Button Plus1000Button;
    public Button Minus50Button;
    public Button Minus100Button;
    public Button Minus1000Button;
    public Button All_InButton;
    public Button ClearButton;
    public Button BetButton;
    public Button BetButtonActivado;
    public Text cartera_text;
    public Text apuesta_text;


    void Start()
    {
        //Establece 1000 euros al jugador
        cartera = 1000;
        apuesta = 0;

        //Deshabilitar botones del Juego
        this.gameObject.GetComponent<Deck>().hitButton.interactable = false;
        this.gameObject.GetComponent<Deck>().stickButton.interactable = false;
        BetButton.interactable = true;
    }

    void Update()
    {
        //Pasar los creditos a texto
        cartera_text.text = cartera.ToString();
        apuesta_text.text = apuesta.ToString();
        //Cuando acabe la partida que boton de apostar se active
        if (!BetButton.IsInteractable())
        {
            BetButton.interactable = true;
        }
    }

    public void anyadirApuestaCincuenta()
    {
        if (cartera >= apuesta + 50)
        {
            apuesta = apuesta + 50;
        }
    }
    public void anyadirApuestaCien()
    {
        if (cartera >= apuesta + 100)
        {
            apuesta = apuesta + 100;
        }
    }
    public void anyadirApuestaMil()
    {
        if (cartera >= apuesta + 1000)
        {
            apuesta = apuesta + 1000;
        }
    }
    public void apostarTodo()
    {
        apuesta = cartera;
    }
    public void eliminarApuestaCincuenta()
    {
        if (apuesta >= 50)
        {
            apuesta = apuesta - 50;
        }
    }
    public void eliminarApuestaCien()
    {
        if (apuesta >= 100)
        {
            apuesta = apuesta - 100;
        }
    }
    public void eliminarApuestaMil()
    {
        if (apuesta >= 1000)
        {
            apuesta = apuesta - 1000;
        }
    }
    public void borrarApuesta()
    {
        apuesta = 0;
    }
    public void apostarAndEmpezar()
    {

        //Habilitar botones del Juego
        this.gameObject.GetComponent<Deck>().hitButton.interactable = true;
        this.gameObject.GetComponent<Deck>().stickButton.interactable = true;

        //Barajar y empezar juego
        this.gameObject.GetComponent<Deck>().ShuffleCards();
        this.gameObject.GetComponent<Deck>().StartGame();

        //Deshabilitar botones de Apuestas
        desactivarBotonesApostar();
    }
    public void ganarApuesta()
    {
        cartera = cartera + (2 * apuesta);
        apuesta = 0;
    }
    public void perderApuesta()
    {
        cartera = cartera - apuesta;
        apuesta = 0;
    }
    public void empatarApuesta()
    {
        apuesta = 0;
    }
    public void desactivarBotonesApostar()
    {
        Plus50Button.interactable = false;
        Plus100Button.interactable = false;
        Plus1000Button.interactable = false;
        Minus50Button.interactable = false;
        Minus100Button.interactable = false;
        Minus1000Button.interactable = false;
        ClearButton.interactable = false;
        All_InButton.interactable = false;
        BetButton.gameObject.SetActive(false);
        BetButtonActivado.gameObject.SetActive(true);
    }
    public void activarBotonesApostar()
    {
        BetButton.interactable = true;
        Plus50Button.interactable = true;
        Plus100Button.interactable = true;
        Plus1000Button.interactable = true;
        Minus50Button.interactable = true;
        Minus100Button.interactable = true;
        Minus1000Button.interactable = true;
        ClearButton.interactable = true;
        All_InButton.interactable = true;
        BetButton.gameObject.SetActive(true);
        BetButtonActivado.gameObject.SetActive(false);
    }
 
}