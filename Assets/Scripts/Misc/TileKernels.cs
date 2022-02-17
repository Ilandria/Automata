namespace Automata
{
	public static class TileKernels
	{
		public enum TileKernelNames
		{
			Grow, Mix, Dissolve
		}

		public static int Grow(int a_currentColour, int[] a_neighbourColours, int a_numAvailableColours)
		{
			int[] neighbourColourCount = new int[a_numAvailableColours];

			for (int i = 0; i < a_neighbourColours.Length; i++)
			{
				neighbourColourCount[a_neighbourColours[i]]++;
			}

			int highestColour = 0;
			int highestColourCount = 0;

			for (int i = 1; i < a_numAvailableColours; i++)
			{
				if (neighbourColourCount[i] > highestColourCount)
				{
					highestColourCount = neighbourColourCount[i];
					highestColour = i;
				}
			}

			return highestColour;
		}

		public static int Mix(int a_currentColour, int[] a_neighbourColours, int a_numAvailableColours)
		{
			int[] neighbourColourCount = new int[a_numAvailableColours];

			for (int i = 0; i < a_neighbourColours.Length; i++)
			{
				// Weight towards higher colours, +1 is so element 0 still gets some weight.
				neighbourColourCount[a_neighbourColours[i]] += a_neighbourColours[i] <= 3 ? 1 : 3;
			}

			int commonColour = 0;
			int commonColourCount = 0;
			int uniqueColourCount = 0;
			
			for (int i = 1; i < neighbourColourCount.Length; i++)
			{
				if (neighbourColourCount[i] > commonColourCount)
				{
					commonColourCount = neighbourColourCount[i];
					commonColour = i;
				}

				if (neighbourColourCount[i] > 0 && i != a_currentColour)
				{
					uniqueColourCount++;
				}
			}

			// If there's too much dead space nearby, the cell dies. Secondary colour cells are much more sensitive to space.
			if (neighbourColourCount[0] >= 5 || (neighbourColourCount[0] >= 2 && commonColour >= 4))
			{
				return 0;
			}

			if (a_currentColour != 0)
			{
				// If the most common colour is the current colour and there's not a single other colour, stay the same colour.
				if (commonColour == a_currentColour && uniqueColourCount == 0 && commonColourCount >= 4)
				{
					return a_currentColour;
				}

				uint currentColourBit = (uint)1 << a_currentColour;
				uint commonColourBit = (uint)1 << commonColour;
				uint currentOrCommon = currentColourBit | commonColourBit;

				// Red + Green = Yellow | 1 and 2 in colour index, 110 in binary.
				if (currentOrCommon == 6)
					return 6;

				// Blue + Red = Magenta | 3 and 1 in colour index, 1010 in binary.
				else if (currentOrCommon == 10)
					return 5;

				// Green + Blue = Cyan | 2 and 3 in colour index, 1100 in binary.
				else if (currentOrCommon == 12)
					return 4;

				// If two different secondaries collide, destroy them.
				else if (a_currentColour > 3 && commonColour > 3 && a_currentColour != commonColour)
					return 0;
			}

			return commonColour;
		}

		public static int Dissolve(int a_currentColour, int[] a_neighbourColours, int a_numAvailableColours)
		{
			int[] neighbourColourCount = new int[a_numAvailableColours];

			for (int i = 0; i < a_neighbourColours.Length; i++)
			{
				neighbourColourCount[a_neighbourColours[i]]++;
			}

			int highestColour = 0;
			int highestColourCount = 0;

			for (int i = 0; i < a_numAvailableColours; i++)
			{
				if (neighbourColourCount[i] > highestColourCount)
				{
					highestColourCount = neighbourColourCount[i];
					highestColour = i;
				}
			}

			return highestColour;
		}
	}
}