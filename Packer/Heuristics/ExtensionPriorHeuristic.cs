using Packer.Core;
using Packer.Core.Interfaces;
using Packer.Core.Models;

namespace Packer.Heuristics
{
	internal sealed class ExtensionPriorHeuristic : IHeuristic
	{
		public HeuristicResult? Apply(HeuristicContext ctx)
		{
			if (string.IsNullOrEmpty(ctx.Extension)) return null;

			if (ctx.Options.TextExtensions.Contains(ctx.Extension))
				return new HeuristicResult(+0.15, $"Расширение {ctx.Extension} типично текстовое.");
			if (ctx.Options.BinaryExtensions.Contains(ctx.Extension))
				return new HeuristicResult(-0.40, $"Расширение {ctx.Extension} типично бинарное.");
			return null;
		}
	}
}
