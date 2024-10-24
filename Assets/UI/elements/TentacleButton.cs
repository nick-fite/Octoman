using UnityEngine;
using UnityEngine.EventSystems;

public class TentacleButton : Widget, IDragHandler, IEndDragHandler
{
    public delegate void OnTentacleMoveDelegate(Vector3 newPos);
    public delegate void OnTentacleEndMoveDelegate();
    public event OnTentacleMoveDelegate OnTentacleMove;
    public event OnTentacleEndMoveDelegate OnTentacleEndMove;


    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta;
        OnTentacleMove?.Invoke(transform.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnTentacleEndMove?.Invoke();
    }

}
