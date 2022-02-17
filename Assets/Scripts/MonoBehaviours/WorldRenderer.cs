using System.Collections.Generic;
using UnityEngine;

namespace Automata
{
	public class WorldRenderer : MonoBehaviour
	{
		[SerializeField]
		private WorldChunkDictionaryReference m_worldChunkDictionary;

		[SerializeField]
		private MeshReference m_tileMesh;

		[SerializeField]
		private IntReference m_worldChunkDimensions;

		[SerializeField]
		private FloatReference m_tileMeshScale;

		[SerializeField]
		private Material m_tileMaterial;

		[SerializeField]
		private Camera m_camera;

		[SerializeField]
		private Vector2Reference m_worldHoverLocation;

		[SerializeField]
		private Vector2Reference m_distanceScaleStrengths;

		[SerializeField]
		private ColourMapReference m_availableTileColours;

		[SerializeField]
		private FloatReference m_elapsedSimulationTime;

		private ComputeBuffer m_availableColoursBuffer;
		private ComputeBuffer m_indirectArgsBuffer;
		private uint[] m_indirectArgs;

		/// <summary>
		/// The width, height, and depth of the bounds around one world chunk - takes dimensions into account.
		/// </summary>
		private Vector3 m_drawBoundsSize;

		/// <summary>
		/// Offset to center the drawing bounds within a world chunk.
		/// </summary>
		private Vector2 m_drawBoundsOffset;

		private void Start()
		{
			// Pre-calculate a bunch of stuff for chunk render bounds to prevent incorrect culling.
			int tileCount = m_worldChunkDimensions.m_value * m_worldChunkDimensions.m_value;
			m_drawBoundsSize = new Vector3(1, 1, 1.0f / m_worldChunkDimensions.m_value);
			m_drawBoundsOffset = Vector2.one * 0.5f;

			// Indirect args are used to tell the GPU how many instances of each mesh to render,
			// and where their indices/vertices start in the mesh's submeshes (which we're not using, hence the 0s).
			m_indirectArgs = new uint[5] { 0, 0, 0, 0, 0 };
			m_indirectArgs[0] = m_tileMesh.m_value.GetIndexCount(0);
			m_indirectArgs[1] = (uint)tileCount;
			m_indirectArgs[2] = m_tileMesh.m_value.GetIndexStart(0);
			m_indirectArgs[3] = m_tileMesh.m_value.GetBaseVertex(0);

			m_indirectArgsBuffer = new ComputeBuffer(1, m_indirectArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
			m_indirectArgsBuffer.SetData(m_indirectArgs);

			// 4 float elements per colour struct.
			m_availableColoursBuffer = new ComputeBuffer(m_availableTileColours.m_value.Length, sizeof(float) * 4);
			m_availableColoursBuffer.SetData(m_availableTileColours.m_value);

			m_tileMaterial.SetFloat("u_worldChunkDimensions", m_worldChunkDimensions.m_value);
			m_tileMaterial.SetFloat("u_tileMeshScale", m_tileMeshScale.m_value);
			m_tileMaterial.SetBuffer("u_availableTileColours", m_availableColoursBuffer);
		}

		private void LateUpdate()
		{
			m_tileMaterial.SetVector("u_cameraWorldLookPosition", m_camera.transform.position);
			m_tileMaterial.SetVector("u_hoverLocation", m_worldHoverLocation.m_value);
			m_tileMaterial.SetFloat("u_cameraFoV", m_camera.fieldOfView);
			m_tileMaterial.SetVector("u_distanceScaleStrengths", m_distanceScaleStrengths.m_value);
			m_tileMaterial.SetFloat("u_elapsedTime", m_elapsedSimulationTime.m_value);

			foreach (KeyValuePair<Vector2Int, WorldChunk> worldChunk in m_worldChunkDictionary.m_value)
			{
				MaterialPropertyBlock materialProperties = new MaterialPropertyBlock();
				materialProperties.SetVector("u_worldCoordinate", (Vector2)worldChunk.Key);
				materialProperties.SetBuffer("u_tileColours", worldChunk.Value.TileColoursBuffer);

				/// Needed so chunks are only culled when outside of the camera's FoV.
				Bounds drawBounds = new Bounds(worldChunk.Key + m_drawBoundsOffset, m_drawBoundsSize);

				Graphics.DrawMeshInstancedIndirect(m_tileMesh.m_value, 0, m_tileMaterial, drawBounds, m_indirectArgsBuffer, 0, materialProperties);
			}
		}

		private void OnDestroy()
		{
			m_indirectArgsBuffer.Release();
			m_availableColoursBuffer.Release();
		}
	}
}