using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringArm : MonoBehaviour
{
    [SerializeField] private Transform m_target = null;

	[SerializeField] private Vector3 m_vTargetOffset = Vector3.zero;
    [SerializeField] private float m_fCameraLagSpeed = 10.0f;
    [SerializeField] private float m_fCameraLagMaxTimeStep = 1.0f / 60.0f;
    [SerializeField] private float m_fCameraLagMaxDistance = 0.0f;

	/** Temporary variables when using camera lag, to record previous camera position */
	private Vector3 m_fPreviousDesiredPos;

    private void Start()
    {
		UpdateSpringArm(0);
    }

    private void FixedUpdate()
    {
		UpdateSpringArm(Time.deltaTime);
    }

	void UpdateSpringArm(float DeltaTime)
	{
		// Get the spring arm 'origin', the target we want to look at
		Vector3 ArmOrigin = m_target.position + m_vTargetOffset;
		Vector3 DesiredLoc = ArmOrigin;

		// Lerp target offset with lag like a spring
		if (DeltaTime > m_fCameraLagMaxTimeStep && m_fCameraLagSpeed > 0.0f)
		{
			Vector3 ArmMovementStep = (DesiredLoc - m_fPreviousDesiredPos) * (1.0f / DeltaTime);
			Vector3 LerpTarget = m_fPreviousDesiredPos;

			float RemainingTime = DeltaTime;
			while (RemainingTime > Mathf.Epsilon)
			{
				float LerpAmount = Mathf.Min(m_fCameraLagMaxTimeStep, RemainingTime);
				LerpTarget += ArmMovementStep * LerpAmount;
				RemainingTime -= LerpAmount;

				DesiredLoc = Vector3.Lerp(m_fPreviousDesiredPos, LerpTarget, LerpAmount * m_fCameraLagSpeed);
				m_fPreviousDesiredPos = DesiredLoc;
			}
		}
		else
		{
			DesiredLoc = Vector3.Lerp(m_fPreviousDesiredPos, DesiredLoc, DeltaTime * m_fCameraLagSpeed);
		}

		// Clamp distance
		if (m_fCameraLagMaxDistance > 0.0f)
		{
			Vector3 FromOrigin = DesiredLoc - ArmOrigin;
			if (FromOrigin.magnitude > (m_fCameraLagMaxDistance * m_fCameraLagMaxDistance))
			{
				DesiredLoc = ArmOrigin + GetClampedToMaxSize(FromOrigin, m_fCameraLagMaxDistance);
			}

		}

		m_fPreviousDesiredPos = DesiredLoc;

		transform.position = DesiredLoc;
	}

	Vector3 GetClampedToMaxSize(Vector3 vector, float MaxSize)
	{
		if (MaxSize < Mathf.Epsilon)
		{
			return Vector3.zero;
		}

		float VSq = vector.magnitude;
		if (VSq > MaxSize * MaxSize)
		{
			float Scale = MaxSize * Mathf.Sqrt(VSq);
			return new Vector3(vector.x * Scale, vector.y * Scale, vector.z * Scale);
		}
		else
		{
			return vector;
		}	
	}
}
