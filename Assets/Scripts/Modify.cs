using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Modify
{
    public string modify_ID;
    public ValueType modify_Type = ValueType.Int;
    public string modify_Des;
    // Value

    public int modify_IntValue;
    public float modify_FloatValue;
    public bool modify_BoolValue;
    public string modify_StringValue;
}

[System.Serializable]
public class AreaInstance
{
    // Basic Stats
    public float damage;
    public GameObject collider;
    public Transform group;

    public AreaInstance(float damage, GameObject collider, Transform group)
    {
        this.damage = damage;
        this.group = group;
        this.collider = collider;
    }
}