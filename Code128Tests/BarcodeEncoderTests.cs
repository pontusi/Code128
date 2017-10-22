using Code128;
using Xunit;

namespace Code128Tests
{
	public class BarcodeEncoderTests
	{
		private static BarcodeEncoder Encoder { get; } = new BarcodeEncoder();

		[Fact]
		public void RejectsUnicode()
		{
			Assert.False(Encoder.TryEncode("日本語", out _));
		}

		[Fact]
		public void Rejects8BitCharacters()
		{
			Assert.False(Encoder.TryEncode("åäö", out _));
		}

		[Fact]
		public void EncodesLowercase()
		{
			var expected = new byte[] { 104, 84, 69, 83, 84, 87, 106 };
			var actual = Encode("test");
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void EncodesCompactDigits()
		{
			var expected = new byte[] { 105, 12, 34, 82, 106 };
			var actual = Encode("1234");
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void EncodesSpecialCharacters()
		{
			var expected = new byte[] { 103, 33, 73, 34, 75, 106 };
			var actual = Encode("A\tB");
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void EncodesShiftedCharacters()
		{
			var expected = new byte[] { 104, 65, 98, 73, 66, 24, 106 };
			var actual = Encode("a\tb");
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void EncodesCharsetChanges()
		{
			var expected = new byte[] { 104, 65, 66, 101, 73, 73, 99, 12, 34, 100, 65, 66, 58, 106 };
			var actual = Encode("ab\t\t1234ab");
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void EncodesFncInCharsetA()
		{
			var expected = new byte[] { 103, 102, 33, 73, 34, 8, 106 };
			var actual = Encode("¤A\tB");
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void EncodesFncInCharsetB()
		{
			var expected = new byte[] { 104, 102, 65, 66, 19, 106 };
			var actual = Encode("¤ab");
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void EncodesFncInCharsetC()
		{
			var expected = new byte[] { 105, 102, 12, 34, 24, 106 };
			var actual = Encode("¤1234");
			Assert.Equal(expected, actual);
		}

		private static byte[] Encode(string text)
		{
			Assert.True(Encoder.TryEncode(text, out var codes));
			return codes;
		}
	}
}