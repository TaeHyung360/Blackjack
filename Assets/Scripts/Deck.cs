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

        for(int Npalo = 0; Npalo < 4; Npalo++){

            for(int carta = 0; carta < 13; carta++){

                if(carta < 10){

                    values[13*Npalo+carta] = carta+1;

                }else{
                    
                    values[13*Npalo+carta] = 10;

                }

            }

        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */

        for (int i = 0; i < 51; i++)
        {
            int posicion = Random.Range(0, 52);
            Sprite fAux = faces[i];
            int vAux = values[i];

            faces[i] = faces[posicion];
            values[i] = values[posicion];

            faces[posicion] = fAux;
            values[posicion] = vAux;
        }

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

            int puntosDealer = values[2] + values[4];

            int puntosJugador = values[0] + values[3];

            if(puntosDealer == 21){

                Debug.Log("Gana Dealer");

            }else if(puntosJugador == 21){

                Debug.Log("Gana Jugador");

            }else if(puntosJugador == 21 && puntosDealer == 21){

                Debug.Log("Empate");

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
        float casosTotales = 49;

        float cantidadSuperarDealer = player.GetComponent<CardHand>().points - values[1]; //Cada vez que cojo carta obtengo la nueva

        float cantidadSuperarJugador = player.GetComponent<CardHand>().points;

        float casoFavDealer = 0;

        float casoFavJugador = 0;

        float casoFavPerder = 0;

        if (values[1] > cantidadSuperarDealer) {

            casoFavDealer++;

        }

        if (values[1] + cantidadSuperarJugador <= 21 && values[1] + cantidadSuperarJugador >= 17) {

            casoFavJugador++;

        }

        if (values[1] + cantidadSuperarJugador > 21) {

            casoFavPerder++;

        }

        // i = 4 por que los 4 primeros ya lo hemos mirado.

        //Calculamos las probabilidades del resto de las cartas.

        for (int i = player.GetComponent<CardHand>().cards.Count + dealer.GetComponent<CardHand>().cards.Count + 1; i < values.Length; i++)
        {

            if (values[i] > cantidadSuperarDealer)
            {

                casoFavDealer++;

            }

            if (values[i] + cantidadSuperarJugador <= 21 && values[i] + cantidadSuperarJugador >= 17)
            {

                casoFavJugador++;

            }

            if (values[i] + cantidadSuperarJugador > 21)
            {

                casoFavPerder++;

            }

        }
        probMessage.text = "El dealer tenga más puntuación que el jugador: " + (100 * (casoFavDealer / casosTotales)).ToString() + "%" +"\r\n" + "\r\n" + "El jugador obtenga entre un 17 y un 21 si pide una carta: " + (100 * (casoFavJugador / casosTotales)).ToString() + "%" +"\r\n" + "\r\n" + "El jugador obtenga más de 21 si pide una carta: " + (100 * (casoFavPerder / casosTotales)).ToString() + "%";
        //========================================================================
        //Probabilad Dealer
        //========================================================================
        Debug.Log("Dealer tenga más puntuación que el jugador " + 100*(casoFavDealer/casosTotales));
        //========================================================================
        //Probabilad Jugador
        //========================================================================
        Debug.Log("Probabilad Jugador "+ 100 * (casoFavJugador / casosTotales));
        //========================================================================
        //Probabilad Pasarse
        //========================================================================
        Debug.Log("Probabilad Pasarse  " + 100 * (casoFavPerder / casosTotales));

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

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
        */
        if(player.GetComponent<CardHand>().points > 21)
        {
            //Le damos la vuelta a la carta el Dealer.
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
            //Mostramos mensaje de derrota.
            finalMessage.text = "La partida ha finalizado, pierdes";
        }

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
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
    
}
