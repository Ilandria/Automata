using UnityEngine;

namespace Automata
{
	public class CursorEventProcessor : MonoBehaviour
	{
		[SerializeField]
		private Vector2Reference m_currentWorldHoverLocation;

		[SerializeField]
		private IntReference m_worldChunkDimensions;

		[SerializeField]
		private GameObject m_cursorRenderer;

		[SerializeField]
		private Camera m_currentCamera;

		private Material m_cursorMaterial;

		[SerializeField]
		private FloatReference m_cursorMoveSpeed;

		[SerializeField]
		private ColourMapReference m_availableTileColours;

		[SerializeField]
		private IntReference m_currentCursorColour;

		[SerializeField]
		private IntReference m_maxCursorColour;

		private Vector3 m_targetPosition;

		[SerializeField]
		private Vector2Reference m_lastInteractLocation;

		private void OnEnable()
		{
			m_targetPosition = transform.position;
			m_cursorRenderer.transform.localScale = Vector3.one / m_worldChunkDimensions.m_value;
			m_cursorMaterial = m_cursorRenderer.GetComponent<MeshRenderer>().material;
		}

		private void Update()
		{
			Vector3 screenMousePosition = Input.mousePosition;
			screenMousePosition.z = -m_currentCamera.transform.position.z;

			m_targetPosition = m_currentCamera.ScreenToWorldPoint(screenMousePosition) * m_worldChunkDimensions.m_value;
			m_targetPosition.x = Mathf.Round(m_targetPosition.x);
			m_targetPosition.y = Mathf.Round(m_targetPosition.y);
			m_targetPosition /= m_worldChunkDimensions.m_value;

			transform.position = Vector3.Lerp(transform.position, m_targetPosition, Time.deltaTime * m_cursorMoveSpeed.m_value);
			m_currentWorldHoverLocation.m_value = m_targetPosition;
		}

		public void CursorColourChanged()
		{
			m_currentCursorColour.m_value = m_currentCursorColour.m_value % m_maxCursorColour.m_value;
			Color cursorColour = m_availableTileColours.m_value[m_currentCursorColour.m_value];
			cursorColour.a = 1;

			m_cursorMaterial.color = cursorColour;
		}

		public void SetLastInteractLocation()
		{
			m_lastInteractLocation.m_value = m_currentWorldHoverLocation.m_value;
		}
	}
}