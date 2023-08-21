using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ErnestoKava.Geophysics.Types.Lines;

namespace ErnestoKava.Geophysics.Serializers.SurferBln
{
	public static class BLNWriter
	{
		public static void Save(this BlnElement data, Stream stream)
		{
			Save(new[] {data}, stream);
		}

		private static void Crop(List<string> data)
		{
			while (data.Count > 0 && String.IsNullOrEmpty(data.Last()))
				data.RemoveAt(data.Count - 1);
		}

		public static void Save(this IEnumerable<BlnElement> data, Stream stream)
		{
			var sb = new StringBuilder();

			foreach (var bln in data)
			{
				var header = new List<string>() {
					bln.Points.Count.ToString(),
					"0", // TODO: replace with direction
					"",  // TODO
					bln.Title != null ? $"\"{bln.Title}\"" : null
				};

				Crop(header);
				sb.AppendLine(String.Join(",", header));
				foreach (var p in bln.Points)
				{
					var item = new List<string>() {
						p.Point.X.ToString(),
						p.Point.Y.ToString(),
						p.Description != null ? $"\"{p.Description}\"" : null
                    };
					Crop(item);
					sb.AppendLine(String.Join(",", item));
				}
			}

			var sw = new StreamWriter(stream);
			sw.Write(sb.ToString());
			sw.Flush();
		}
	}
}
