using System.Collections.Generic;
using System.Text;

namespace TaycanLogger
{
    public static class MyExtensions
	{
		public static string ToCSV(this Dictionary<string, string> dict)
		{
			var sb = new StringBuilder();
			foreach (var element in dict)
			{
				sb.Append(element.Value.Replace(',', '.') + ",");
			}
			return sb.ToString();
		}
	}

}
