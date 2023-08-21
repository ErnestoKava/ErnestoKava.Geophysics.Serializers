using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using ErnestoKava.Geophysics.Serializers.SurferGrid;
using ErnestoKava.Geophysics.Types.Grids;

namespace ErnestoKava.Geophysics.Serializers.Grid3d
{
	public class GRD3DReader
	{
		public static Types.Grids.Grid3d Read(string filename)
		{
			string extension = Path.GetExtension(filename).ToLowerInvariant();

			IFileSystem fileSystem = null;

			if (extension == ".zip")
			{
				fileSystem = new ZipFileSystem(filename);
				filename = Path.GetFileNameWithoutExtension(filename) + ".xml";
			}
			else
			{
				fileSystem = new RealFileSystem(Path.GetDirectoryName(filename));
			}

			layers data;
			using (var stream = fileSystem.Open(filename))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(layers));
				data = (layers)serializer.Deserialize(stream);
			}

			if (data.layer.Count() == 0)
				throw new InvalidDataException("No layer data found");

			// Take values from layer.z for compatibility with old cubes without zMin&zMax data
			// TODO: check if layer.z exists because it is not needed anymore and can be removed from Grig3d generation
			double zMin = data.layer.Select(l => double.Parse(l.z)).Min();
			double zMax = data.layer.Select(l => double.Parse(l.z)).Max();
			if (!String.IsNullOrEmpty(data.zmin) && !String.IsNullOrEmpty(data.zmax))
			{
				zMin = double.Parse(data.zmin);
				zMax = double.Parse(data.zmax);
			}

			double zStep = (zMax - zMin) / (data.layer.Count() - 1);

			Grid[] grids = new Grid[data.layer.Count()];

			for (int i = 0; i < data.layer.Count(); i++)
				grids[i] = GRDReader.Read(fileSystem.Open(data.layer[i].grid));

			return new Types.Grids.Grid3d(grids, zMin, zStep);
		}
	}
}
