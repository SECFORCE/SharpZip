using System;
using System.IO.Compression;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace SharpZip
{
	class Program
	{
		static void AddDirFiles(ZipArchive archive, string parentDir, string dir, int retries)
		{
			foreach (var file in System.IO.Directory.GetFiles(dir) )
			{
				try
				{
					var fullPath = Path.GetFullPath(file);

					// get relative path to have pretty zip names
					string relPath = Util.GetRelativePath(parentDir, fullPath);

					if(relPath.StartsWith("."))
					{
						relPath = relPath.Substring(2);
					}

					CreateZipEntry(archive, file, relPath, retries);
					
				}
				catch(Exception ex)
				{
					Console.WriteLine($"Failed zippping {file}: {ex.Message}");
				}
			}

			foreach (var subdir in System.IO.Directory.GetDirectories(dir))
			{
				try
				{
					AddDirFiles(archive, parentDir, subdir, retries);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed zippping dir {subdir}: {ex.Message}");
				}
			}
		}

		private static void CreateZipEntry(ZipArchive archive, string file, string path, int retries)
		{
			int count = 0;
			bool error = false;

			do
			{
				try
				{
					archive.CreateEntryFromFile(file, path);
					error = false;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error reading {path}. Retrying {retries-count} more times.");
					error = true;
					count += 1;
					System.Threading.Thread.Sleep(666);
				}
			} while (error && count < retries);
		}

		static void Main(string[] args)
		{
			try
			{
				if(args.Length!=2 && args.Length!=3)
				{
					Console.WriteLine("Sharp.exe <dir> <zip_file> [retryCount: default=5]");
					Console.WriteLine();
					Console.WriteLine("Example:");
					Console.WriteLine(@"Sharp.exe C:\temp\ c:\ahchive.zip 10");
					return;
				}

				// add trailling \\ to keep parent folder out of the zip
				string startPath = Path.GetFullPath(args[0])+"\\"; 
				string zipPath = args[1];
				int retries = 5;

				if (args.Length > 2)
					int.TryParse(args[2], out retries);

				using (var zipStream = new FileStream(zipPath, FileMode.Create))
				{
					using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create))
					{
						try
						{
							AddDirFiles(zipArchive, startPath, startPath, retries);
							Console.WriteLine($"Created zip {zipPath}.");
						}
						catch (Exception ex)
						{
							Console.WriteLine($"Failed zippping {startPath}: {ex.Message}");
						}
					}
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		}
	}

	public static class Util
	{
		[DllImport("shlwapi.dll", EntryPoint = "PathRelativePathTo")]
		static extern bool PathRelativePathTo(StringBuilder lpszDst,
			string from, UInt32 attrFrom,
			string to, UInt32 attrTo);

		public static string GetRelativePath(string from, string to)
		{
			StringBuilder builder = new StringBuilder(1024);
			bool result = PathRelativePathTo(builder, from, 0, to, 0);
			return builder.ToString();
		}
	}

}
