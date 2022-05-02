using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FindReplace.Utils
{
	class TextFinder
	{
		public static int CountMatches(string searchString, string path)
		{
			try
			{
				File.Exists(path);
			}
			catch (Exception e)
			{
				throw;
			}

			try
			{
				string fileContent;
				fileContent = File.ReadAllText(path);
				int index = fileContent.IndexOf(searchString, 0);
				int numberOfMathces = 0;

				while (index > -1)
				{
					numberOfMathces++;

					index = fileContent.IndexOf(searchString, index + searchString.Length);
				}

				return numberOfMathces;
			}
			catch (Exception e)
			{
				throw;
			}
		}
	}
}
