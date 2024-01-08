using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class ControlLobby : MonoBehaviour
{

    #region PANEL INICIO

    [SerializeField] private GameObject panelInicio;
    [SerializeField] private TMP_InputField inputNickname;
    [SerializeField] private Button botonIniciar;
    [SerializeField] private TMP_Text notificacion;

    private void Awake()
    {




    }

    private void Start()
    {

        PhotonNetwork.ConnectUsingSettings();

    }


    #endregion PANEL INICIO

    #region PANEL SELECCION

    [SerializeField] private GameObject panelSeleccion;



    #endregion PANEL SELECCION




}
