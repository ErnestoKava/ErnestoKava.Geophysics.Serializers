using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErnestoKava.Geophysics.Types.Functions;

namespace ErnestoKava.Geophysics.Serializers.Functions
{
	public class LinearPiecewiseFunctionSerializer
	{
		public static string Serialize(LinearPiecewiseFunction function)
		{
			throw new NotImplementedException();
		}

		public static LinearPiecewiseFunction Deserialize(string data)
		{
			List<double> _boundaries = new List<double>();
			var _ab = new List<LinearPiecewiseFunction.ABPair>();

			var parts = data.Split('/');
			for (int i = 0; i < parts.Length; i++)
			{
				if ((i % 2) == 0)
					_boundaries.Add(double.Parse(parts[i]));
				else
				{
					var ab = parts[i].Split(',');
					_ab.Add(new LinearPiecewiseFunction.ABPair() {
						A = double.Parse(ab[0]), B = double.Parse(ab[1])
					});
				}
			}

			return new LinearPiecewiseFunction(_boundaries.ToArray(), _ab.ToArray());
		}
	}
}
