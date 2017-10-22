using System.Collections.Generic;
using System.Linq;

namespace Code128
{
	/// <summary>
	/// Converts text into barcode codes that can then be converted into a barcode by the <see cref="BarcodePainter"/>.
	/// </summary>
	public sealed class BarcodeEncoder
	{
		private const byte Shift = 98;
		private const byte CodeC = 99;
		private const byte CodeB = 100;
		private const byte CodeA = 101;
		private const byte Fnc1 = 102;
		private const byte StartA = 103;
		private const byte StartB = 104;
		private const byte StartC = 105;
		private const byte End = 106;

		public BarcodeEncoder()
		{
			this.Encoders = new CodesetEncoder[3];
			this.Encoders[(int)Codeset.A] = new CodesetAEncoder();
			this.Encoders[(int)Codeset.B] = new CodesetBEncoder();
			this.Encoders[(int)Codeset.C] = new CodesetCEncoder();
		}

		private CodesetEncoder[] Encoders { get; }

		public static bool TryEncodeBarcode(string text, out byte[] codes)
		{
			var encoder = new BarcodeEncoder();
			return encoder.TryEncode(text, out codes);
		}

		public bool TryEncode(string text, out byte[] codes)
		{
			if (!text.All(IsSupportedCharacter))
			{
				codes = null;
				return false;
			}

			codes = this.Encode(text);
			return true;

			bool IsSupportedCharacter(char c) => c <= 127 || c == Surrogates.Fnc1;
		}

		private byte[] Encode(string text)
		{
			var buffer = new List<byte>();
			var codesets = BarcodeOptimizer.ChooseCodesets(text);
			var currentCodeset = codesets[0];
			var currentEncoder = this.Encoders[(int)currentCodeset];
			var currentIndex = 0;

			buffer.Add(currentEncoder.StartToken);
			while (true)
			{
				currentEncoder.Encode(text, ref currentIndex, buffer);
				if (currentIndex == text.Length)
				{
					break;
				}

				var nextCodeset = codesets[currentIndex];
				if (nextCodeset != currentCodeset)
				{
					currentCodeset = nextCodeset;
					currentEncoder = this.Encoders[(int)currentCodeset];
					buffer.Add(currentEncoder.CodesetToken);
				}
			}

			int checksum = buffer[0];
			for (var i = 1; i < buffer.Count; i++)
			{
				checksum += i * buffer[i];
			}

			buffer.Add((byte)(checksum % 103));
			buffer.Add(End);

			return buffer.ToArray();
		}

		private abstract class CodesetEncoder
		{
			public abstract byte StartToken { get; }

			public abstract byte CodesetToken { get; }

			public void Encode(string text, ref int index, ICollection<byte> output)
			{
				var c = text[index];
				if (c == Surrogates.Fnc1)
				{
					output.Add(Fnc1);
					index += 1;
					return;
				}

				this.EncodeUnique(text, ref index, output);
			}

			protected static bool TryEncodeA(string text, ref int index, ICollection<byte> output)
			{
				var c = text[index];
				if (c > 95)
				{
					return false;
				}

				output.Add((byte)(c >= 32 ? c - 32 : c + 64));
				index += 1;
				return true;
			}

			protected static bool TryEncodeB(string text, ref int index, ICollection<byte> output)
			{
				var c = text[index];
				if (c < 32)
				{
					return false;
				}

				output.Add((byte)(c - 32));
				index += 1;
				return true;
			}

			protected abstract void EncodeUnique(string text, ref int index, ICollection<byte> output);
		}

		private sealed class CodesetAEncoder : CodesetEncoder
		{
			public override byte StartToken => StartA;

			public override byte CodesetToken => CodeA;

			protected override void EncodeUnique(string text, ref int index, ICollection<byte> output)
			{
				if (!TryEncodeA(text, ref index, output))
				{
					output.Add(Shift);
					TryEncodeB(text, ref index, output);
				}
			}
		}

		private sealed class CodesetBEncoder : CodesetEncoder
		{
			public override byte StartToken => StartB;

			public override byte CodesetToken => CodeB;

			protected override void EncodeUnique(string text, ref int index, ICollection<byte> output)
			{
				if (!TryEncodeB(text, ref index, output))
				{
					output.Add(Shift);
					TryEncodeA(text, ref index, output);
				}
			}
		}

		private sealed class CodesetCEncoder : CodesetEncoder
		{
			public override byte StartToken => StartC;

			public override byte CodesetToken => CodeC;

			protected override void EncodeUnique(string text, ref int index, ICollection<byte> output)
			{
				var value = (text[index++] - '0') * 10 + (text[index++] - '0');
				output.Add((byte)value);
			}
		}
	}
}