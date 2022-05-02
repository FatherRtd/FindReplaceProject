using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FindReplace.Utils
{
	class FileFinder
	{

		public static List<string> FindFiles(string path)
		{
			try
			{
				List<string> files = new List<string>();
				files = Directory.EnumerateFiles(path).ToList();
				return files;
			}
			catch (Exception e)
			{
				throw;
			}
		}
		public static List<string> FindFiles(string path, string mask, System.IO.SearchOption searchOption)
		{
			try
			{
				List<string> files = new List<string>();
				files = Directory.EnumerateFiles(path, mask, searchOption).ToList();
				return files;
			}
			catch (Exception e)
			{
				throw;
			}
		}
		public static List<string> FindFiles(string path, string mask, string excludeMask, System.IO.SearchOption searchOption)
		{
			try
			{
				List<string> files = new List<string>();
				files = Directory.EnumerateFiles(path, mask, searchOption).Where((name => !name.Contains(excludeMask))).ToList();
				return files;
			}
			catch (Exception e)
			{
				throw;
			}
		}
	}
}
