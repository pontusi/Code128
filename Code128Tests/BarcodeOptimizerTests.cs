using Code128;
using Xunit;

namespace Code128Tests
{
	public class BarcodeOptimizerTests
	{
		public class BasicTests
		{
			[Fact]
			public void UsesCodesetBForLowercase()
			{
				var expected = new[] { Codeset.B, Codeset.B, Codeset.B, Codeset.B };
				var actual = Optimize("test");
				Assert.Equal(expected, actual);
			}

			[Fact]
			public void UsesCodesetAForSpecialCharacters()
			{
				var expected = new[] { Codeset.A, Codeset.A, Codeset.A, Codeset.A };
				var actual = Optimize("\r\n\r\n");
				Assert.Equal(expected, actual);
			}

			[Fact]
			public void FavorsCodesetBOverAWhenLengthEqual()
			{
				var expected = new[] { Codeset.B, Codeset.B, Codeset.B, Codeset.B };
				var actual = Optimize("TEST");
				Assert.Equal(expected, actual);
			}

			[Fact]
			public void UsesCodesetCFor2Digits()
			{
				var expected = new[] { Codeset.C, Codeset.C };
				var actual = Optimize("12");
				Assert.Equal(expected, actual);
			}

			[Fact]
			public void UsesCodesetCFor4Digits()
			{
				var expected = new[] { Codeset.C, Codeset.C, Codeset.C, Codeset.C };
				var actual = Optimize("1234");
				Assert.Equal(expected, actual);
			}

			[Fact]
			public void UsesCodesetCFor6Digits()
			{
				var expected = new[] { Codeset.C, Codeset.C, Codeset.C, Codeset.C, Codeset.C, Codeset.C };
				var actual = Optimize("123456");
				Assert.Equal(expected, actual);
			}
		}

		public class ChaingingCodeset
		{
			[Fact]
			public void ChangesCodeset()
			{
				var expected = new[] { Codeset.B, Codeset.B, Codeset.B, Codeset.A, Codeset.A, Codeset.A };
				var actual = Optimize("abc\r\nA");
				Assert.Equal(expected, actual);
			}
		}

		public class Shifting
		{
			[Theory]
			[InlineData("t\nest")]
			[InlineData("te\nst")]
			[InlineData("tes\nt")]
			public void UsesBShiftToAWhenOptimal(string text)
			{
				var expected = new[] { Codeset.B, Codeset.B, Codeset.B, Codeset.B, Codeset.B };
				var actual = Optimize(text);
				Assert.Equal(expected, actual);
			}

			[Theory]
			[InlineData("\nx\n\n\n")]
			[InlineData("\n\nx\n\n")]
			[InlineData("\n\n\nx\n")]
			public void UsesAShiftToBWhenOptimal(string text)
			{
				var expected = new[] { Codeset.A, Codeset.A, Codeset.A, Codeset.A, Codeset.A };
				var actual = Optimize(text);
				Assert.Equal(expected, actual);
			}
		}

		public class DoubleDigits
		{
			[Fact]
			public void UsesCodesetBFor3Digits()
			{
				var expected = new[] { Codeset.B, Codeset.B, Codeset.B };
				var actual = Optimize("123");
				Assert.Equal(expected, actual);
			}

			[Fact]
			public void UsesCodesetCWhenStartsWith4Digits()
			{
				var expected = new[] { Codeset.C, Codeset.C, Codeset.C, Codeset.C, Codeset.B };
				var actual = Optimize("1234x");
				Assert.Equal(expected, actual);
			}

			[Fact]
			public void UsesCodesetCWhenEndsWith4Digits()
			{
				var expected = new[] { Codeset.B, Codeset.C, Codeset.C, Codeset.C, Codeset.C };
				var actual = Optimize("x1234");
				Assert.Equal(expected, actual);
			}

			[Fact]
			public void DoesNotSwitchToCodesetCUnlessLengthIsReduced()
			{
				var expected = new[] { Codeset.B, Codeset.B, Codeset.B, Codeset.B, Codeset.B, Codeset.B };
				var actual = Optimize("x1234x");
				Assert.Equal(expected, actual);
			}

			[Fact]
			public void UsesCodesetCFor6Of7Digits()
			{
				var expected = new[] { Codeset.B, Codeset.B, Codeset.C, Codeset.C, Codeset.C, Codeset.C, Codeset.C, Codeset.C };
				var actual = Optimize("x1234567");
				Assert.Equal(expected, actual);
			}

			[Theory]
			[InlineData("¤1234")]
			[InlineData("12¤34")]
			[InlineData("1234¤")]
			public void UsesFnc1(string text)
			{
				var expected = new[] { Codeset.C, Codeset.C, Codeset.C, Codeset.C, Codeset.C };
				var actual = Optimize(text);
				Assert.Equal(expected, actual);
			}
		}

		private static Codeset[] Optimize(string text)
		{
			return BarcodeOptimizer.ChooseCodesets(text);
		}
	}
}