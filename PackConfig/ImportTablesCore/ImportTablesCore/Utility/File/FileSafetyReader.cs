using System;
using System.IO;

namespace ImportTables.Utils
{
	public class FileSafetyReader : IDisposable
	{
		public FileStream Fs { get; private set; }
		private string finalPath;
		public FileSafetyReader(string path)
		{
			try
			{
				//Fs = File.Open(path, FileMode.Open);

				Fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			}
			catch (Exception)
			{
				//策划会喜欢开表导表，防止IO异常
				var fn = Path.GetFileName(path);
				var cPath = path.Replace(fn, fn + "sadasdwekqwhnjdkhnasjkdbhas1241254326");
				File.Copy(path, cPath, true);
				Fs = File.Open(cPath, FileMode.Open);
				finalPath = cPath;
			}
		}
		public void Dispose()
		{
			Fs?.Dispose();
			if (!string.IsNullOrEmpty(finalPath))
			{
				File.Delete(finalPath);
			}
			Fs = null;
		}
	}
}