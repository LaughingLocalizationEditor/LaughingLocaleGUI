using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaughingLocale.Data
{
	public static class CommonFilters
	{
		public static List<string> CombineFilters(params FileDialogFilter[] filters)
		{
			return filters.SelectMany(f => f.Extensions).ToList();
		}

		public static FileDialogFilter All { get; private set; } = new FileDialogFilter()
		{
			Name = "All types",
			Extensions = { "*" }
		};

		public static FileDialogFilter NormalTextFile { get; private set; } = new FileDialogFilter()
		{
			Name = "Normal text file",
			Extensions = { "txt" }
		};

		public static FileDialogFilter TabSeparatedFile { get; private set; } = new FileDialogFilter()
		{
			Name = "Tab-Separated file",
			Extensions = { "tsv" }
		};

		public static FileDialogFilter CommaSeparatedFile { get; private set; } = new FileDialogFilter()
		{
			Name = "Comma-Separated file",
			Extensions = { "csv" }
		};

		public static FileDialogFilter DelimitedLocaleFiles { get; private set; } = new FileDialogFilter()
		{
			Name = "Delimited Localization file",
			Extensions = CombineFilters(TabSeparatedFile, CommaSeparatedFile, NormalTextFile)
		};

		public static List<FileDialogFilter> DefaultFilters { get; set; } = new List<FileDialogFilter>()
		{
			DelimitedLocaleFiles,
			TabSeparatedFile,
			CommaSeparatedFile,
			All
		};
	}
}
