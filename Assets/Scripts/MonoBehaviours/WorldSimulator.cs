using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Automata
{
	public class WorldSimulator : MonoBehaviour
	{
		[SerializeField]
		private World m_world;

		[SerializeField]
		private WorldChunkDictionaryReference m_worldChunkDictionary;

		[SerializeField]
		private IntReference m_worldChunkDimensions;

		[SerializeField]
		private FloatReference m_timeBetweenTicks;

		[SerializeField]
		private BoolReference m_isPlaying;

		[SerializeField]
		private ColourMapReference m_availableTileColours;

		[SerializeField]
		private TileKernelReference m_currentTileKernel;

		[SerializeField]
		private FloatReference m_elapsedSimulationTime;

		[SerializeField]
		private ParticleSystem m_particleSystem;

		private float m_timeUntilTick = 0;

		private void Update()
		{
			if (m_isPlaying.m_value)
			{
				m_timeUntilTick -= Time.deltaTime;
				m_elapsedSimulationTime.m_value += Time.deltaTime;

				if (m_timeUntilTick <= 0)
				{
					m_timeUntilTick += m_timeBetweenTicks.m_value;

					TickSimulation();

					m_world.UpdatePreviousColours();

					m_world.CleanMemory();
				}
			}
			else
			{
				m_timeUntilTick = m_timeBetweenTicks.m_value;
			}
		}

		public void ToggleIsPlaying()
		{
			m_isPlaying.m_value = !m_isPlaying.m_value;
		}

		public void PausePlaying()
		{
			m_isPlaying.m_value = false;
		}

		public void ResumePlaying()
		{
			m_isPlaying.m_value = true;
		}

		public void QuitGame()
		{
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		/// <summary>
		/// Advances the simulation by one tick. TODO: This really should be multi-threaded!
		/// </summary>
		private void TickSimulation()
		{
			int numAvailableColours = m_availableTileColours.m_value.Length;
			Vector2 worldLocation = Vector2.zero;
			int tileColour;
			int[] neighbourColours;

			List<Vector2Int> chunksToProcess = new List<Vector2Int>();
			EmitParams emitParams = new EmitParams();

			// Build a list before processing since we'll be adding to the world dictionary during writes.
			foreach (Vector2Int chunkCoordinate in m_worldChunkDictionary.m_value.Keys)
			{
				chunksToProcess.Add(chunkCoordinate);
			}

			for(int i = 0; i < chunksToProcess.Count; i++)
			{
				for (int x = 0; x < m_worldChunkDimensions.m_value; x++)
				{
					for (int y = 0; y < m_worldChunkDimensions.m_value; y++)
					{
						// Get the location of the tile to process, its current colour, and neighbouring colours.
						worldLocation = chunksToProcess[i] + m_world.WorldTileWidth * new Vector2(x, y);
						tileColour = m_world.GetColour(worldLocation);
						neighbourColours = m_world.GetNeighbouringColours(worldLocation);

						// Run the kernel per tile.
						int newColour = m_currentTileKernel.m_value(tileColour, neighbourColours, numAvailableColours);

						// Don't process anything if the tile didn't change. This is a massive performance boost.
						if (tileColour != newColour)
						{
							m_world.SetColour(worldLocation, newColour);
							emitParams.startColor = m_availableTileColours.m_value[newColour];
							emitParams.position = worldLocation;
							m_particleSystem.Emit(emitParams, 1);
						}
					}
				}
			}
		}
	}
}