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
	private int m_iKeys = 0;
	[SerializeField] private bool m_bGrounded = false;

	private float m_fJumpScale = 1, m_fSpeedScale = 1;

    void Awake()
    {
		m_rBody = GetComponent<Rigidbody>();
		m_bCollider = GetComponent<BoxCollider>();
		m_animator = GetComponentInChildren<Animator>();
		m_fHalfHeight = m_bCollider.bounds.extents.y;
		m_vStartPos = transform.position;
		player = this;
	}

	bool pressedJump = false;

	private void Update()
	{
		if (Input.GetButtonDown("Jump")) // FixedUpdate proved real bad for detecting button down.
		{
			pressedJump = true; // Used in FixedUpdate
		}
	}

    private void FixedUpdate()
    {
		m_bGrounded = CheckGrounded();
		m_animator.SetBool("Grounded", m_bGrounded);

		float fHorizontal = Input.GetAxisRaw("Horizontal");

		m_rBody.AddForce(Vector3.right * fHorizontal * m_fMovementSpeed * m_fSpeedScale, ForceMode.VelocityChange); // Add force on rigid body based on horizontal movement

		//m_rBody.velocity *= m_fSlowDownScale; // Slow down player every frame to avoid slideyness

		float fAbsX = Mathf.Abs(m_rBody.velocity.x);
		m_animator.SetFloat("Movement", fAbsX / m_fTopSpeed); // Set animator float from scale 0-1.

		Vector3 velocity = m_rBody.velocity;
		if (fAbsX > m_fTopSpeed) // Only use top speed for lateral movement
        {
			velocity.x = velocity.x < 0 ? -m_fTopSpeed : m_fTopSpeed;
			m_rBody.velocity = velocity;
        }

		// Rotate on movement
		transform.rotation = fHorizontal == 0 ? transform.rotation : fHorizontal < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
		
		// Jump only if grounded.
		if (m_bGrounded)
		{
			if (pressedJump)
			{
				velocity.y = 0;
				m_rBody.velocity = velocity; // Reset Y velocity
				m_rBody.AddForce(Vector3.up * m_fJumpForce * m_fJumpScale, ForceMode.VelocityChange); // Set Y velocity by force
				m_animator.SetTrigger("Jump");
			}
		}

		pressedJump = false; // Reset so late jumps aren't a thing
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Key"))
        {
			other.gameObject.SetActive(false);
			m_iKeys++;
			GameManager.SetInfoMsg("You found a key!", 2);
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
		player.m_rBody.velocity = Vector3.zero;
		player.m_rBody.angularVelocity = Vector3.zero;
		player.transform.rotation = Quaternion.Euler(0, 0, 0);
		player.m_iKeys = 0;
    }

	public static Rigidbody Rgbd()
    {
		return player.m_rBody;
    }

	public static Animator GetAnimator()
    {
		return player.m_animator;
    }

	public static void SetJumpScale(float scale)
    {
		player.m_fJumpScale = scale;
    }
	
	public static void SetSpeedScale(float scale)
    {
		player.m_fSpeedScale = scale;
    }

	public static void AddKey()
    {
		player.m_iKeys++;
    }

	public static void RemoveKey()
    {
		player.m_iKeys--;
    }

	public static int GetKeys()
    {
		return player.m_iKeys;
    }
}
