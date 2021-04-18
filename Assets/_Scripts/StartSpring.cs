using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSpring : MonoBehaviour
{
    [SerializeField] private Spring m_spring;

    // Use this script to enable spring movement.

    private void OnCollisionEnter(Collision collision)
    {
        m_spring.SetInUse(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        m_spring.SetInUse(false);
    }
}
