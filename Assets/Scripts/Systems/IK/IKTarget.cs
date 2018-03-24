using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTarget
{
    Vector3 _position = Vector3.zero;
    Quaternion _rotation = Quaternion.identity;
    float _weight = 0;

    public IKTarget()
    {
        _position = Vector3.zero;
        _rotation = Quaternion.identity;
        _weight = 0;
    }

    public IKTarget(Vector3 position, float weight)
    {
        _position = position;
        _rotation = Quaternion.identity;
        _weight = weight;
    }

    public IKTarget(Vector3 position, Quaternion rotation, float weight)
    {
        _position = position;
        _rotation = rotation;
        _weight = weight;
    }

    public void Set(Transform t)
    {
        _position = t.position;
        _rotation = t.rotation;
    }

    public static IKTarget FromTransform(Transform t)
    {
        if (t)
            return new IKTarget(t.position, t.rotation, 0);
        else return null;
    }

    public Vector3 position
    {
        set { _position = value; }
        get { return _position; }
    }
    
    public Quaternion rotation
    {
        set { _rotation = value; }
        get { return _rotation; }
    }

    public float weight
    {
        set { _weight = value; }
        get { return _weight; }
    }
}
