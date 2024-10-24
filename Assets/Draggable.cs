using UnityEngine;

public class Draggable : MonoBehaviour
{
    [SerializeField] LayerMask _mask;
    [SerializeField] Rigidbody _rb;
    [SerializeField] private TentacleButton _widgetPrefab;
    [SerializeField] private Transform _attachTransform;
    
    private TentacleButton _widget;

    void Start()
    {
        _widget = Instantiate(_widgetPrefab);
        _widget.SetOwner(gameObject);

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if(canvas)
        {
            _widget.transform.SetParent(canvas.transform);
        }
        
        _widget.OnTentacleMove += MoveTentacle;
        _widget.OnTentacleEndMove += EndMove;

        _rb = GetComponent<Rigidbody>();
    }

    private void MoveTentacle(Vector3 newPos)
    {
        _rb.isKinematic = true;
        Vector3 posToWorld = Camera.main.ScreenToWorldPoint(newPos);
        transform.position = posToWorld;
    }

    private void EndMove()
    {
        _rb.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(_widget)
        {
            _widget.transform.position = Camera.main.WorldToScreenPoint(_attachTransform.position);
        }
    }
}
