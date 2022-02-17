using UnityEngine;

namespace Automata
{
	/// <summary>
	/// This class generates input events. Normally I'd split this up into an "input type" system
	/// where you can register functionality through a more modular system, but that's out of
	/// scope for this project.
	/// </summary>
	public class InputEventGenerator : MonoBehaviour
	{
		[SerializeField]
		private string m_interactAxisName = "Interact";

		[SerializeField]
		private string m_horizontalMoveAxisName = "HorizontalMove";

		[SerializeField]
		private string m_verticalMoveAxisName = "VerticalMove";

		[SerializeField]
		private string m_zoomAxisName = "Zoom";

		[SerializeField]
		private Event m_onInteractHeld;

		[SerializeField]
		private Event m_onInteractDown;

		[SerializeField]
		private Event m_onInteractUp;

		private bool m_interactIsUp = true;

		[SerializeField]
		private Event m_onBeginPlay;

		[SerializeField]
		private Event m_cameraMove;

		[SerializeField]
		private Vector2Reference m_cameraMovementInput;

		[SerializeField]
		private Camera m_currentCamera;

		[SerializeField]
		private Event m_cameraZoomEvent;

		[SerializeField]
		private FloatReference m_cameraZoom;

		[SerializeField]
		private KeyCodeReference m_cursorColourKey;

		[SerializeField]
		private IntReference m_currentCursorColour;

		[SerializeField]
		private Event m_cursorColourChangedEvent;

		[SerializeField]
		private KeyCodeReference m_toggleMouseKey;

		[SerializeField]
		private KeyCodeReference m_togglePlayingKey;

		[SerializeField]
		private Event m_togglePlayingEvent;

		[SerializeField]
		private KeyCodeReference m_clearWorldKey;

		[SerializeField]
		private Event m_clearWorldEvent;

		[SerializeField]
		private KeyCodeReference m_quitGameKey;

		[SerializeField]
		private Event m_quitGameEvent;

		private void Update()
		{
			ProcessMove();
			ProcessInteract();
			ProcessZoom();
			ProcessCursorColourCycle();
			ProcessToggleMouse();
			ProcessTogglePlaying();
			ProcessClearWorld();
			ProcessQuitGame();
		}

		private void Start()
		{
			m_onBeginPlay.Invoke();
			Cursor.visible = false;
		}

		private void ProcessMove()
		{
			Vector2 movementInput = new Vector2(
				Input.GetAxis(m_horizontalMoveAxisName),
				Input.GetAxis(m_verticalMoveAxisName));
			
			m_cameraMovementInput.m_value = movementInput;

			m_cameraMove.Invoke();
		}

		private void ProcessInteract()
		{
			float interactAxis = Input.GetAxis(m_interactAxisName);

			if (m_interactIsUp)
			{
				if (interactAxis >= 0.5f)
				{
					m_onInteractDown.Invoke();
					m_interactIsUp = false;
				}
			}
			else
			{
				if (interactAxis < 0.5f)
				{
					m_onInteractUp.Invoke();
					m_interactIsUp = true;
				}
				else
				{
					m_onInteractHeld.Invoke();
				}
			}
		}

		private void ProcessZoom()
		{
			float zoomAxis = Input.GetAxis(m_zoomAxisName);
			m_cameraZoom.m_value = zoomAxis;

			m_cameraZoomEvent.Invoke();
		}

		private void ProcessCursorColourCycle()
		{
			if (Input.GetKeyDown(m_cursorColourKey.m_value))
			{
				m_currentCursorColour.m_value++;
				m_cursorColourChangedEvent.Invoke();
			}
		}

		private void ProcessToggleMouse()
		{
			if (Input.GetKeyDown(m_toggleMouseKey.m_value))
			{
				Cursor.visible = !Cursor.visible;
			}
		}

		private void ProcessTogglePlaying()
		{
			if (Input.GetKeyDown(m_togglePlayingKey.m_value))
			{
				m_togglePlayingEvent.Invoke();
			}
		}

		private void ProcessClearWorld()
		{
			if (Input.GetKeyDown(m_clearWorldKey.m_value))
			{
				m_clearWorldEvent.Invoke();
			}
		}

		private void ProcessQuitGame()
		{
			if (Input.GetKeyDown(m_quitGameKey.m_value))
			{
				m_quitGameEvent.Invoke();
			}
		}
	}
}