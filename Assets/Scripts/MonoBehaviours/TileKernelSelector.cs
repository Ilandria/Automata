using UnityEngine;

namespace Automata
{
	public class TileKernelSelector : MonoBehaviour
	{
		[SerializeField]
		private TileKernels.TileKernelNames m_kernel;

		[SerializeField]
		private TileKernelReference m_currentKernel;

		private void Awake()
		{
			switch(m_kernel)
			{
				case TileKernels.TileKernelNames.Grow:
					m_currentKernel.m_value = TileKernels.Grow;
					break;

				case TileKernels.TileKernelNames.Mix:
					m_currentKernel.m_value = TileKernels.Mix;
					break;

				case TileKernels.TileKernelNames.Dissolve:
					m_currentKernel.m_value = TileKernels.Dissolve;
					break;
			}
		}
	}
}