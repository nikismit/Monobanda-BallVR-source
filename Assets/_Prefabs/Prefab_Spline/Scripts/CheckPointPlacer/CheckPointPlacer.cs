using UnityEngine;

public class CheckPointPlacer : MonoBehaviour
{

	public BezierSpline spline;

	public int frequency;

	public bool lookForward;

	public Transform[] points;

	private void Awake()
	{
		if (frequency <= 0 || points == null || points.Length == 0)
		{
			return;
		}
		float stepSize = frequency * points.Length;
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
			for (int i = 0; i < points.Length; i++, p++)
			{
				Transform point = Instantiate(points[i]) as Transform;
				Vector3 position = spline.GetPoint(p * stepSize);
				point.transform.localPosition = position;
				if (lookForward)
				{
					point.transform.LookAt(position + spline.GetDirection(p * stepSize));
				}
				point.transform.parent = transform;

				if (!point.gameObject.activeSelf)
					point.gameObject.SetActive(true);
			}
		}
	}
}