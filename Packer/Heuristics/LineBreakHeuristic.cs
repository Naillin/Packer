using Packer.Core;
using Packer.Core.Interfaces;
using Packer.Core.Models;

namespace Packer.Heuristics
{
	internal sealed class LineBreakHeuristic : IHeuristic
	{
		public HeuristicResult? Apply(HeuristicContext ctx)
		{
			int lf = 0;
			for (int i = 0; i < ctx.Read; i++) if (ctx.Buffer[i] == (byte)'\n') lf++;
			if (lf == 0) return null;
			return new HeuristicResult(+0.05, "Обнаружены переводы строк.");
		}
	}
}
