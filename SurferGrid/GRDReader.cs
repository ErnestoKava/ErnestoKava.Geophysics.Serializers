using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ErnestoKava.Geophysics.Types.Grids;

namespace ErnestoKava.Geophysics.Serializers.SurferGrid
{
	public class GRDReader
	{
		public static Grid Read(string filename)
		{
			return Read(File.Open(filename, FileMode.Open));
		}

		public static Grid Read(Stream stream)
		{
			BinaryReader breader = new BinaryReader(stream);

			double[,] result = new double[1,1];
			int nRow = 0;
			int nCol = 0;
			double xLL = 0;
			double yLL = 0;
			double xSize = 0;
			double ySize = 0;
			double zMin = 0;
			double zMax = 0;
			double Rotation = 0;
			double BlankValue = 0;

			try
			{
				while (true)
				{
					int ID = breader.ReadInt32();
					if (ID == 0x42525344)	// header DSRB
					{
						int Size = breader.ReadInt32();
						int Version = breader.ReadInt32();
						continue;
					}
					if (ID == 0x44495247)	// grid GRID
					{
						int Size = breader.ReadInt32();

						nRow = breader.ReadInt32();
						nCol = breader.ReadInt32();
						xLL = breader.ReadDouble();
						yLL = breader.ReadDouble();
						xSize = breader.ReadDouble();
						ySize = breader.ReadDouble();
						zMin = breader.ReadDouble();
						zMax = breader.ReadDouble();
						Rotation = breader.ReadDouble();
						BlankValue = breader.ReadDouble();

						result = new double[nCol, nRow];
						continue;
					}
					if (ID == 0x41544144)	// data
					{
						int Size = breader.ReadInt32();
						for (int i = 0; i < nRow; i++)
							for (int j = 0; j < nCol; j++)
								result[j, i] = breader.ReadDouble();

						Grid grid = new Grid(result, xLL, yLL, xSize, ySize);
						return grid;
					}
					if (ID == 0x49544c46)	// fault
					{
					}
				}
			}
			catch (EndOfStreamException)
			{
				return null;
			}
			finally
			{
				stream.Close();
			}
		}
	}
}
