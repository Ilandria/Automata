using UnityEngine;

namespace Automata
{
	public class CameraEventProcessor : MonoBehaviour
	{
		[SerializeField]
		private Camera m_camera;

		[SerializeField]
		private Vector2Reference m_cameraMovementDirection;

		[SerializeField]
		private FloatReference m_cameraMoveSpeed;

		[SerializeField]
		private FloatReference m_cameraZoomDirection;

		[SerializeField]
		private FloatReference m_cameraZoomSpeed;

		[SerializeField]
		private Vector2Reference m_cameraFoVLimits;

		public void MoveCamera()
		{
			Vector3 movement = Vector3.zero;
			movement.x = m_cameraMovementDirection.m_value.x * Time.deltaTime;
			movement.y = m_cameraMovementDirection.m_value.y * Time.deltaTime;

			// Movement speed based on FoV so movement doesn't feel too much slower when zoomed out.
			transform.position += movement * m_cameraMoveSpeed.m_value * m_camera.fieldOfView;
		}

		public void ZoomCamera()
		{
			float zoom = m_cameraZoomDirection.m_value * Time.deltaTime * m_cameraZoomSpeed.m_value;
			zoom = Mathf.Clamp(m_camera.fieldOfView + zoom, m_cameraFoVLimits.m_value.x, m_cameraFoVLimits.m_value.y);

			m_camera.fieldOfView = zoom;
		}
	}
}