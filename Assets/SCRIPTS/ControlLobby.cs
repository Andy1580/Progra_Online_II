using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
        notificacion.text = String.Empty;

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

        //Se puede volver a clickear el boton
        botonIniciar.interactable = true;
        notificacion.text = String.Empty;
        
    }

    private void Iniciar()
    {

        //Obtenemos el string del input del Field
        string nickname = inputNickname.text;

        //Revisar que el string no este vacio
        if (nickname == String.Empty)
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
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {

        ActualizarChat();

    }

    //Este metodo se ejecuta cuando cambias las propiedades de un Jugador
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {

        ActualizarPersonaje(targetPlayer);

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
    [SerializeField] private RectTransform content;
    [SerializeField] private TMP_Text chatTexto;
    [SerializeField] private RectTransform scrollView;

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

        //Cambiamos el value de la Key Chat
        propiedades["Chat"] = chat;

        //Aplicamos los cambios a las propiedades
        //Osea ... enviamos nuestro nuevo mensaje
        PhotonNetwork.CurrentRoom.SetCustomProperties(propiedades);

        //En cuanto envie el mensaje, limpie el InputField
        inputMensaje.text = String.Empty;

        //Focus al InputField
        inputMensaje.ActivateInputField();

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

        int contectOffset = 25;
        int alturaLineaTexto = 34;

        //Importar la libreria: using System.Linq;
        int espacio = chat.Count(car => car == '\n');

        //Calculamos la altura segun los saltos de linea
        float altura = contectOffset + (alturaLineaTexto * espacio);

        //Crecemos el Conect
        content.sizeDelta = new Vector2(content.sizeDelta.x, altura);

        //Si el Content es mas alto que el Scroll View
        if(content.sizeDelta.y > scrollView.sizeDelta.y)
        {
            //Obtenemos la posicion del Content
            Vector3 posicionContent = content.localPosition;

            //Obtener la posicion inferior del chat
            posicionContent.y = content.sizeDelta.y - scrollView.sizeDelta.y;

            //Establecer la posicion
            content.localPosition = posicionContent;
        }

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnviarMensaje();
        }

    }

    #endregion Chat

    #region Personajes

    public static void SeleccionarPersonaje(Lobby_SlotPersonaje slot)
    {

        //Obtenemos el nombre del personaje
        string nombre = slot.gameObject.name;

        //Obtenemos las propiedades de nuestro user online
        var propiedades = PhotonNetwork.LocalPlayer.CustomProperties;

        //Le asiganmos el personaje
        propiedades["Personaje"] = nombre;

        //Aplicamos los cambios
        PhotonNetwork.LocalPlayer.SetCustomProperties(propiedades);

    }

    private void ActualizarPersonaje(Player player)
    {

        //Si aun no ha elegido personaje
        if (!player.CustomProperties.ContainsKey("Personaje"))
            return;

        //Obtenemos la ruta del Prefab del personaje
        string nombre = player.CustomProperties["Personaje"].ToString();
        string ruta = nombre + "/" + nombre + "Image";

        //Obtenemos la referencia del Prefab
        GameObject pfPersonaje = Resources.Load<GameObject>(ruta);

        //Obtenemos el Slot del Player
        Lobby_SlotJugador slot = slotsJugador[player];

        //Si ya tenia una Image, lo eliminamos
        if (slot.SlotPersonaje.childCount > 0)
            Destroy(slot.SlotPersonaje.GetChild(0).gameObject);

        //Lo instanciamos
        Instantiate(pfPersonaje, slot.SlotPersonaje);

    }

    #endregion Personajes

    #endregion PANEL SELECCION

}