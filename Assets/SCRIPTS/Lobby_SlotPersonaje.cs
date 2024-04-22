using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Lobby_SlotPersonaje : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler
{

    [SerializeField] private Animator personaje;

    public void OnPointerClick(PointerEventData eventData)
    {

        ControlLobby.SeleccionarPersonaje(this);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        personaje.SetTrigger("ataqueUI");

    }

}
