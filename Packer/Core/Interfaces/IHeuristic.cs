using Packer.Core.Models;

namespace Packer.Core.Interfaces
{
	internal interface IHeuristic
	{
		HeuristicResult? Apply(HeuristicContext ctx);
	}
}
