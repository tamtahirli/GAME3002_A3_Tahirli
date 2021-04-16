using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
	[SerializeField] private Animator m_animator;
	[SerializeField] private float m_fMovementSpeed = 1f;
    [SerializeField] private float m_fTopSpeed = 20f;
    [SerializeField] private float m_fJumpForce = 5f;
	[SerializeField] private float m_fSlowDownScale = 0.99f;
	[SerializeField] private LayerMask m_lGroundedMask;

	private Rigidbody m_rBody;
	private BoxCollider m_bCollider;
	private float m_fHalfHeight;
	private Vector3 m_vStartScale;
	[SerializeField] private bool m_bGrounded;

    void Awake()
    {
		m_rBody = GetComponent<Rigidbody>();
		m_bCollider = GetComponent<BoxCollider>();
		m_fHalfHeight = m_bCollider.bounds.extents.y;
		m_vStartScale = transform.localScale;
	}

    private void FixedUpdate()
    {
		m_bGrounded = CheckGrounded();
		float fHorizontal = Input.GetAxis("Horizontal");

		m_rBody.AddForce(Vector3.right * fHorizontal, ForceMode.VelocityChange); // Add force on rigid body based on horizontal movement

		m_rBody.velocity *= m_fSlowDownScale; // Slow down player every frame to avoid slideyness

		float fAbsX = Mathf.Abs(m_rBody.velocity.x);
		m_animator.SetFloat("Movement", fAbsX / m_fTopSpeed); // Set animator float from scale 0-1.

		if(fAbsX > m_fTopSpeed) // Only use top speed for lateral movement
        {
			Vector3 velocity = m_rBody.velocity;
			velocity.x = velocity.x < 0 ? -m_fTopSpeed : m_fTopSpeed;
			m_rBody.velocity = velocity;
        }

		transform.localScale = fHorizontal == 0 ? transform.localScale : fHorizontal < 0 ? new Vector3(-m_vStartScale.x, m_vStartScale.y, m_vStartScale.z) : new Vector3(m_vStartScale.x, m_vStartScale.y, m_vStartScale.z);
		
		// Jump only if grounded.
		if (m_bGrounded)
		{
			if (Input.GetButtonDown("Jump"))
			{
				m_rBody.AddForce(Vector3.up * m_fJumpForce, ForceMode.VelocityChange);
				m_animator.SetTrigger("Jump");
			}
		}
	}

	bool CheckGrounded()
    {
		return Physics.Raycast(m_bCollider.bounds.center, Vector3.down, m_fHalfHeight + 0.01f, m_lGroundedMask);
    }
}
