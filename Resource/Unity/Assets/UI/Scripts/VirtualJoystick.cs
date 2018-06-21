using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

    public Image ImgBg;
    public Image ImgJoystick;

    private Vector2 m_InputVector;

    public void OnPointerDown(PointerEventData e)
    {
        OnDrag(e);
    }

    public void OnPointerUp(PointerEventData e)
    {
        UpdateInputVector(Vector2.zero);
        ImgJoystick.rectTransform.anchoredPosition = Vector3.zero;
    }

    public void OnDrag(PointerEventData e)
    {
        Vector2 pos;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(ImgBg.rectTransform, e.position, e.pressEventCamera, out pos))
        {
            pos.x = (pos.x / ImgBg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / ImgBg.rectTransform.sizeDelta.y);

            UpdateInputVector(new Vector2(pos.x * 2 - 1, pos.y * 2 - 1));
            UpdateInputVector((m_InputVector.magnitude > 1.0f) ? m_InputVector.normalized : m_InputVector);

            ImgJoystick.rectTransform.anchoredPosition = new Vector3(m_InputVector.x * (ImgBg.rectTransform.sizeDelta.x * .4f),
                                                                     m_InputVector.y * (ImgBg.rectTransform.sizeDelta.y * .4f));
        }
    }

    private void UpdateInputVector(Vector2 vector)
    {
        m_InputVector = vector;
        UnityDelegate.GfxMoudle.Instance.SetJoystickXY(m_InputVector.x, m_InputVector.y);
    }
}

