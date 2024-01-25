using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

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

        notificacion.text = "Entrando al Lobby ...";

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
            notificacion.text = "! El nickname esta vacio";
            return;
        }

        //Revisamos que el nickname no supere los 10 caracteres
        if (nickname.Length > 10)
        {
            notificacion.text = "! El nickname no puede tener mas de 10 caracteres";
            return;
        }

        //Asignamos nuestro nickname online
        PhotonNetwork.NickName = nickname;

        notificacion.text = "Conectando a la sala ...";

        //Si no hay ninguna sala abierta ...
        if (PhotonNetwork.CountOfRooms == 0)
        {
                //Tienen que importar Photon.Realtime;
                var config = new RoomOptions() { MaxPlayers = 8 };

                //Creamos la sala
                bool conectado = PhotonNetwork.CreateRoom("XP", config);

                if (!conectado)
                    notificacion.text = "! Problema al crear sala";
        }

        //Si ya hay una sala creada
        else
        {
            //Unirnos a la sala XP
            bool conectado = PhotonNetwork.JoinRoom("XP");

            if (!conectado)
                notificacion.text = "! No se pudo unir a la sala";
        }

    }


    #endregion PANEL INICIO

    #region PANEL SELECCION

    [SerializeField] private GameObject panelSeleccion;
    [SerializeField] private Transform panelJugadores;
    [SerializeField] private Lobby_SlotJugador pfSlotJugador;

    private void Awake_Seleccion()
    {



    }

    //Este metodo se ejecuta cuando entramos a la sala
    public override void OnJoinedRoom()
    {

        //Sincronizamos la escena
        PhotonNetwork.AutomaticallySyncScene = true;

        panelInicio.SetActive(false);
        panelSeleccion.SetActive(true);

    }

    #endregion PANEL SELECCION




}
