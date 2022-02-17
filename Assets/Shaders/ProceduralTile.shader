Shader "Automata/ProceduralTile"
{
	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

		Pass
		{
			Tags
			{
				"LightMode" = "ForwardBase"
			}

			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			CGPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			#pragma target 5.0
			
			#include "UnityCG.cginc"

			uniform StructuredBuffer<int> u_tileColours;
			uniform float2 u_worldCoordinate;
			uniform float u_worldChunkDimensions;
			uniform float u_tileMeshScale;
			uniform float4x4 u_localToWorld;
			uniform float2 u_cameraWorldLookPosition;
			uniform float u_cameraFoV;
			uniform float2 u_distanceScaleStrengths;
			uniform StructuredBuffer<float4> u_availableTileColours;
			uniform float u_elapsedTime;
			uniform float3 u_hoverLocation;

			struct Varyings
			{
				float4 pos : SV_POSITION;
				float4 colour : TEXCOORD2;
			};

			Varyings Vert (appdata_full a_vertex, uint a_instanceID : SV_InstanceID)
			{
				Varyings output;

				float3 localCoordinate;
				localCoordinate.x = a_instanceID % (uint)u_worldChunkDimensions;
				localCoordinate.y = a_instanceID / (uint)u_worldChunkDimensions;
				localCoordinate.z = 0;

				float3 objectPosition = (a_vertex.vertex.xyz * u_tileMeshScale + localCoordinate) / u_worldChunkDimensions;
				float3 worldPosition = float3(u_worldCoordinate.xy, 0) + objectPosition;

				// How far the cursor and this object are from each other.
				// Saturated to make a ring around the cursor.
				float distFromHover = saturate(distance(u_hoverLocation.xy, worldPosition.xy));

				// Scale objects along z when away from camera's center.
				float distFromCamera = 1 + distance(worldPosition.xy, u_cameraWorldLookPosition);
				float lengthScale = (distFromCamera / u_distanceScaleStrengths.x) / u_cameraFoV;
				lengthScale *= u_distanceScaleStrengths.y;
				lengthScale *= distFromHover;

				// Apply offsets to get the final world position.
				objectPosition.z *= lengthScale;
				worldPosition = float3(u_worldCoordinate.xy, 0) + objectPosition;

				// Apply a wave to the tiles. This was mostly to show when paused, but it looks really cool too!
				// TODO: Make the values here uniforms.
				float waveOffset = sin(u_elapsedTime + worldPosition.x) * sin(u_elapsedTime + worldPosition.y) * 0.1;
				waveOffset *= distFromHover;
				worldPosition.z += waveOffset;

				output.colour = u_availableTileColours[u_tileColours[a_instanceID]];
				output.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));

				return output;
			}
			
			fixed4 Frag (Varyings a_fragment) : SV_Target
			{
				return a_fragment.colour;
			}

			ENDCG
		}
	}
}