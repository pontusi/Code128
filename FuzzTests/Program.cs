using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Code128;
using ZXing;

namespace FuzzTests
{
	public class Program
	{
		public static void Main()
		{
			var encoder = new BarcodeEncoder();
			var reader = new BarcodeReader { Options = { PossibleFormats = new[] { BarcodeFormat.CODE_128 } } };

			var count = 0;
			foreach (var text in CreateTexts(25, 4000))
			{
				if (!encoder.TryEncode(text, out var codes))
				{
					throw new Exception("Failed to encode " + text);
				}

				var barcode = new BarcodePainter(codes);
				using (var bitmap = barcode.Draw(3, 42))
				{
					var decoded = reader.Decode(bitmap);
					if (decoded.Text != text)
					{
						throw new Exception("Encoded " + text + " but read " + decoded.Text);
					}
				}

				Console.Write($"\rTested {++count} codes");
			}

			Console.WriteLine();
		}

		private static IEnumerable<string> CreateTexts(int maxLength, int iterations)
		{
			var rng = new RNGCryptoServiceProvider();

			for (var length = 1; length <= maxLength; length++)
			{
				var buffer = new byte[length];
				for (var i = iterations; i > 0; i--)
				{
					rng.GetBytes(buffer);
					for (var j = 0; j < length; j++)
					{
						buffer[j] &= 127;
					}

					yield return Encoding.ASCII.GetString(buffer);
				}
			}
		}
	}
}