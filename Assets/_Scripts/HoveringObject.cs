using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoveringObject : MonoBehaviour
{

    [SerializeField] private float m_fHoverSpeed = 5f, m_fHoverScale = 0.5f;



    private Vector3 m_vStartPos = Vector3.zero;

    void Start()
    {
        m_vStartPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = m_vStartPos;
        pos.y += Mathf.Sin(Time.time * m_fHoverSpeed) * m_fHoverScale;
        transform.position = pos;
    }
}
