using UnityEngine;
using UnityEngine.Events;

namespace Automata
{
	public class EventListener : MonoBehaviour
	{
		[SerializeField]
		private Event m_event;

		[SerializeField]
		private UnityEvent m_unityEvent;

		private void OnEnable()
		{
			m_event.Register(this);
		}

		private void OnDisable()
		{
			m_event.Deregister(this);
		}

		public void OnEvent()
		{
			m_unityEvent.Invoke();
		}
	}
}