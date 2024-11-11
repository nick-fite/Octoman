using Unity.Mathematics;
using UnityEditor;
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
        _bones = new Transform[chainLength + 1];
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
       ResolveIK(); 
    }

    private void ResolveIK()
    {
        if (target == null)
            return;
        
        if(_boneLength.Length != chainLength)
            Init();

        for(int i = 0; i < _bones.Length; i++)
        {
            _positions[i] = GetPositionRootSpace(_bones[i]);
        }

        Vector3 targetPos = GetPositionRootSpace(target);
        quaternion targetRot = GetRotationRootSpace(target);

        //if it's possible to reach
        /*if((targetPos - GetPositionRootSpace(_bones[0])).sqrMagnitude >= _completeLength * _completeLength)
        {
            Vector3 dir = (targetPos - _positions[0]).normalized;

            for(int i = 1; i < _positions.Length; i++)
            {
                _positions[i] = _positions[i -1] + dir * _boneLength[i-1];
            }
        }
        else
        {*/
            for(int i = 0; i < _positions.Length -1; i++)
            {
                _positions[i + 1] = Vector3.Lerp(_positions[i + 1], _positions[i] + _startDirectionSuccess[i], snapbackStrength);
            }
            for(int iteration = 0; iteration < iterations; iteration++)
            {
                for(int i = _positions.Length -1; i > 0; i--)
                {
                    if(i == _positions.Length - 1)
                    {
                        _positions[i] = targetPos;
                    }
                    else 
                    {
                        _positions[i] = _positions[i+1] + (_positions[i] - _positions[i+1]).normalized * _boneLength[i];
                    }
                }

                for(int i = 1; i < _positions.Length - 1; i++)
                {
                    _positions[i] = _positions[i] + (_positions[i] - _positions[i-1]).normalized * _boneLength[i-1];
                }

                if ((_positions[_positions.Length -1] - targetPos).sqrMagnitude < delta * delta)
                    break;
            }
        //}

        if (pole != null)
        {
            Vector3 polePosition = GetPositionRootSpace(pole);
            for(int i = 1; i < _positions.Length - 1; i++)
            {
                Plane plane = new Plane(_positions[i + 1] - _positions[i-1], _positions[i -1]);
                Vector3 projectedPole = plane.ClosestPointOnPlane(polePosition);
                Vector3 projectedBone = plane.ClosestPointOnPlane(_positions[i]);
                float angle = Vector3.SignedAngle(projectedBone - _positions[i - 1], projectedPole - _positions[i - 1], plane.normal);
                _positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (_positions[i] - _positions[i - 1]) + _positions[i-1];
            }
        }

        for(int i = 0; i < _positions.Length; i++)
        {
            if(i == _positions.Length - 1)
            {
                SetRotationRootSpace(_bones[i],  Quaternion.Inverse(targetRot) * _startRotationTarget * Quaternion.Inverse(_startRotationBone[i]));
            }
            else
            {
                SetRotationRootSpace(_bones[i], Quaternion.FromToRotation(_startDirectionSuccess[i], _positions[i + 1] - _positions[i]) * Quaternion.Inverse(_startRotationBone[i]));
            }
            SetPositionRootSpace(_bones[i], _positions[i]);
        }
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
    
    private void SetRotationRootSpace(Transform current, Quaternion rot)
    {
        if (_root == null)
            current.rotation = rot;
        else
            current.rotation = _root.rotation * rot;
    }

    void OnDrawGizmos()
    {
        Transform current = transform;
        for(int i = 0; i < chainLength && current != null && current.parent != null; i++)
        {
            float scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }

}
