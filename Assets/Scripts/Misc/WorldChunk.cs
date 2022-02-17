using UnityEngine;

namespace Automata
{
	public class WorldChunk
	{
		public bool AllTilesEmpty { get { return m_numUsedTiles == 0; } }

		private int[] m_currentTileColours;
		private int[] m_previousTileColours;
		private int m_dimensions;
		private int m_numUsedTiles;

		/// <summary>
		/// World chunks must store their own colour compute buffers instead of setting
		/// one global buffer at render-time due to how Unity batches draw calls.
		/// MaterialPropertyBlocks are used to set which colour buffer is being read from
		/// during an instanced indirect draw call.
		/// </summary>
		private ComputeBuffer m_tileColoursBuffer;

		/// <summary>
		/// Update and get the colours compute buffer for this world chunk. Used for rendering.
		/// </summary>
		public ComputeBuffer TileColoursBuffer
		{
			get
			{
				m_tileColoursBuffer.SetData(m_currentTileColours);
				return m_tileColoursBuffer;
			}
		}

		public WorldChunk(int a_dimensions)
		{
			m_dimensions = a_dimensions;
			m_numUsedTiles = 0;

			int tileCount = a_dimensions * a_dimensions;
			m_currentTileColours = new int[tileCount];
			m_previousTileColours = new int[tileCount];

			for (int i = 0; i < m_currentTileColours.Length; i++)
			{
				m_currentTileColours[i] = 0;
				m_previousTileColours[i] = m_currentTileColours[i];
			}
			
			m_tileColoursBuffer = new ComputeBuffer(m_currentTileColours.Length, sizeof(int));
		}

		public void DestroyChunk()
		{
			m_tileColoursBuffer.Release();
			m_tileColoursBuffer = null;
		}

		/// <summary>
		/// Sets the current tile colour. This does not change the previous value.
		/// </summary>
		/// <param name="a_localCoordinate">2D chunk-local coordinate to set.</param>
		/// <param name="a_colour">Colour index to set to.</param>
		public void SetColour(Vector2Int a_localCoordinate, int a_colour)
		{
			int tileIndex = Coord2Dto1D(a_localCoordinate);
			int prevColour = m_previousTileColours[tileIndex];
			m_currentTileColours[tileIndex] = a_colour;

			// Count how many tiles are currently in use. Used for memory management later.
			if (prevColour != 0 && a_colour == 0)
			{
				m_numUsedTiles--;
			}
			else if (prevColour == 0 && a_colour != 0)
			{
				m_numUsedTiles++;
			}
		}

		/// <summary>
		/// Get a tile's colour. This reads from the previous colour so setting colour
		/// and reading colour are independent on a per-tick basis.
		/// </summary>
		/// <param name="a_localCoordinate"></param>
		/// <returns></returns>
		public int GetColour(Vector2Int a_localCoordinate)
		{
			return m_previousTileColours[Coord2Dto1D(a_localCoordinate)];
		}

		/// <summary>
		/// Updates the previous colour for the given tile.
		/// </summary>
		/// <param name="a_localCoordinate">Which local tile to update.</param>
		public void UpdatePreviousColour(Vector2Int a_localCoordinate)
		{
			int tileIndex = Coord2Dto1D(a_localCoordinate);
			m_previousTileColours[tileIndex] = m_currentTileColours[tileIndex];
		}

		/// <summary>
		/// Updates all previous colours to match the current colour. Used at the end
		/// of a tick.
		/// </summary>
		public void UpdatePreviousColours()
		{
			for(int i = 0; i < m_currentTileColours.Length; i++)
			{
				m_previousTileColours[i] = m_currentTileColours[i];
			}
		}

		private int Coord2Dto1D(Vector2Int a_localCoordinate)
		{
			return a_localCoordinate.x + a_localCoordinate.y * m_dimensions;
		}
	}
}