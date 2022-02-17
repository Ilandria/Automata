using System.Collections.Generic;
using UnityEngine;

namespace Automata
{
	[CreateAssetMenu(fileName = "world", menuName = "Automata/World")]
	public class World : ScriptableObject
	{
		public Vector2 WorldTileWidth { get {  return Vector2.one * (1.0f / m_worldChunkDimensions.m_value); } }

		[SerializeField]
		private WorldChunkDictionaryReference m_worldChunkDictionary;

		[SerializeField]
		private Vector2Reference m_lastInteractLocation;

		[SerializeField]
		private IntReference m_worldChunkDimensions;

		[SerializeField]
		private IntReference m_currentCursorColour;

		[SerializeField]
		private Vector2IntArrayReference m_neighbourWorldChunks;

		[SerializeField]
		private Vector2IntArrayReference m_neighbourTiles;

		public void ClearWorld()
		{
			m_worldChunkDictionary.ClearWorld();
		}

		/// <summary>
		/// Sets colour from an interact event.
		/// </summary>
		public void SetColourInteract()
		{
			SetColour(m_lastInteractLocation.m_value, m_currentCursorColour.m_value, true);
		}

		/// <summary>
		/// Sets the colour at a given location and manages surrounding chunk memory.
		/// </summary>
		/// <param name="a_worldLocation">Where to change colour.</param>
		/// <param name="a_colour">Which colour to change to.</param>
		/// <param name="a_shouldUpdatePrevious">If the chunk's previous colours should be updated afterwards.</param>
		public void SetColour(Vector2 a_worldLocation, int a_colour, bool a_shouldUpdatePrevious = false)
		{
			Vector2Int worldCoordinate;
			Vector2Int localCoordinate;
			WorldToChunkLocalCoordPair(a_worldLocation, out worldCoordinate, out localCoordinate);

			// Create the chunk if it didn't exist.
			ValidateChunk(worldCoordinate);

			// Create all neighbouring world chunks. This is needed so colour can grow into new chunks.
			for (int i = 0; i < m_neighbourWorldChunks.m_value.Length; i++)
			{
				ValidateChunk(worldCoordinate + m_neighbourWorldChunks.m_value[i]);
			}

			// Set the colour at the given location.
			WorldChunk worldChunk = m_worldChunkDictionary.m_value[worldCoordinate];

			if (worldChunk != null)
			{
				worldChunk.SetColour(localCoordinate, a_colour);
				
				if (a_shouldUpdatePrevious)
				{
					worldChunk.UpdatePreviousColour(localCoordinate);
				}
			}
		}

		/// <summary>
		/// Get the colour of the tile at the given world location.
		/// </summary>
		/// <param name="a_worldLocation">Where to get the colour from.</param>
		/// <returns>The index of the tile's colour.</returns>
		public int GetColour(Vector2 a_worldLocation)
		{
			Vector2Int worldCoordinate;
			Vector2Int localCoordinate;
			WorldToChunkLocalCoordPair(a_worldLocation, out worldCoordinate, out localCoordinate);

			if (m_worldChunkDictionary.m_value.ContainsKey(worldCoordinate))
			{
				return m_worldChunkDictionary.m_value[worldCoordinate].GetColour(localCoordinate);
			}

			return 0;
		}

		/// <summary>
		/// Get an array of all neighbouring colours. Neighbours are defined by settings objects.
		/// </summary>
		/// <param name="a_worldLocation">Where to get neighbours for.</param>
		/// <returns></returns>
		public int[] GetNeighbouringColours(Vector2 a_worldLocation)
		{
			int[] neighbouringColours = new int[m_neighbourTiles.m_value.Length];

			for (int i = 0; i < m_neighbourTiles.m_value.Length; i++)
			{
				neighbouringColours[i] = GetColour(a_worldLocation + WorldTileWidth * m_neighbourTiles.m_value[i]);
			}

			return neighbouringColours;
		}

		/// <summary>
		/// Updates all previous colours in every chunk within the world.
		/// </summary>
		public void UpdatePreviousColours()
		{
			foreach (WorldChunk worldChunk in m_worldChunkDictionary.m_value.Values)
			{
				worldChunk.UpdatePreviousColours();
			}
		}

		/// <summary>
		/// Processes all world chunks. If a world chunk and its neighbours are all empty, it is deleted.
		/// </summary>
		public void CleanMemory()
		{
			// Add to a list to release so we're not removing from the dictionary as we free memory.
			List<Vector2Int> chunkCoordinatesToRelease = new List<Vector2Int>();

			foreach (KeyValuePair<Vector2Int, WorldChunk> worldChunk in m_worldChunkDictionary.m_value)
			{
				bool shouldRelease = true;

				// If this chunk is empty, check adjacent chunks. We only want to free chunks if all
				// adjacent ones are empty, else tiles won't be able to grow into new chunks.
				if (worldChunk.Value.AllTilesEmpty)
				{
					Vector2Int worldCoordinate;

					for (int i = 0; i < m_neighbourWorldChunks.m_value.Length; i++)
					{
						worldCoordinate = worldChunk.Key + m_neighbourWorldChunks.m_value[i];

						if (m_worldChunkDictionary.m_value.ContainsKey(worldCoordinate))
						{
							if (!m_worldChunkDictionary.m_value[worldCoordinate].AllTilesEmpty)
							{
								shouldRelease = false;
								break;
							}
						}
					}

					// If all adjacent chunks were empty, release memory.
					if (shouldRelease)
					{
						chunkCoordinatesToRelease.Add(worldChunk.Key);
					}
				}
			}

			// Actually free memory.
			for (int i = 0; i < chunkCoordinatesToRelease.Count; i++)
			{
				m_worldChunkDictionary.m_value[chunkCoordinatesToRelease[i]].DestroyChunk();
				m_worldChunkDictionary.m_value.Remove(chunkCoordinatesToRelease[i]);
			}
		}

		/// <summary>
		/// Adds a world chunk at the given world coordinate. Will not create
		/// duplicate world chunks.
		/// </summary>
		/// <param name="a_worldCoordinate">Which chunk to add.</param>
		private void ValidateChunk(Vector2Int a_worldCoordinate)
		{
			if (!m_worldChunkDictionary.m_value.ContainsKey(a_worldCoordinate))
			{
				WorldChunk worldChunk = new WorldChunk(m_worldChunkDimensions.m_value);

				m_worldChunkDictionary.m_value.Add(a_worldCoordinate, worldChunk);
			}
		}

		/// <summary>
		/// Converts a world position into a pair of world chunk coordinates and local chunk coordinates.
		/// </summary>
		/// <param name="a_worldPosition">The position to convert.</param>
		/// <param name="a_worldCoordinate">The resulting world chunk coordinate.</param>
		/// <param name="a_localCoordinate">The resulting tile coordinate local to the chunk.</param>
		private void WorldToChunkLocalCoordPair(Vector2 a_worldPosition, out Vector2Int a_worldCoordinate, out Vector2Int a_localCoordinate)
		{
			int dimensions = m_worldChunkDimensions.m_value;

			// Figure out which world chunk coordinate we're in.
			Vector2Int worldCoordinate = new Vector2Int(
				Mathf.FloorToInt(a_worldPosition.x),
				Mathf.FloorToInt(a_worldPosition.y));

			a_worldCoordinate = worldCoordinate;

			// Figure out which local chunk coordinate we're in.
			Vector2Int localCoordinate = new Vector2Int(
				Mathf.RoundToInt((a_worldPosition.x - worldCoordinate.x) * dimensions),
				Mathf.RoundToInt((a_worldPosition.y - worldCoordinate.y) * dimensions));

			a_localCoordinate = localCoordinate;

			// TODO: This shouldn't be needed, figure out what's going on to cause positive bounds to be = to dimensions.
			if(a_localCoordinate.x == dimensions)
			{
				a_localCoordinate.x = 0;
				a_worldCoordinate.x++;
			}

			if (a_localCoordinate.y == dimensions)
			{
				a_localCoordinate.y = 0;
				a_worldCoordinate.y++;
			}
		}
	}
}