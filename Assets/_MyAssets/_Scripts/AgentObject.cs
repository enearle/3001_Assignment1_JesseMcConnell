using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentObject : MonoBehaviour
{
    [SerializeField] private Transform m_target;

    public Vector3 TargetPostition
    {
        get { return m_target.position; }
        set { m_target.position = value; }
    }
    void Start()
    {
        Debug.Log("Starting Agent");
        TargetPostition = m_target.position;
    }
    
}
