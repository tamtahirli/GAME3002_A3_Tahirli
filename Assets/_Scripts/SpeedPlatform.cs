using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPlatform : MonoBehaviour
{
    [SerializeField] private float m_fSpeedScalar = 5f;
    [SerializeField] private float m_fJumpScalar = 1f;

    private bool m_bOnPlatform = false;

    private void FixedUpdate()
    {
        if(m_bOnPlatform)
        {
            if (m_fSpeedScalar < 1)
            {
                Vector3 vel = Player.Rgbd().velocity;
                vel.x *= m_fSpeedScalar; // Multiply speed if speed scalar is reducing the speed
                Player.Rgbd().velocity = vel;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_bOnPlatform = true;
            Player.SetJumpScale(m_fJumpScalar); // Set jump scale while on platform.
            if (m_fSpeedScalar > 1)
            {
                Player.SetSpeedScale(m_fSpeedScalar); // Just set speed scalar on movement if its over 1
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_bOnPlatform = false;
            Player.SetJumpScale(1);
            Player.SetSpeedScale(1); // Reset scalars
        }
    }
}
