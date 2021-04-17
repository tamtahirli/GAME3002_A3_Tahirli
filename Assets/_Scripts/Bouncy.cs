using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncy : MonoBehaviour
{
    [SerializeField] private float m_fBounceForce;

    private BoxCollider m_cBox;

    private void Awake()
    {
        m_cBox = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (collision.transform.position.y >= m_cBox.bounds.max.y - 1) // If player hit top
            {
                Vector3 velocity = Player.Rgbd().velocity;
                velocity.y = 0; // Reset Y velocity
                Player.Rgbd().velocity = velocity;
                Player.Rgbd().AddForce(Vector3.up * m_fBounceForce, ForceMode.Impulse); // Bounce up
            }
        }
    }
}
