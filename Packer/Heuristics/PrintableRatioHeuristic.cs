using Packer.Core;
using Packer.Core.Interfaces;
using Packer.Core.Models;

namespace Packer.Heuristics
{
	internal sealed class PrintableRatioHeuristic : IHeuristic
	{
		public HeuristicResult? Apply(HeuristicContext ctx)
		{
			int printable = 0, bytes = ctx.Read;

			for (int i = 0; i < bytes; i++)
			{
				byte b = ctx.Buffer[i];
				if (b == 0x09 || b == 0x0A || b == 0x0D) { printable++; continue; } // \t \n \r
				if (b >= 0x20 && b <= 0x7E) { printable++; continue; }              // видимые ASCII
																					// байты >= 0x80 оставим нейтральными — это может быть UTF-8/однобайтные локали
			}

			double ratio = (double)printable / Math.Max(1, bytes);
			if (ratio >= 0.85) return new HeuristicResult(+0.20, $"Высокая доля печатаемых ASCII ({ratio:P0}).");
			if (ratio < 0.50) return new HeuristicResult(-0.20, $"Низкая доля печатаемых ASCII ({ratio:P0}).");
			return null;
		}
	}
}
