using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingAxe : MonoBehaviour
{
    [SerializeField] private float m_fAngularTorque = 10f, m_fMaxTorque = 20f;
    [SerializeField] private float m_fMinRotation = 0, m_fMaxRotation = 0, m_fRotationalThreshold = 5f;

    [SerializeField]
    private Vector3 m_vForce = Vector3.zero;
    [SerializeField]
    private Vector3 m_vCenterOfMass = Vector3.zero;
    [SerializeField]
    private Vector3 m_vForcePoint = Vector3.zero;

    private Vector3 m_vTorque = Vector3.zero;

    private HingeJoint m_hJoint = null;
    private Rigidbody m_rBody = null;

    private void Awake()
    {
        m_hJoint = GetComponent<HingeJoint>();
        m_rBody = GetComponent<Rigidbody>();

        m_rBody.maxAngularVelocity = m_fMaxTorque;

        // Set joint limits based on editor preference.
        JointLimits limits = m_hJoint.limits;
        limits.min = m_fMinRotation;
        limits.max = m_fMaxRotation;
        m_hJoint.limits = limits;
        m_hJoint.useLimits = true;
    }

    private void FixedUpdate()
    {
        m_vTorque = Vector3.Cross(m_vForce, m_vForcePoint - m_vCenterOfMass);
        Debug.Log(m_vTorque);
        m_rBody.AddTorque(m_vTorque);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.TransformPoint(m_vCenterOfMass), 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.TransformPoint(m_vForcePoint), 0.1f);
    }
}
