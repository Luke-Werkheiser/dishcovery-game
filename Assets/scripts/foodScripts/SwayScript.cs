using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayScript : MonoBehaviour {
    public bool isLocal;
    public Vector3 point = Vector3.up; 
    [Range( 1, 25 )] 
    public float drag = 10; 
    public Transform pendulumObject;
    private Vector3 _pointDefault;
    private Transform _t;

    private Vector3 _upDirection;
    private float _length;
    private Vector3 _tangent;
    private Vector3 _offset;
    private const float MinValue = 0.002f;
    private const float OffsetMul = 2f;
    private const float DragMul = 100f;
    private bool isSleeping;   

    private const float Threshold = 1e-08f;

    void Start()
    {
        _t = transform;
        _length = point.magnitude;
        _pointDefault = point;
        _upDirection = -_pointDefault.normalized;
        pendulumObject.parent = null;
    }

    void Update()
    {
        if (isLocal)
        {
            _upDirection = _t.TransformDirection(-_pointDefault.normalized);
        }

        _tangent = ((_t.position - point).normalized - _upDirection);
        isSleeping = _tangent.sqrMagnitude < Threshold;

        if (!isSleeping)
        {
            _offset = Vector3.Lerp(_offset, _tangent / drag, Time.deltaTime * (_offset.sqrMagnitude * OffsetMul + MinValue ) * drag * DragMul);
            point = _t.position + (point + _offset - _t.position).normalized * _length;
            pendulumObject.position = point;
        }
    }}


