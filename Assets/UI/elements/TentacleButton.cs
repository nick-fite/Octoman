using UnityEngine;
using UnityEngine.EventSystems;

public class TentacleButton : Widget, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler
{
    public delegate void OnTentacleMoveDelegate(Vector3 newPos);
    public delegate void OnTentacleEndMoveDelegate();
    public delegate void OnTentacleStartMoveDelegate();
    //public delegate void OnFreezePosDelegate(bool isFrozen);
    public event OnTentacleMoveDelegate OnTentacleMove;
    public event OnTentacleEndMoveDelegate OnTentacleEndMove;
    public event OnTentacleStartMoveDelegate OnTentacleStartMove;
    //public event OnFreezePosDelegate OnFreezePos;

    bool iSDragging;
    public bool isFrozen = false;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            isFrozen = false;
            iSDragging = true;

            _rectTransform.anchoredPosition += eventData.delta;
            OnTentacleMove?.Invoke(transform.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        iSDragging = false;
        OnTentacleEndMove?.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnTentacleStartMove?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right && iSDragging)
        {
            isFrozen = !isFrozen;
        }
    }

}
