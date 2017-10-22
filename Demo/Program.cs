using System;
using System.Drawing.Imaging;
using Code128;

namespace Demo
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var text = args.Length > 0 ? args[0] : "test";

			if (BarcodeEncoder.TryEncodeBarcode(text, out var codes))
			{
				using (var bitmap = BarcodePainter.Draw(codes, 1, 20))
				{
					bitmap.Save("output.png", ImageFormat.Png);
				}
			}
			else
			{
				throw new Exception($"The text {text} contains unsupported characters.");
			}
		}
	}
}