
namespace Packer.Core
{
	internal sealed class TextDetectionOptions
	{
		public int SampleSize { get; init; } = 8192;
		public double MinConfidence { get; init; } = 0.50; // порог для IsText
		public IReadOnlySet<string> TextExtensions { get; init; } = DefaultTextExtensions();
		public IReadOnlySet<string> BinaryExtensions { get; init; } = DefaultBinaryExtensions();

		private static HashSet<string> DefaultTextExtensions() => new(StringComparer.OrdinalIgnoreCase)
		{
			".txt",".log",".csv",".tsv",".json",".xml",".yaml",".yml",".ini",".cfg",".properties",
			".md",".rst",".html",".htm",".css",".js",".ts",".tsx",".jsx",
			".cs",".cpp",".cxx",".cc",".c",".h",".hpp",".csproj",".sln",
			".sql",".bat",".cmd",".ps1",".sh",".dockerfile",".toml",".gradle",".kt"
		};

		private static HashSet<string> DefaultBinaryExtensions() => new(StringComparer.OrdinalIgnoreCase)
		{
			".exe",".dll",".so",".dylib",".png",".jpg",".jpeg",".gif",".bmp",".ico",
			".pdf",".zip",".rar",".7z",".xz",".gz",".tar",".iso",
			".mp3",".wav",".flac",".mp4",".mkv",".avi",".mov",
			".doc",".docx",".xls",".xlsx",".ppt",".pptx"
		};
	}
}
