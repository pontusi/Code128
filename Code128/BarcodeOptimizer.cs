namespace Code128
{
	/// <summary>
	/// Takes text and for each character chooses code set so that the final barcode will be as short as possible.
	/// </summary>
	internal sealed class BarcodeOptimizer
	{
		private string Text { get; }

		private int[,] Costs { get; }

		private int[,] NextIndex { get; }

		private Codeset[,] NextCodeset { get; }

		public static Codeset[] ChooseCodesets(string text)
		{
			var optimizer = new BarcodeOptimizer(text);
			optimizer.ComputeCosts();
			return optimizer.GeneratePath();
		}

		private BarcodeOptimizer(string text)
		{
			this.Text = text;
			this.Costs = new int[3, text.Length + 1];
			this.NextIndex = new int[3, text.Length];
			this.NextCodeset = new Codeset[3, text.Length];

			InitCosts(Codeset.A);
			InitCosts(Codeset.B);
			InitCosts(Codeset.C);

			void InitCosts(Codeset codeset)
			{
				for (var i = text.Length - 1; i >= 0; i--)
				{
					this.Costs[(int)codeset, i] = int.MaxValue;
				}
			}
		}

		private void ComputeCosts()
		{
			var prevWasDigit = false;
			var text = this.Text;
			for (var i = text.Length - 1; i >= 0; i--)
			{
				var c = text[i];

				if (c == Surrogates.Fnc1)
				{
					this.UseSameCodeset(i, 1, Codeset.A);
					this.UseSameCodeset(i, 1, Codeset.B);
					this.UseSameCodeset(i, 1, Codeset.C);
					prevWasDigit = false;
					continue;
				}

				var isCodesetA = IsCodesetA(c);
				var isCodesetB = IsCodesetB(c);
				var isDigit = IsDigit(c);

				if (isCodesetA)
				{
					TryCodesetAb(Codeset.A, Codeset.B, !isCodesetB);
				}

				if (isCodesetB)
				{
					TryCodesetAb(Codeset.B, Codeset.A, !isCodesetA);
				}

				if (isDigit && prevWasDigit)
				{
					this.UseSameCodeset(i, 2, Codeset.C);
					this.ChangeCodeset(i, 2, Codeset.A, Codeset.C);
					this.ChangeCodeset(i, 2, Codeset.B, Codeset.C);
				}

				prevWasDigit = isDigit;

				void TryCodesetAb(Codeset main, Codeset other, bool isExclusive)
				{
					this.UseSameCodeset(i, 1, main);
					this.ChangeCodeset(i, 1, other, main);
					this.ChangeCodeset(i, 1, Codeset.C, main);

					if (isExclusive)
					{
						// Try using a shift operation.
						this.UseSameCodeset(i, 1, other, 2);
					}
				}
			}
		}

		private void UseSameCodeset(int index, int length, Codeset codeset, int cost = 1)
		{
			this.Advance(index, length, codeset, codeset, cost);
		}

		private void ChangeCodeset(int index, int length, Codeset fromCodeset, Codeset toCodeset)
		{
			this.Advance(index, length, fromCodeset, toCodeset, 2);
		}

		private void Advance(int index, int length, Codeset fromCodeset, Codeset toCodeset, int cost)
		{
			var fromIndex = index + length;
			var fromCost = this.Costs[(int)fromCodeset, fromIndex];
			if (fromCost < int.MaxValue)
			{
				var toIndex = index;
				var toCost = fromCost + cost;
				var currentBestCost = this.Costs[(int)toCodeset, toIndex];
				if (toCost < currentBestCost)
				{
					this.Costs[(int)toCodeset, toIndex] = toCost;
					this.NextIndex[(int)toCodeset, toIndex] = fromIndex;
					this.NextCodeset[(int)toCodeset, toIndex] = fromCodeset;
				}
			}
		}

		private Codeset[] GeneratePath()
		{
			var length = this.Text.Length;
			var codesets = new Codeset[length];
			var currentIndex = 0;
			var currentCodeset = ChooseStartCodeset();

			while (currentIndex < length)
			{
				codesets[currentIndex] = currentCodeset;

				var nextIndex = this.NextIndex[(int)currentCodeset, currentIndex];
				currentCodeset = this.NextCodeset[(int)currentCodeset, currentIndex];
				currentIndex = nextIndex;
			}

			return codesets;

			Codeset ChooseStartCodeset()
			{
				var costA = this.Costs[(int)Codeset.A, 0];
				var costB = this.Costs[(int)Codeset.B, 0];
				var costC = this.Costs[(int)Codeset.C, 0];

				if (costC < costA && costC < costB)
				{
					return Codeset.C;
				}

				return costA < costB ? Codeset.A : Codeset.B;
			}
		}

		private static bool IsDigit(char c) => '0' <= c && c <= '9';

		private static bool IsCodesetA(char c) => c <= 95;

		private static bool IsCodesetB(char c) => c >= 32 && c <= 127;
	}
}