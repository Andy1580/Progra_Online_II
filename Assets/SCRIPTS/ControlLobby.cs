using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using System;

public class ControlLobby : MonoBehaviourPunCallbacks
{

    #region PANEL INICIO

    [SerializeField] private GameObject panelInicio;
    [SerializeField] private TMP_InputField inputNickname;
    [SerializeField] private Button botonIniciar;
    [SerializeField] private TMP_Text notificacion;

    private void Awake()
    {

        panelInicio.SetActive(true);
        panelSeleccion.SetActive(false);

        //Deshabilitar el clic del Boton Inicio 
        botonIniciar.interactable = false;

        //Borrar la notificacion
        notificacion.text = string.Empty;

    }

    private void Start()
    {

        notificacion.text = "Conectandose a Photon";
        PhotonNetwork.ConnectUsingSettings();

    }

    //Este metodo se ejecutara cuando estemos conectados a Photon
    public override void OnConnectedToMaster()
    {

        notificacion.text = "Entrando al Lobby...";

        //Enviarnos al lobby
        PhotonNetwork.JoinLobby();

    }

    //Este metodo se ejecuta cuando entramos al Lobby
    public override void OnJoinedLobby()
    {

        //Esperamos medio segundo a activar el boton de Iniciar
        Invoke("ActivarBoton", 0.5f);

    }

    private void ActivarBoton()
    {
        //Le asignamos funcion al boton
        botonIniciar.onClick.AddListener(Iniciar);

        //Se puede volver a clicear el boton
        botonIniciar.interactable = true;
        notificacion.text = string.Empty;
        
    }

    private void Iniciar()
    {

        //Obtenemos el string del input del Field
        string nickname = inputNickname.text;

        //Revisar que el string no este vacio
        if (nickname == string.Empty)
        {
            notificacion.text = "¡El nickname esta vacio!";
            return;
        }

        //Revisamos que el nickname no supere los 10 caracteres
        if (nickname.Length > 10)
        {
            notificacion.text = "¡El nickname no puede tener mas de 10 caracteres!";
            return;
        }

        //Asignamos nuestro nickname online
        PhotonNetwork.NickName = nickname;

        notificacion.text = "Conectando a la sala...";

        //Si no hay ninguna sala abierta ...
        if (PhotonNetwork.CountOfRooms == 0)
        {
                //Tienen que importar Photon.Realtime;
                var config = new RoomOptions() { MaxPlayers = 8 };

                //Creamos la sala
                bool conectado = PhotonNetwork.CreateRoom("XP", config);

                if (!conectado)
                    notificacion.text = "¡Problema al crear la sala!";
        }

        //Si ya hay una sala creada
        else
        {
            //Unirnos a la sala XP
            bool conectado = PhotonNetwork.JoinRoom("XP");

            if (!conectado)
                notificacion.text = "¡No se pudo unir a la sala!";
        }

    }


    #endregion PANEL INICIO

    #region PANEL SELECCION

    [SerializeField] private GameObject panelSeleccion;
   
    private void Awake_Seleccion()
    {



    }

    #region PHOTON

    //Este metodo solo se ejecuta quien creo la sala, antes que OnJoinedRoom
    public override void OnCreatedRoom()
    {

        InicializarChat();

    }

    //Este metodo se ejecuta cuando entramos a la sala
    public override void OnJoinedRoom()
    {

        //Sincronizamos la escena
        PhotonNetwork.AutomaticallySyncScene = true;

        //Cambiar de Pantalla
        panelInicio.SetActive(false);
        panelSeleccion.SetActive(true);

        //Genera los slots de los jugadores en la sala
        Inicializar_SlotsJugador();

        //Asignarle la funcion al boton
        botonEnviar.onClick.AddListener(EnviarMensaje);

    }

    //Este metodo se ejecuta cuando un jugador entra a la sala
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

        print(newPlayer);

        //Cuando se una un nuevo jugador a la sala, se creara el Slot
        Crear_SlotJugador(newPlayer);

    }

    //Este metodo se ejecuta cuando un jugador deja la sala
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {

        Eliminar_Jugador(otherPlayer);

    }

    //Este metodo se ejecuta cuando cambias las propiedades de la sala
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

        ActualizarChat();

    }

    #endregion PHOTON

    #region PANEL SLOTS JUGADOR

    [SerializeField] private Transform panelJugadores;
    [SerializeField] private Lobby_SlotJugador pfSlotJugador;
    private Dictionary<Player, Lobby_SlotJugador> slotsJugador;

    private void Inicializar_SlotsJugador()
    {

        //Inicializamos el Diccionario
        slotsJugador = new Dictionary<Player, Lobby_SlotJugador>();

        //Obtenemos los Players que hay en la sala
        var jugadorEnSala = PhotonNetwork.CurrentRoom.Players;

        foreach(Player player in jugadorEnSala.Values)
        {
            Crear_SlotJugador(player);
        }

    }

    private void Crear_SlotJugador(Player player)
    {
        //Instanciamos el slot en el Panel Jugadores
        Lobby_SlotJugador slot = Instantiate(pfSlotJugador, panelJugadores);

        //Le asignamos su Player
        slot.Player = player;

        //Le guardamos en el diccionario
        slotsJugador[player] = slot;

    }

    private void Eliminar_Jugador(Player player)
    {

        //Eliminamos el slot de la UI
        Destroy(slotsJugador[player].gameObject);


        //Eliminamos el par del diccionario
        slotsJugador.Remove(player);

    }

    #endregion PANEL SLOTS JUGADOR

    #region Chat

    [Header("\nCHAT")]
    [SerializeField] private TMP_InputField inputMensaje;
    [SerializeField] private Button botonEnviar;
    [SerializeField] private RectTransform contenct;
    [SerializeField] private TMP_Text chatTexto;

    private void InicializarChat()
    {

        //Obtenemos las propiedades de la sala
        var propiedades = PhotonNetwork.CurrentRoom.CustomProperties;

        //Creamos una nueva entrada
        propiedades["Chat"] = "INICIO DE CHAT";

        //Aplicamos los cambios
        PhotonNetwork.CurrentRoom.SetCustomProperties(propiedades);

    }

    private void EnviarMensaje()
    {

        //Obtenemos el mensaje del Input Field
        string mensaje = inputMensaje.text;

        //Verificamos que el mensaje no este vacio
        if (mensaje == string.Empty)
            return;

        //Obtenemos las porpiedades de la sala
        var propiedades = PhotonNetwork.CurrentRoom.CustomProperties;

        //Obtenemos el valor del chat en una string
        string chat = propiedades["Chat"].ToString();

        //Concatenamos nuestro nuevo mensaje
        chat += $"\n{PhotonNetwork.NickName}: {mensaje}";

        //Cambiamos el value de la Kay Chat
        propiedades["Chat"] = chat;

        //Aplicamos los cambios a las propiedades
        //Osea ... enviamos nuestro nuevo mensaje
        PhotonNetwork.CurrentRoom.SetCustomProperties(propiedades);

        //En cuanto envie el mensaje, limpie el InputField
        inputMensaje.text = String.Empty;

    }

    private void ActualizarChat()
    {

        //Obtenemos las propiedades de Photon
        var propiedades = PhotonNetwork.CurrentRoom.CustomProperties;

        //Si no existe la Key Chat
        if (!propiedades.ContainsKey("Chat"))
            return;

        //Obtenemos la string de Chat
        string chat = propiedades["Chat"].ToString();

        //Asignamos el texto en pantalla
        chatTexto.text = chat;

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnviarMensaje();
        }

    }

    #endregion Chat

    #endregion PANEL SELECCION

}