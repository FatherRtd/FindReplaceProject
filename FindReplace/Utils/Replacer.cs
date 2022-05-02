using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FindReplace.Utils
{
	class Replacer
	{
		public static void Replace(string path, string searchString, string replaceString)
		{
			try
			{
				StreamReader reader = new StreamReader(path);
				string content = reader.ReadToEnd();
				reader.Close();
				content = Regex.Replace(content, searchString, replaceString);
				StreamWriter writer = new StreamWriter(path);
				writer.Write(content);
				writer.Close();
			}
			catch (Exception e)
			{
				throw;
			}
		}
	}
}
