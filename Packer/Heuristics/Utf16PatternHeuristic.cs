using Packer.Core;
using Packer.Core.Interfaces;
using Packer.Core.Models;

namespace Packer.Heuristics
{
	/// <summary>
	/// Обнаружение UTF-16 без BOM: у ASCII-текста в UTF-16LE нули стоят в байтах с нечётным индексом, в BE — с чётным.
	/// </summary>
	internal sealed class Utf16PatternHeuristic : IHeuristic
	{
		public HeuristicResult? Apply(HeuristicContext ctx)
		{
			var b = ctx.Buffer; int n = ctx.Read;
			if (n < 4 || (ctx.Flags & (ContextFlags.UTF16LELike | ContextFlags.UTF16BELike)) != 0) return null;

			int zerosEven = 0, zerosOdd = 0, consider = Math.Min(n, 4096);
			for (int i = 0; i < consider; i++)
			{
				if (b[i] == 0) { if ((i & 1) == 0) zerosEven++; else zerosOdd++; }
			}
			// Требуем заметную долю нулей и асимметрию
			int totalZeros = zerosEven + zerosOdd;
			if (totalZeros < consider * 0.1) return null;

			if (zerosOdd > zerosEven * 3)
			{
				ctx.Flags |= ContextFlags.UTF16LELike;
				return new HeuristicResult(+0.30, "Похоже на UTF-16 LE без BOM.");
			}
			if (zerosEven > zerosOdd * 3)
			{
				ctx.Flags |= ContextFlags.UTF16BELike;
				return new HeuristicResult(+0.30, "Похоже на UTF-16 BE без BOM.");
			}
			return null;
		}
	}
}
