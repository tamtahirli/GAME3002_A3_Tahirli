using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingAxe : MonoBehaviour
{
    [SerializeField] private float m_fMaxTorque = 20f, m_fMinRotation = 0, 
        m_fMaxRotation = 0, m_fPushForce = 10f, m_fUpPushForce = 5f;

    [SerializeField] private Vector3 m_vForce = Vector3.zero;
    [SerializeField] private Vector3 m_vCenterOfMass = Vector3.zero;
    [SerializeField] private Vector3 m_vForcePoint = Vector3.zero;

    private Vector3 m_vTorque = Vector3.zero;

    private HingeJoint m_hJoint = null;
    private Rigidbody m_rBody = null;

    private void Awake()
    {
        m_hJoint = GetComponent<HingeJoint>();
        m_rBody = GetComponent<Rigidbody>();

        // Set max torque
        m_rBody.maxAngularVelocity = m_fMaxTorque;

        // Set joint limits based on editor preference.
        JointLimits limits = m_hJoint.limits;
        limits.min = m_fMinRotation;
        limits.max = m_fMaxRotation;

        // Enable limits and set hinge joints to created limits
        m_hJoint.limits = limits;
        m_hJoint.useLimits = true;

        // Calculate torque based on cross product from force point and force
        m_vTorque = Vector3.Cross(m_vForce, m_vForcePoint - m_vCenterOfMass);
    }

    private void FixedUpdate()
    {
        // If hit minimum or maximum angle, negate torque so it goes the opposite direction
        if (m_hJoint.angle <= m_fMinRotation)
        {
            m_vTorque *= -1;
        }
        else if (m_hJoint.angle >= m_fMaxRotation)
        {
            m_vTorque *= -1;
        }

        m_rBody.AddTorque(m_vTorque);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Make a push collision effect when hitting the axe
        if(collision.gameObject.CompareTag("Player"))
        {
            Vector3 normal = collision.GetContact(0).normal;

            // All of the normal values are too unpredictable, removing X so they dont get pushed forward only to the sides
            // Set the Y so they aren't ever thrown too far in the air
            normal.x = 0;
            normal.y = m_fUpPushForce;
            normal.z = normal.z >= 0 ? -m_fPushForce : m_fPushForce; // Set normal manually based on direction

            Vector3 lookPos = transform.position;
            lookPos.y = Player.player.transform.position.y;
            Player.player.transform.LookAt(lookPos); // Make player look at axe when hit

            Player.GetAnimator().SetTrigger("Pushback"); // Push back animation. Will stop when grounded.

            Player.Rgbd().AddForce(normal, ForceMode.VelocityChange);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.TransformPoint(m_vCenterOfMass), 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.TransformPoint(m_vForcePoint), 0.1f);
    }
#endif
}
