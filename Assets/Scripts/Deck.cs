using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;
    public Button apuesta10;
    public Button Allin;
    public Text apuestaMessage;
    public Text BancaMessage;
    int apuesta = 0;
    public int banca = 1000;

    public int[] values = new int[52];
    int cardIndex = 0;    
       
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

        //Una variable para llevar cuenta de los valores 
        int valores = 1;
        //Una variable para llevar cuenta de las cartas que han pasado, para resetear los valores
        int j = 1;

        for (int i = 0; i <= values.Length - 1; i++)
        {
            if (j == 1)
            {
                valores = 1;
            }          
            
            if ( comprobarReyes(j))
            {
                valores = 10;
                if(j == 13)
                {
                    j = 0;
                }
            }

            values[i] = valores;
            valores++;
            j++;
           
        }

        

    }

    //Comprobar si la carta a la que se le asigna valor de la realeza
    private bool comprobarReyes(int i)
    {
        if (i > 10 )
        {
            return true;
        }
        {
            return false;
        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */
        int valoresAux;
        Sprite carasAux;
        int nuevoIndice;
        for (int i = 0; i < values.Length; i++)
        {
           nuevoIndice = Random.Range(i, values.Length - 1);

           valoresAux = values[nuevoIndice];
           carasAux = faces[nuevoIndice];

            values[nuevoIndice] = values[i];
            faces[nuevoIndice] = faces[i];

            values[i] = valoresAux;
            faces[i] = carasAux;


        }
       
    }

 

    void StartGame()
    {
        apuestaMessage.text = 0.ToString();
        BancaMessage.text = banca.ToString();

        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
        }
        int puntuacionPlayer = player.GetComponent<CardHand>().points;
        Debug.Log("Player: " + puntuacionPlayer.ToString());
        int puntuacionDealer = dealer.GetComponent<CardHand>().points;
        Debug.Log("Dealer: " + puntuacionDealer.ToString());


      
        
        if(comprobarPuntuacionPlayer(puntuacionPlayer).Equals("W"))
        {
            finalMessage.text = "¡BLACKJACK!";
            hitButton.interactable = false;
            stickButton.interactable = false;
            banca += apuesta * 2;
            actualizarBanca();
            
        }

        if (puntuacionDealer == 21)
        {
            finalMessage.text = "¡EL CROUPIER TIENE BLACKJACK!";
            hitButton.interactable = false;
            stickButton.interactable = false;
            banca += 0;
            actualizarBanca();

        }
    }

    string comprobarPuntuacionPlayer(int puntosP)
    {
        if (puntosP == 21)
        {
           
            return "W";
        }
        else if (puntosP > 21)
        {
            
            return"L" ;
        }
        else
        {
            return "P";
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
        int numCartasP = player.GetComponent<CardHand>().cards.Count;
        int numCartasD = dealer.GetComponent<CardHand>().cards.Count;

        int[] puntuacionCartasPlayer = new int[10];
        int[] puntuacionCartasDealer = new int[10];
        
        for (int i = 0; i < numCartasP; i++)
        {
            puntuacionCartasPlayer[i] = player.GetComponent<CardHand>().cards[i].GetComponent<CardModel>().value;

        }

        for (int i = 0; i < numCartasD; i++)
        {
            puntuacionCartasPlayer[i] = dealer.GetComponent<CardHand>().cards[i].GetComponent<CardModel>().value;

        }

        if (numCartasP == 2)
        {
            calcularCartaOculta(puntuacionCartasDealer[0], puntuacionCartasPlayer);
        }

        probMessage.text = calcularRango(puntuacionCartasPlayer).ToString();
        
        calcularPerder(puntuacionCartasDealer, puntuacionCartasPlayer);
       



    }

    public float calcularCartaOculta(int puntuacionCartasDealer, int[] puntuacionCartasPlayer)
    {
        int valorTotalCartasP = 0;
        int cartasTotales = 48;
        for (int i = 0; i <= 2; i++)
        {
            valorTotalCartasP += puntuacionCartasPlayer[i];
        }

        int valorASuperar = valorTotalCartasP - puntuacionCartasDealer;
        int cartasProbables = 0;
        foreach (int value in values)
        {
            if (value > valorASuperar)
            {
                cartasProbables++;
            }
        }

        for(int i = 0; i <= 2; i++)
        {
            if(puntuacionCartasPlayer[i] > valorASuperar)
            {
                cartasProbables--;
            }
        }

        float probabilidad = (cartasProbables / cartasTotales) * 100;
        Debug.Log(probabilidad);
        return probabilidad;
    }

    public float calcularRango(int[] puntuacionCartasPlayer)
    {
        int numCartas = player.GetComponent<CardHand>().cards.Count;
        int valorTotalCartasP = 0;
        int cartasTotales = 52 - player.GetComponent<CardHand>().cards.Count;

        for (int i = 0; i < numCartas; i++)
        {
            valorTotalCartasP += puntuacionCartasPlayer[i];
        }

        int valorASuperar = 17 - valorTotalCartasP;
        int valorAIgualar = 21 - valorTotalCartasP;
        int cartasProbables = 0;

        if ( valorASuperar < 0)
        {
            valorASuperar = (valorASuperar * -1);
        }

        foreach (int value in values)
        {
            if (value >= valorASuperar)
            {
                if (value <= valorAIgualar)
                {
                    cartasProbables++;
                }
            }
        }



        for (int i = 0; i <= 2; i++)
        {
            if (puntuacionCartasPlayer[i] >= valorASuperar)
            {
                if (puntuacionCartasPlayer[i] <= valorAIgualar)
                {
                    cartasProbables--;
                }
            }
        }

        float probabilidad = cartasProbables / cartasTotales;
        return probabilidad;

    }

    public float calcularPerder(int[] puntuacionCartasDealer, int[] puntuacionCartasPlayer)
    {

        int valorTotalCartasP = 0;
        int cartasTotales = 52 - player.GetComponent<CardHand>().cards.Count;

        for (int i = 0; i <= 2; i++)
        {
            valorTotalCartasP += puntuacionCartasPlayer[i];
        }

        int valorASuperar = 21 - valorTotalCartasP;       
        int cartasProbables = 0;     
        foreach (int value in values)
        {
            if (value > valorASuperar)
            {              
               cartasProbables++;               
            }
        }

        for (int i = 0; i < player.GetComponent<CardHand>().cards.Count; i++)
        {
            if (puntuacionCartasPlayer[i] > valorASuperar)
            {               
                cartasProbables--;              
            }
        }

        float probabilidad = cartasProbables / cartasTotales;
        return probabilidad;

    }


    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
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
        int puntuacionPlayer = player.GetComponent<CardHand>().points;
        Debug.Log("Player" + puntuacionPlayer.ToString());


        if (comprobarPuntuacionPlayer(puntuacionPlayer).Equals( "W" ) )
        {
            finalMessage.text = "BLACKJACK";
            hitButton.interactable = false;
            stickButton.interactable = false;
            banca += apuesta * 2;
            actualizarBanca();
        }


        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (comprobarPuntuacionPlayer(puntuacionPlayer).Equals("L"))
        {
            finalMessage.text = "TE HAS PASADO DE 21";
            hitButton.interactable = false;
            stickButton.interactable = false;
            banca += 0;
            actualizarBanca();
        }
    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        int totalEnMano = player.GetComponent<CardHand>().cards.Count;
        if (totalEnMano == 2)
        {
            voltearCarta();
        }

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */       
        else
        {
            repartirDealer();
        }

        acabarRonda();
        hitButton.interactable = false;
        stickButton.interactable = false;
    }

    void voltearCarta()
    {
        GameObject primeraCarta = dealer.GetComponent<CardHand>().cards[0];
        primeraCarta.GetComponent<CardModel>().ToggleFace(true);
    }

    void repartirDealer()
    {
        int puntosTotales = dealer.GetComponent<CardHand>().points;
        if (sePlanta(puntosTotales) == false)
        {
            PushDealer();
        }
        else
        {
            return;
        }
    }

    bool sePlanta(int puntosD)
    {
        if(puntosD >= 17)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void acabarRonda()
    {
        int puntosP = player.GetComponent<CardHand>().points;
        int puntosD = dealer.GetComponent<CardHand>().points;
        if (puntosD < 22 && puntosP < 22)
        {
            if (puntosD > puntosP)
            {
                finalMessage.text = "El croupier gana con " + puntosD.ToString() + " puntos sobre tus " + puntosP.ToString();
                banca += 0;
                actualizarBanca();
            }
            else if (puntosP == puntosD)
            {
                finalMessage.text = "Habeis empatado los dos con " + puntosP.ToString();

            }
            else
            {
                finalMessage.text = "El jugador gana con " + puntosP.ToString() + " sobre los " + puntosD.ToString() + " del croupier";
                banca += apuesta * 2;
                actualizarBanca();
            }
        }
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }


    public void apostar10()
    {
        if (banca > 10)
        {

            apuesta += 10;
            banca -= 10;
            actualizarBanca();
        }
    }

    public void restar10()
    {
        if (banca > 10)
        {

            apuesta -= 10;
            banca += 10;
            actualizarBanca();
        }
    }

    public void allin()
    {
        if (banca > 10)
        {
            apuesta += banca;
            banca -= banca;
            actualizarBanca();
        }
    }

    private void actualizarBanca()
    {
        apuestaMessage.text = apuesta.ToString();
        BancaMessage.text = banca.ToString();
    }

}
