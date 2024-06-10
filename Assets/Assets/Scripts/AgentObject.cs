using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentObject : MonoBehaviour
{
    //[SerializeField] private Transform m_target;

    protected Vector3 TargetPosition;
    void Start()
    {
        Debug.Log("Starting Agent");
        //TargetPostition = m_target.position;
    }
    
}
