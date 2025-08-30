using Packer.Core;
using Packer.Core.Interfaces;
using Packer.Core.Models;

namespace Packer.Heuristics
{
	internal sealed class ControlCharHeuristic : IHeuristic
	{
		public HeuristicResult? Apply(HeuristicContext ctx)
		{
			int ctrl = 0;
			for (int i = 0; i < ctx.Read; i++)
			{
				byte b = ctx.Buffer[i];
				bool isAllowedWhitespace = b is 0x09 or 0x0A or 0x0D; // \t \n \r
				if (b < 0x20 && !isAllowedWhitespace) ctrl++;
			}

			if (ctrl == 0) return new HeuristicResult(+0.10, "Управляющих символов нет (кроме CR/LF/TAB).");

			double p = (double)ctrl / ctx.Read;
			if (p > 0.10) return new HeuristicResult(-0.40, $"Много управляющих символов ({p:P0}).");
			return new HeuristicResult(-0.15, $"Обнаружены управляющие символы ({p:P0}).");
		}
	}
}
