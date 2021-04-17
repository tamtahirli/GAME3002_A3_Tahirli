using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ObjectInfo
{
    public GameObject obj;
    public Vector3 startPos;
    public Quaternion startRot;
    public Rigidbody rigidbody;
}

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public Rigidbody[] m_aRgbdToReset;
    private List<ObjectInfo> m_aObjectsToReset = new List<ObjectInfo>();

    private void Awake()
    {
        gm = this;
        SetupRigidBodies();
    }

    private void SetupRigidBodies()
    {
        foreach (Rigidbody rgbd in m_aRgbdToReset)
        {
            ObjectInfo newObj = new ObjectInfo();
            newObj.obj = rgbd.gameObject;
            newObj.rigidbody = rgbd;
            newObj.startPos = rgbd.transform.position;
            newObj.startRot = rgbd.transform.rotation;
            m_aObjectsToReset.Add(newObj);

        }
    }

    public static void ResetRigidBodies()
    {
        foreach(ObjectInfo obj in gm.m_aObjectsToReset)
        {
            obj.rigidbody.velocity = Vector3.zero;
            obj.rigidbody.angularVelocity = Vector3.zero;
            obj.rigidbody.angularDrag = 0;
            obj.obj.transform.position = obj.startPos;
            obj.obj.transform.rotation = obj.startRot;
        }
    }
}
