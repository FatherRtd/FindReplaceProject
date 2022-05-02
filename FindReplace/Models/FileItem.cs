using System;
using System.Collections.Generic;
using System.Text;

namespace FindReplace.Models
{
	class FileItem
	{
		public string Path { get; set; }
		public int Matches { get; set; }

		public FileItem(string path, int matches)
		{
			Matches = matches;
			Path = path;
		}
	}
}
