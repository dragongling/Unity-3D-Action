using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
	[SerializeField]
	Transform focus = default;

	[SerializeField, Range(1f, 20f)]
	float distance = 5f;

	[SerializeField, Min(0f)]
	float focusRadius = 1f;

	[SerializeField, Range(0f, 1f)]
	float focusCentering = 0.5f;

	[SerializeField, Range(1f, 360f)]
	float rotationSpeed = 90f;

	[SerializeField, Range(-89f, 89f)]
	float minVerticalAngle = -30f, maxVerticalAngle = 60f;

	Vector3 focusPoint;
	Vector2 orbitAngles = new Vector2(45f, 0f);

    private void OnValidate()
    {
		if (maxVerticalAngle < minVerticalAngle)
		{
			maxVerticalAngle = minVerticalAngle;
		}
	}

    void Awake()
	{
		focusPoint = focus.position;
		transform.localRotation = Quaternion.Euler(orbitAngles);
	}

	void LateUpdate()
	{
		UpdateFocusPoint();
		Quaternion lookRotation;
        if (ManualRotation())
        {
			ConstrainAngles();
			lookRotation = Quaternion.Euler(orbitAngles);
        }
        else
        {
			lookRotation = transform.localRotation;
        }
		Vector3 lookDirection = lookRotation * Vector3.forward;
		Vector3 lookPosition = focusPoint - lookDirection * distance;
		transform.SetPositionAndRotation(lookPosition, lookRotation);
	}

    private void UpdateFocusPoint()
    {
		Vector3 targetPoint = focus.position;
		if (focusRadius > 0f)
		{
			float distance = Vector3.Distance(targetPoint, focusPoint);
			float t = 1f;
			if (distance > 0.01f && focusCentering > 0f)
			{
				t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
			}
			if (distance > focusRadius)
			{
				t = Mathf.Min(t, focusRadius / distance);
			}
			focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
		}
		else
		{
			focusPoint = targetPoint;
		}
	}

	bool ManualRotation()
	{
		Vector2 input = new Vector2(
			Input.GetAxis("Vertical Camera"),
			Input.GetAxis("Horizontal Camera")
		);
		const float e = 0.001f;
		if (input.x < -e || input.x > e || input.y < -e || input.y > e)
		{
			orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
			return true;
		}
		return false;
	}

	void ConstrainAngles()
	{
		orbitAngles.x =
			Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

		if (orbitAngles.y < 0f)
		{
			orbitAngles.y += 360f;
		}
		else if (orbitAngles.y >= 360f)
		{
			orbitAngles.y -= 360f;
		}
	}
}
