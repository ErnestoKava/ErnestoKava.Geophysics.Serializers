using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ErnestoKava.Geophysics.Types.Lines;
using ErnestoKava.Geophysics.Types.Vectors;

namespace ErnestoKava.Geophysics.Serializers.SurferBln
{
	public static class BLNReader
	{
		public static BlnElement[] Read(string fileName)
		{
			var result = new List<BlnElement>();

			using (StreamReader sr = new StreamReader(fileName))
			{
				while (!sr.EndOfStream)
				{
					var bln = new BlnElement();
					string header = sr.ReadLine();
					if (String.IsNullOrEmpty(header))
						continue;

					var parts = header.Split(new[] { ',' });
					int count;
					if (!int.TryParse(parts[0].Trim(), out count))
						throw new Exception(String.Format("Invalid BLN header: {0}", header));

					bln.Title = parts.Length > 1 ? parts[1] : null;

					for (int i = 0; i < count; i++)
					{
						if (sr.EndOfStream)
							throw new Exception("Unexpected end of file");

						if (String.IsNullOrEmpty(header))
							continue;

						string pointData = sr.ReadLine();
						var elements = pointData.Split(new char[] { ',', ' ' }).Where(p => p.Trim() != "").ToList();
						var values = elements.Take(2).Select(p => double.Parse(p.Trim())).ToArray();
						bln.Points.Add(new Polyline<Vector2d>.PointDescriptoinPair(new Vector2d(values[0], values[1]), elements.Count > 2 ? elements[2] : null));
					}

					result.Add(bln);
				}
			}
			return result.ToArray();
		}
	}
}
