using System.Collections.Generic;
using UnityEngine;

namespace Automata
{
	[CreateAssetMenu(fileName = "Event", menuName = "Automata/Event")]
	public class Event : ScriptableObjectReference<List<EventListener>>
	{
		/// <summary>
		/// This hash set is used to ensure only one copy of a given event listener
		/// is ever added to the registered listeners value list.
		/// </summary>
		private HashSet<EventListener> m_registeredListeners;

		/// <summary>
		/// Make sure objects can only register and deregister themselves while this
		/// object is operational.
		/// </summary>
		private bool m_isEnabled = false;

		public void Invoke()
		{
			if (m_isEnabled)
			{
				for (int i = m_value.Count - 1; i >= 0; i--)
				{
					m_value[i].OnEvent();
				}
			}
		}

		public void Register(EventListener a_listener)
		{
			if (m_isEnabled)
			{
				if (m_registeredListeners.Add(a_listener))
				{
					m_value.Add(a_listener);
				}
			}
		}

		public void Deregister(EventListener a_listener)
		{
			if (m_isEnabled)
			{
				if (m_registeredListeners.Remove(a_listener))
				{
					m_value.Remove(a_listener);
				}
			}
		}

		private void OnEnable()
		{
			m_value = new List<EventListener>();
			m_registeredListeners = new HashSet<EventListener>();
			m_isEnabled = true;
		}

		private void OnDisable()
		{
			m_value = null;
			m_registeredListeners = null;
			m_isEnabled = false;
		}
	}
}