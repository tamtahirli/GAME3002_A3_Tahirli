using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Text m_tTimer, m_tInfoMsg;
    [SerializeField] private float m_fTotalTime = 120;
    [SerializeField] private GameObject[] m_aGameObjectsToShow;

    private float m_fTimer = 0, m_fStartTime;

    private void Awake()
    {
        gm = this;
        SetupRigidBodies();
        m_fStartTime = Time.time;
    }

    private void Update()
    {
        UpdateTimer();
    }

    void UpdateTimer()
    {
        m_fTimer = (Time.time - m_fStartTime);
        float fRemainingTime = Mathf.Floor(m_fTotalTime - m_fTimer);
        m_tTimer.text = fRemainingTime + " seconds remaining";

        if (fRemainingTime <= 0)
        {
            ResetGame();
            SetInfoMsg("You lose! Try again", 2);
        }
    }

    public static void ResetTimer()
    {
        gm.m_fTimer = 0;
        gm.m_fStartTime = Time.time;
    }

    public static void ResetGame()
    {
        ResetTimer();
        ResetRigidBodies();
        Player.Respawn();
        DoorScript.ResetAllDoors();
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

        foreach(GameObject go in m_aGameObjectsToShow)
        {
            ObjectInfo newObj = new ObjectInfo();
            newObj.obj = go;
            newObj.rigidbody = null;
            newObj.startPos = go.transform.position;
            newObj.startRot = go.transform.rotation;
            m_aObjectsToReset.Add(newObj);
        }
    }

    public static void ResetRigidBodies()
    {
        foreach(ObjectInfo obj in gm.m_aObjectsToReset)
        {
            if (obj.rigidbody != null)
            {
                obj.rigidbody.velocity = Vector3.zero;
                obj.rigidbody.angularVelocity = Vector3.zero;
                obj.rigidbody.angularDrag = 0;
            }
            else
            {
                obj.obj.SetActive(true);
            }
            obj.obj.transform.position = obj.startPos;
            obj.obj.transform.rotation = obj.startRot;
        }
    }

    public static void SetInfoMsg(string text, float stopShowAfter = 0)
    {
        if (!gm.m_tInfoMsg.isActiveAndEnabled) ShowInfoMsg();
        gm.m_tInfoMsg.text = text;
        if(stopShowAfter > 0)
        {
            gm.Invoke("HideInfoMsg", stopShowAfter);
        }
    }

    private void HideInfoMsg()
    {
        ShowInfoMsg(false);
    }

    public static void ShowInfoMsg(bool show = true)
    {
        gm.m_tInfoMsg.enabled = show;
    }
}
