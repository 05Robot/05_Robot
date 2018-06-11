using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BloodHandleEvent : MonoBehaviour , IPointerEnterHandler, IPointerDownHandler, ISelectHandler, IDeselectHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        RepairStationUI.Instance.HightLight();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RepairStationUI.Instance.HightLight();
    }


    public void OnSelect(BaseEventData eventData)
    {
        RepairStationUI.Instance.HightLight();

    }

    public void OnDeselect(BaseEventData eventData)
    {
        RepairStationUI.Instance.NotHightLight();
    }
}
