using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using DatamuseLib;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using MuseExtension.Pages;

namespace MuseExtension;

internal sealed partial class MuseExtensionPage : DynamicListPage
{
	public MuseExtensionPage()
	{
		Icon = new IconInfo(string.Empty);
		Name = "Muse";
		
		ShowDetails = true;
		EmptyContent = new CommandItem("Look up a word.");
	}

	public override void UpdateSearchText(string oldSearch, string newSearch) => RaiseItemsChanged();

	public override IListItem[] GetItems()
	{
		IsLoading = true;
		var listItems = new List<IListItem>();

		var results = DatamuseClient.GetSuggestions(SearchText);

		foreach (var result in results)
		{
			listItems.Add(new ListItem()
			{
				Title = result.Word,
				Subtitle = result.Score.ToString(),
				Command = new MuseWordPage(result.Word)
			});
		}

		IsLoading = false;
		return listItems.ToArray();
	}

	//public override IListItem[] GetItems()
	//{
	//	IsLoading = true;
	//	var listItems = new List<ListItem>();
		
	//	var client = new DatamuseClient();
	//	var results = client.GetWordInfo(SearchText);

	//	foreach (var result in results)
	//	{
	//		listItems.Add(new ListItem()
	//		{
	//			Title = "Definitions",
	//			Details = new Details()
	//			{
	//				Title = SearchText,
	//				Metadata = GetDefinitionDetails(result.Definitions)
	//			}
	//		});
	//	}

	//	IsLoading = false;

	//	return listItems.ToArray();
	//}

	//private static DetailsElement[] GetDefinitionDetails(List<DatamuseDefinition> defs)
	//{
	//	var listItems = new List<DetailsElement>();
	//	var counter = 1;

	//	foreach (var definition in defs)
	//	{
	//		listItems.Add(new DetailsElement()
	//		{
	//			Key = "Definition " + counter++,
	//			Data = new DetailsLink()
	//			{
	//				Text = definition.PartOfSpeech + " — " + definition.Text
	//			}
	//		});
	//	}

	//	return listItems.ToArray();
	//}
}