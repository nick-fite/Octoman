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
        _rb.useGravity = false;
        Vector3 posToWorld = Camera.main.ScreenToWorldPoint(newPos);
        transform.position = posToWorld;
    }

    private void EndMove()
    {
        if(_widget.isFrozen)
        {
            return;
        }
        _rb.useGravity = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_rb.useGravity && !_widget.isFrozen)
        {
            if(Input.GetMouseButtonDown(1))
            {
                _widget.isFrozen = true;
            }
            transform.Translate(Vector3.up * Input.GetAxis("Mouse ScrollWheel"));
        }

        if(_widget)
        {
            _widget.transform.position = Camera.main.WorldToScreenPoint(_attachTransform.position);
        }
    }
}
