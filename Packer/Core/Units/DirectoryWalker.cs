using Packer.Core.Interfaces;

namespace Packer.Core
{
	internal abstract class DirectoryWalker : IDirectoryWalker
	{
		protected DirectoryInfo _directoryInfo;
		private string _ignorePath = ".packingore";
		protected HashSet<string> _ignore = new HashSet<string>();

		public DirectoryWalker(string path = "")
		{
			if (string.IsNullOrWhiteSpace(path))
				_directoryInfo = new DirectoryInfo(Environment.CurrentDirectory);
			else
				_directoryInfo = new DirectoryInfo(path);

			GetIgnore();
		}

		private void GetIgnore()
		{
			if (!File.Exists(_ignorePath))
				File.WriteAllText(_ignorePath, ".packingore\npack.txt");
			else
				_ignore = File.ReadAllLines(_ignorePath).ToHashSet<string>();
		}

		protected bool IsIgnored(FileSystemInfo fsi)
		{
			string fullPath = fsi.FullName;
			string name = fsi.Name;
			string ext = fsi is FileInfo fi ? fi.Extension : string.Empty;

			if (name == "pack.txt" || name == ".packingore" || ext == ".packingore")
				return true;

			// Проверяем по полному пути
			if (_ignore.Contains(fullPath, StringComparer.OrdinalIgnoreCase))
				return true;

			// Проверяем по имени файла/папки
			if (_ignore.Contains(name, StringComparer.OrdinalIgnoreCase))
				return true;

			// Проверяем по расширению
			if (!string.IsNullOrEmpty(ext) && _ignore.Contains(ext, StringComparer.OrdinalIgnoreCase))
				return true;

			return false;
		}

		public abstract void WalkOnDirectory();
	}
}
