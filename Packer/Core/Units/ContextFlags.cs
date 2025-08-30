
namespace Packer.Core
{
	[Flags]
	internal enum ContextFlags
	{
		None = 0,
		HasBOM = 1 << 0,
		UTF16LELike = 1 << 1,
		UTF16BELike = 1 << 2,
		LikelyBinary = 1 << 3
	}
}
