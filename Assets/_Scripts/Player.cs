using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
	public static Player player;

	[SerializeField] private Animator m_animator = null;
	[SerializeField] private float m_fMovementSpeed = 1f;
    [SerializeField] private float m_fTopSpeed = 20f;
    [SerializeField] private float m_fJumpForce = 5f;
	[SerializeField] private float m_fSlowDownScale = 0.99f;
	[SerializeField] private float m_fGroundedThreshold = 0.05f;
	[SerializeField] private LayerMask m_lGroundedMask = new LayerMask();

	private static Vector3 m_vStartPos = Vector3.zero;
	private Rigidbody m_rBody = null;
	private BoxCollider m_bCollider = null;
	private float m_fHalfHeight = 0;
	private Vector3 m_vStartScale = Vector3.zero;
	[SerializeField] private bool m_bGrounded = false;

    void Awake()
    {
		m_rBody = GetComponent<Rigidbody>();
		m_bCollider = GetComponent<BoxCollider>();
		m_animator = GetComponentInChildren<Animator>();
		m_fHalfHeight = m_bCollider.bounds.extents.y;
		m_vStartScale = transform.localScale;
		m_vStartPos = transform.position;
		player = this;
	}

    private void FixedUpdate()
    {
		m_bGrounded = CheckGrounded();
		m_animator.SetBool("Grounded", m_bGrounded);

		float fHorizontal = Input.GetAxis("Horizontal");

		m_rBody.AddForce(Vector3.right * fHorizontal, ForceMode.VelocityChange); // Add force on rigid body based on horizontal movement

		//m_rBody.velocity *= m_fSlowDownScale; // Slow down player every frame to avoid slideyness

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

    private void OnCollisionStay(Collision collision)
    {
		m_bGrounded = true; // This type of game benefits from being able to jump when against an object.
    }

    bool CheckGrounded()
    {
		return Physics.Raycast(m_bCollider.bounds.center, Vector3.down, m_fHalfHeight + m_fGroundedThreshold, m_lGroundedMask);
    }

	public static void Respawn()
    {
		player.transform.position = m_vStartPos;
    }
}
