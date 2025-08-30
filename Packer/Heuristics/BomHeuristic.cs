using Packer.Core;
using Packer.Core.Interfaces;
using Packer.Core.Models;
using System.Text;

namespace Packer.Heuristics
{
	internal sealed class BomHeuristic : IHeuristic
	{
		public HeuristicResult? Apply(HeuristicContext ctx)
		{
			var b = ctx.Buffer; int n = ctx.Read;
			if (n >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF)
			{
				ctx.Flags |= ContextFlags.HasBOM;
				return new HeuristicResult(+0.40, "Найден BOM UTF-8.", new UTF8Encoding(false));
			}
			if (n >= 2 && b[0] == 0xFF && b[1] == 0xFE)
			{
				ctx.Flags |= ContextFlags.HasBOM | ContextFlags.UTF16LELike;
				return new HeuristicResult(+0.50, "Найден BOM UTF-16 LE.", Encoding.Unicode);
			}
			if (n >= 2 && b[0] == 0xFE && b[1] == 0xFF)
			{
				ctx.Flags |= ContextFlags.HasBOM | ContextFlags.UTF16BELike;
				return new HeuristicResult(+0.50, "Найден BOM UTF-16 BE.", Encoding.BigEndianUnicode);
			}
			return null;
		}
	}
}
