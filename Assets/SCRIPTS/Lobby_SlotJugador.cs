using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Lobby_SlotJugador : MonoBehaviour
{

    [SerializeField] private TMP_Text nickname;
    [SerializeField] private Transform slotPersonaje;
    public Transform SlotPersonaje => slotPersonaje;

    private Player _player;

    public Player Player
    {
        get => _player;
        set
        {
            _player = value;
            nickname.text = value.NickName;
        }
    }

}