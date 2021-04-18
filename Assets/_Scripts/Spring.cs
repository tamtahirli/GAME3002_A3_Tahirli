using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    [SerializeField] private float m_fSpring = 2f, m_fDamping = 20f;
    [SerializeField] private Vector3 m_vRestPos = Vector3.zero;
    [SerializeField] private bool m_bIsBungee = false, m_bCalcSpring = false;
    [SerializeField] private Rigidbody m_attachedBody = null;

    private Vector3 m_vForce = Vector3.zero, m_vPrevVel = Vector3.zero;
    private float m_fMass = 0f;
    private bool m_bInUse = false;

    private void Start()
    {
        m_fMass = m_attachedBody.mass;
        m_vRestPos = transform.position + m_vRestPos;
        if (m_bCalcSpring) m_fSpring = CalculateSpringConstant();
    }

    private void FixedUpdate()
    {
        if (m_bInUse)
            UpdateSpringForce();
        else
            m_attachedBody.transform.position = m_vRestPos;
    }

    public void SetInUse(bool inuse)
    {
        m_bInUse = inuse;
    }

    private float CalculateSpringConstant()
    {
        // k = F / dX
        // F = m * a
        // k = m * a / (xf - xi)

        float fDX = (m_vRestPos - m_attachedBody.transform.position).magnitude;

        if (fDX <= 0f)
        {
            return Mathf.Epsilon;
        }

        return (m_fMass * Physics.gravity.y) / (fDX);
    }

    private void UpdateSpringForce()
    {
        // F = -kx
        // F = -kx -bv

        if (m_bIsBungee)
        {
            float fLen = (m_vRestPos - m_attachedBody.transform.position).magnitude;

            if (fLen <= m_vRestPos.y)
            {
                return;
            }
        }

        m_vForce = -m_fSpring * (m_vRestPos - m_attachedBody.transform.position) -
            m_fDamping * (m_attachedBody.velocity - m_vPrevVel);

        m_attachedBody.AddForce(m_vForce, ForceMode.Acceleration);

        m_vPrevVel = m_attachedBody.velocity;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + m_vRestPos, 1f);

        if (m_attachedBody)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_attachedBody.transform.position, 1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, m_attachedBody.transform.position);
        }
    }
#endif
}
