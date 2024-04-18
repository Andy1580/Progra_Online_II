using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Lobby_SlotPersonaje : MonoBehaviour,IPointerClickHandler
{

    public void OnPointerClick(PointerEventData eventData)
    {

        ControlLobby.SeleccionarPersonaje(this);

    }

}
