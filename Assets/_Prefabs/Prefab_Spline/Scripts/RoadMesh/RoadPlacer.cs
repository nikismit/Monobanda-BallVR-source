using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoadPlacer : MonoBehaviour
{

	public BezierSpline spline;

	public int frequency;

	public bool lookForward;

	public Transform[] checkPoints;

	private void Awake()
	{
		if (frequency <= 0 || checkPoints == null || checkPoints.Length == 0)
		{
			return;
		}
		float stepSize = frequency * checkPoints.Length;
		if (spline.Loop || stepSize == 1)
		{
			stepSize = 1f / stepSize;
		}
		else
		{
			stepSize = 1f / (stepSize - 1);
		}

		for (int p = 0, f = 0; f < frequency; f++)
		{
			for (int i = 0; i < checkPoints.Length; i++, p++)
			{
				Transform point = Instantiate(checkPoints[i]) as Transform;
				Vector3 position = spline.GetPoint(p * stepSize);
				point.transform.localPosition = position;
				if (lookForward)
				{
					point.transform.LookAt(position + spline.GetDirection(p * stepSize));
				}
				point.transform.parent = transform;
			}
		}
	}
}