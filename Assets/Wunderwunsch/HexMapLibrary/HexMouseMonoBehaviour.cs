
using System;
using UnityEngine;

namespace Wunderwunsch.HexMapLibrary
{
	public class HexMouseMonoBehaviour : MonoBehaviour
	{
		/// <summary>
		/// collision plane to cast rays against to get mouse position;
		/// </summary>
		private Plane plane;

		private Action<Vector3> functionToRunOnUpdate;

		public void Init(Action<Vector3> functionToRunOnUpdate)
		{
			this.functionToRunOnUpdate = functionToRunOnUpdate;
			plane = new Plane(Vector3.up, 0);
			functionToRunOnUpdate?.Invoke(GetPlanePosition());
		}
		public virtual void Update()
		{
			functionToRunOnUpdate?.Invoke(GetPlanePosition());
		}

		private Vector3 GetPlanePosition()
		{
			//Debug.Log("plane Normal: " + plane.normal);
			Vector3 mousePos = Input.mousePosition;
			//Debug.Log(mousePos);            

			Ray ray = Camera.main.ScreenPointToRay(mousePos);
			//Debug.Log("Ray origin: " + ray.origin);
			//Debug.Log("Ray direction: " + ray.direction);

			//plane = new Plane(Vector3.up, Camera.main.transform.position.y - Camera.main.nearClipPlane);
			float dist;
			Vector3 point = Vector3.zero;
			plane.Raycast(ray, out dist);
			point = ray.GetPoint(dist);
			//Debug.Log("Plane hitPoint: " + point);
			point = new Vector3(point.x, 0, point.z);
			return point;
		}
	}
}
