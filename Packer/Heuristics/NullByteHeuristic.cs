using Packer.Core;
using Packer.Core.Interfaces;
using Packer.Core.Models;

namespace Packer.Heuristics
{
	internal sealed class NullByteHeuristic : IHeuristic
	{
		public HeuristicResult? Apply(HeuristicContext ctx)
		{
			// Если это похоже на UTF-16 — нули допустимы
			if ((ctx.Flags & (ContextFlags.UTF16LELike | ContextFlags.UTF16BELike)) != 0) return null;

			int zeros = 0;
			for (int i = 0; i < ctx.Read; i++) if (ctx.Buffer[i] == 0) zeros++;

			if (zeros == 0) return new HeuristicResult(+0.05, "Нулевых байтов нет.");
			double p = (double)zeros / ctx.Read;
			if (p >= 0.02) // много нулей для «обычного» текста
			{
				ctx.Flags |= ContextFlags.LikelyBinary;
				return new HeuristicResult(-0.50, $"Найдено много нулевых байтов ({p:P0}).");
			}
			return new HeuristicResult(-0.10, $"Найдены нулевые байты ({p:P0}).");
		}
	}
}
