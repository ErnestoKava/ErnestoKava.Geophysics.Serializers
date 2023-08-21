using System;
using System.Linq;
using System.Globalization;
using System.IO;
using ErnestoKava.Geophysics.Types.Grids;

namespace ErnestoKava.Geophysics.Serializers.SurferGrid
{
	public static class GRDWriter
	{
		static public bool Save(this Grid grid, Stream stream, bool binary = true)
		{
			var minz = grid.Min(g => g.Z);
			var maxz = grid.Max(g => g.Z);

			if (binary)
			{
				BinaryWriter writer = new BinaryWriter(stream);

				// DSRB
				writer.Write((UInt32)0x42525344);
				writer.Write((UInt32)0x00000004);
				writer.Write((UInt32)0x00000001);

				// GRID
				writer.Write((UInt32)0x44495247);
				writer.Write((UInt32)(sizeof(UInt32) * 2 + sizeof(Double) * 8));

				writer.Write((UInt32)grid.nRow);
				writer.Write((UInt32)grid.nCol);
				writer.Write((Double)grid.xLL);
				writer.Write((Double)grid.yLL);
				writer.Write((Double)grid.xSize);
				writer.Write((Double)grid.ySize);
				writer.Write((Double)minz);
				writer.Write((Double)maxz);
				writer.Write((Double)0);
				writer.Write((Double)grid.Blank);

				writer.Write((UInt32)0x41544144);
				writer.Write((UInt32)(sizeof(Double) * grid.nCol * grid.nRow));

				for (int i = 0; i < grid.nRow; i++)
					for (int j = 0; j < grid.nCol; j++)
					{
						writer.Write((Double)grid.data[j, i]);
					}

				writer.Close();
			}
			else
			{
				StreamWriter writer = new StreamWriter(stream);

				var culture = new CultureInfo("en-US");

				writer.WriteLine("DSAA");
				writer.WriteLine(String.Format("{0} {1}", grid.nCol.ToString(culture), grid.nRow.ToString(culture)));
				writer.WriteLine(String.Format("{0} {1}", grid.xLL.ToString(culture), grid.xUR.ToString(culture)));
				writer.WriteLine(String.Format("{0} {1}", grid.yLL.ToString(culture), grid.yUR.ToString(culture)));
				writer.WriteLine(String.Format("{0} {1}", minz.ToString(culture), maxz.ToString(culture)));

				for (int y = 0; y < grid.nRow; y++)
				{
					for (int x = 0; x < grid.nCol; x++)
					{
						writer.Write(grid.data[x, y].ToString(culture) + " ");
					}

					writer.WriteLine();
					writer.WriteLine();
				}

				writer.Close();
			}
			return true;
		}
		static public bool Save(this Grid grid, string fileName, bool binary = true)
		{
			using (FileStream fstream = File.Open(fileName, FileMode.OpenOrCreate))
			{
				return Save(grid, fstream, binary);
			}
		}
	}
}
