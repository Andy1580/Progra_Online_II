using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class Lobby_SlotJugador : MonoBehaviour
{

    [SerializeField] private TMP_Text nickname;

    private Player _player;

    public Player player
    {
        get => _player;
        set
        {
            _player = value;
            nickname.text = value.NickName;
        }
    }

}