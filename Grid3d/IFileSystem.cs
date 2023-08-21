using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.SharpZipLib.Zip;

namespace ErnestoKava.Geophysics.Serializers.Grid3d
{
	public interface IFileSystem
	{
		Stream Open(string filename);
		Stream Create(string filename);
	}

	public class RealFileSystem : IFileSystem
	{
		private string directory;
		public RealFileSystem(string directory)
		{
			this.directory = directory;
		}

		public Stream Open(string filename)
		{
			return File.Open(Path.Combine(directory, filename), FileMode.Open);
		}

		public Stream Create(string filename)
		{
			return File.Create(Path.Combine(directory, filename));
		}
	}

	public class ZipFileSystem : IFileSystem, IDisposable
	{
		public class ToZipMemoryStream : MemoryStream
		{
			ZipFileSystem _zipFileSystem;
			public ToZipMemoryStream(ZipFileSystem zipFileSystem)
			{
				_zipFileSystem = zipFileSystem;
			}
			public override void Close()
			{
				base.Close();

				lock (_zipFileSystem.zippedFiles)
				{
					var d = _zipFileSystem.zippedFiles.FirstOrDefault(z => z.Stream == this);
					var file = _zipFileSystem.file;

					file.BeginUpdate();
					file.Add(new FakeStaticDataSource(d), d.FileName);
					file.CommitUpdate();
				}
			}
		}

		public class FakeStaticDataSource : IStaticDataSource
		{
			ZippedData _data;
			public FakeStaticDataSource(ZippedData data)
			{
				_data = data;
			}

			public Stream GetSource()
			{
				return new MemoryStream(_data.Stream.ToArray());
			}
		}

		public class ZippedData
		{
			public ZippedData(ZipFileSystem fileSystem)
			{
				Stream = new ToZipMemoryStream(fileSystem);
			}

			public MemoryStream Stream { get; set; }
			public string FileName { get; set; }
		}

		ZipFile file = null;

		List<ZippedData> zippedFiles = new List<ZippedData>();

		public ZipFileSystem(string zipFileName)
		{
			if (File.Exists(zipFileName))
				file = new ZipFile(File.OpenRead(zipFileName));
			else
			{
				file = ZipFile.Create(File.Create(zipFileName));
			}
		}

		public Stream Open(string filename)
		{
			int entry = file.FindEntry(filename, true);
			if (entry >= 0)
				return file.GetInputStream(entry);
			else
				return null;
		}

		public void Dispose()
		{
			if (file != null)
			{
				file.IsStreamOwner = true; // Makes close also shut the underlying stream
				file.Close(); // Ensure we release resources
			}
		}

		public Stream Create(string filename)
		{
			lock (zippedFiles)
			{
				if (zippedFiles.Any(z => z.FileName == filename))
					throw new FileNotFoundException();

				var data = new ZippedData(this) { FileName = filename };
				zippedFiles.Add(data);

				return data.Stream;
			}
		}
	}
}
