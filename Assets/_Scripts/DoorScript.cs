using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private float m_fDoorOpenForce = 100f, m_fTargetVel = 100f, m_fMinLimit = 0, m_fMaxLimit = 90;
    [SerializeField] private BoxCollider m_cBox;

    private HingeJoint m_hJoint = null;
    private bool m_bOpened = false;

    private JointLimits m_jLimits = new JointLimits();

    void Start()
    {
        m_hJoint = GetComponentInChildren<HingeJoint>();

        // Joint motor will work great to make it open with a lerp.
        JointMotor jOpenMotor = new JointMotor();
        jOpenMotor.targetVelocity = m_fTargetVel;
        jOpenMotor.force = m_fDoorOpenForce;
        m_hJoint.motor = jOpenMotor;

        // Set limits to 0 so it doesn't open
        m_jLimits.min = 0;
        m_jLimits.max = 0;
        m_hJoint.limits = m_jLimits;
        m_hJoint.useLimits = true;
    }

    public void OpenDoor()
    {
        // Set limits so it only opens 90 degrees or whatever is set in editor
        m_jLimits.min = m_fMinLimit;
        m_jLimits.max = m_fMaxLimit;
        m_hJoint.limits = m_jLimits;

        m_hJoint.useMotor = true; // Start using the motor to open the door
        m_cBox.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_bOpened) return;
        if(other.CompareTag("Player"))
        {
            if(Player.GetKeys() > 0)
            {
                Player.RemoveKey();
                m_bOpened = true;
                OpenDoor();
            }
            else
            {
                GameManager.SetInfoMsg("You need a key!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_bOpened) return;
        if (other.CompareTag("Player"))
        {
            GameManager.ShowInfoMsg(false);
        }
    }

    private void ResetDoor()
    {
        m_hJoint.useMotor = false;
        m_jLimits.min = 0;
        m_jLimits.max = 0;
        m_hJoint.limits = m_jLimits;
        m_cBox.enabled = true;
        m_bOpened = false;
    }

    public static void ResetAllDoors()
    {
        foreach(DoorScript door in FindObjectsOfType<DoorScript>())
        {
            door.ResetDoor();
        }
    }
}
