using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Code128
{
	/// <summary>
	/// Converts an encoded barcode into a bitmap.
	/// </summary>
	public sealed class BarcodePainter
	{
		private const byte End = 106;

		private static readonly short[] Barcodes = {
			0b11011001100,
			0b11001101100,
			0b11001100110,
			0b10010011000,
			0b10010001100,
			0b10001001100,
			0b10011001000,
			0b10011000100,
			0b10001100100,
			0b11001001000,
			0b11001000100,
			0b11000100100,
			0b10110011100,
			0b10011011100,
			0b10011001110,
			0b10111001100,
			0b10011101100,
			0b10011100110,
			0b11001110010,
			0b11001011100,
			0b11001001110,
			0b11011100100,
			0b11001110100,
			0b11101101110,
			0b11101001100,
			0b11100101100,
			0b11100100110,
			0b11101100100,
			0b11100110100,
			0b11100110010,
			0b11011011000,
			0b11011000110,
			0b11000110110,
			0b10100011000,
			0b10001011000,
			0b10001000110,
			0b10110001000,
			0b10001101000,
			0b10001100010,
			0b11010001000,
			0b11000101000,
			0b11000100010,
			0b10110111000,
			0b10110001110,
			0b10001101110,
			0b10111011000,
			0b10111000110,
			0b10001110110,
			0b11101110110,
			0b11010001110,
			0b11000101110,
			0b11011101000,
			0b11011100010,
			0b11011101110,
			0b11101011000,
			0b11101000110,
			0b11100010110,
			0b11101101000,
			0b11101100010,
			0b11100011010,
			0b11101111010,
			0b11001000010,
			0b11110001010,
			0b10100110000,
			0b10100001100,
			0b10010110000,
			0b10010000110,
			0b10000101100,
			0b10000100110,
			0b10110010000,
			0b10110000100,
			0b10011010000,
			0b10011000010,
			0b10000110100,
			0b10000110010,
			0b11000010010,
			0b11001010000,
			0b11110111010,
			0b11000010100,
			0b10001111010,
			0b10100111100,
			0b10010111100,
			0b10010011110,
			0b10111100100,
			0b10011110100,
			0b10011110010,
			0b11110100100,
			0b11110010100,
			0b11110010010,
			0b11011011110,
			0b11011110110,
			0b11110110110,
			0b10101111000,
			0b10100011110,
			0b10001011110,
			0b10111101000,
			0b10111100010,
			0b11110101000,
			0b11110100010,
			0b10111011110,
			0b10111101110,
			0b11101011110,
			0b11110101110,
			0b11010000100,
			0b11010010000,
			0b11010011100,
			0b1100011101011
		};

		public BarcodePainter(byte[] codes)
		{
			this.Bars = CodesToBars(codes);
		}

		public bool[] Bars { get; }

		public static Bitmap Draw(byte[] codes, int pixelsPerModule, int heightInPixels)
		{
			var painter = new BarcodePainter(codes);
			return painter.Draw(pixelsPerModule, heightInPixels);
		}

		public Bitmap Draw(int pixelsPerModule, int heightInPixels)
		{
			const int QuietZoneWidth = 10;

			var moduleCount = this.Bars.Length + QuietZoneWidth * 2;
			using (var bitmap = new Bitmap(pixelsPerModule * moduleCount, heightInPixels))
			using (var graphics = Graphics.FromImage(bitmap))
			using (var pen = new Pen(Brushes.Black, pixelsPerModule))
			{
				graphics.Clear(Color.White);
				var x = QuietZoneWidth * pixelsPerModule;
				foreach (var bar in this.Bars)
				{
					if (bar)
					{
						graphics.DrawLine(pen, x, 0, x, heightInPixels);
					}

					x += pixelsPerModule;
				}

				graphics.Save();

				return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format1bppIndexed);
			}
		}

		private static bool[] CodesToBars(byte[] codes)
		{
			if (codes[codes.Length - 1] != End)
			{
				throw new ArgumentException("Codes are not terminated with end token.", nameof(codes));
			}

			var bars = new bool[codes.Length * 11 + 2];
			var barIndex = 0;
			for (var codeIndex = 0; codeIndex < codes.Length - 1; codeIndex++)
			{
				AddBars(codes[codeIndex], 11);
			}

			AddBars(End, 13);
			return bars;

			void AddBars(byte value, short count)
			{
				var pattern = Barcodes[value];
				for (var i = count - 1; i >= 0; i--)
				{
					bars[barIndex++] = (pattern & (1 << i)) != 0;
				}
			}
		}
	}
}