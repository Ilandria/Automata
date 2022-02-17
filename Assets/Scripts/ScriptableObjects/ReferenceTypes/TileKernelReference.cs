using UnityEngine;
using UnityEditor;

namespace Automata
{
	/// <summary>
	/// Used to define which tile kernel to use. Parameters are current tile colour, neighbour tile colours,
	/// total number of available colours. Return type is the final resulting colour.
	/// </summary>
	[CreateAssetMenu(fileName = "TileKernel", menuName = "Automata/TileKernel")]
	public class TileKernelReference : ScriptableObjectReference<System.Func<int, int[], int, int>>
	{

	}
}