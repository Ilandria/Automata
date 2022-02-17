using System.Collections.Generic;
using UnityEngine;

namespace Automata
{
	[CreateAssetMenu(fileName = "WorldChunkDictionary", menuName = "Automata/WorldChunkDictionary")]
	public class WorldChunkDictionaryReference : ScriptableObjectReference<Dictionary<Vector2Int, WorldChunk>>
	{
		private void OnEnable()
		{
			m_value = new Dictionary<Vector2Int, WorldChunk>();
		}

		private void OnDisable()
		{
			ClearWorld();
		}

		private void OnDestroy()
		{
			ClearWorld();
		}

		public void ClearWorld()
		{
			foreach (WorldChunk worldChunk in m_value.Values)
			{
				worldChunk.DestroyChunk();
			}

			m_value?.Clear();
		}
	}
}