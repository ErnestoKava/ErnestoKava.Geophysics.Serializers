using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using ErnestoKava.Geophysics.Serializers.SurferGrid;

namespace ErnestoKava.Geophysics.Serializers.Grid3d
{
	public class GRD3DWriter
	{
		public static bool Save(Types.Grids.Grid3d grd, string filename)
		{
			IFileSystem fileSystem;

			string directory = Path.GetDirectoryName(filename);
			string name = Path.GetFileNameWithoutExtension(filename);

			var extension = Path.GetExtension(filename);
			if (extension == ".zip")
				fileSystem = new ZipFileSystem(filename);
			else
				fileSystem = new RealFileSystem(directory);

			layers xml = new layers();
			xml.layer = new layersLayer[grd.nLayer];
			xml.zmin = grd.zTop.ToString();
			xml.zmax = (grd.zTop + grd.zSize * (grd.nLayer - 1)).ToString();

			for (int i = 0, l = grd.nLayer-1; i < grd.nLayer; i++, l--)
			{
				string layerFile = String.Format("{1}_{0:000}.grd", l, name);
				var layerStream = fileSystem.Create(layerFile);
				GRDWriter.Save(grd.ExtractPlane(i), layerStream);
				
				// layerStream.Flush();
				layerStream.Close();
				xml.layer[i] = new layersLayer() { grid = layerFile, z = i.ToString() };
			}

			var stream = fileSystem.Create(Path.GetFileNameWithoutExtension(name) + ".xml");
			XmlSerializer serializer = new XmlSerializer(typeof(layers));
			serializer.Serialize(stream, xml);
			stream.Close();

			return true;
		}
	}
}
