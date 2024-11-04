using UnityEngine;

public class CustomIK : MonoBehaviour
{
    //number of bones
    [SerializeField] int chainLength = 2;
    
    [SerializeField] Transform target;
    [SerializeField] Transform pole;

    [SerializeField] int iterations = 10;

    [SerializeField] float delta = 0.001f;

    [SerializeField] float snapbackStrength = 1f;
    float[] _boneLength;
    float _completeLength;
    Transform[] _bones;
    Vector3[] _positions;
    Vector3[] _startDirectionSuccess;
    Quaternion[] _startRotationBone;
    Quaternion _startRotationTarget;
    Transform _root;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _bones = new Transform[chainLength - 1];
        _positions = new Vector3[chainLength + 1];
        _boneLength = new float[chainLength];
        _startDirectionSuccess = new Vector3[chainLength+1];
        _startRotationBone = new Quaternion[chainLength+1];
        
        _root = transform;
        for(int i = 0; i <= chainLength; i++)
        {
            if(_root == null)
                throw new UnityException("Chain value is longer then ancestoral chain");
            _root = _root.parent;
        }

        if(target == null)
        {
            target = new GameObject(gameObject.name + "target").transform;
            SetPositionRootSpace(target, GetPositionRootSpace(transform));
        }

        _startRotationTarget = GetRotationRootSpace(target);

        Transform current = transform;
        _completeLength = 0;

        for(int i = _bones.Length-1; i >= 0; i--)
        {
            _bones[i] = current;
            _startRotationBone[i] = GetRotationRootSpace(current);

            if(i == _bones.Length-1)
            {
                _startDirectionSuccess[i] = GetPositionRootSpace(target) - GetPositionRootSpace(current);
            }
            else
            {
                _startDirectionSuccess[i] = GetPositionRootSpace(_bones[i+1])- GetPositionRootSpace(current);
                _boneLength[i] = _startDirectionSuccess[i].magnitude;
                _completeLength += _boneLength[i];
            }
            current = current.parent;
        }
    }

    private void LateUpdate()
    {
        
    }

    private void SetPositionRootSpace(Transform current, Vector3 position)
    {
        if(_root == null)
            current.position = position;
        else
            current.position = _root.rotation * position + _root.position;
    }

    private Vector3 GetPositionRootSpace(Transform current)
    {
        if(_root == null)
            return current.position;
        
        return Quaternion.Inverse(_root.rotation) * (current.position - _root.position);
    }

    private Quaternion GetRotationRootSpace(Transform current)
    {
        if (_root == null)
            return current.rotation;
        
        return Quaternion.Inverse(current.rotation) * _root.rotation;
    }
    
}
