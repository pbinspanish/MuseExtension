using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatamuseLib;

namespace MuseExtension.Pages
{
	internal partial class MuseWordPage : ListPage
	{
		public string term {  get; set; }

		public MuseWordPage(string term)
		{
			Icon = new IconInfo(string.Empty);
			Name = "Muse";

			ShowDetails = true;

			this.term = term;
		}

		public override IListItem[] GetItems()
		{
			IsLoading = true;
			var listItems = new List<ListItem>();

			var client = new DatamuseClient();
			var results = client.GetWordInfo(term);

			foreach (var result in results)
			{
				listItems.Add(new ListItem()
				{
					Title = result.Word,
					Subtitle = "Definitions",
					Details = new Details()
					{
						Title = result.Word,
						Metadata = GetDefinitionDetails(result.Definitions)
					}
				});
			}

			IsLoading = false;

			return listItems.ToArray();
		}

		private static DetailsElement[] GetDefinitionDetails(List<DatamuseDefinition> defs)
		{
			var listItems = new List<DetailsElement>();
			var counter = 1;

			foreach (var definition in defs)
			{
				listItems.Add(new DetailsElement()
				{
					Key = "Definition " + counter++,
					Data = new DetailsLink()
					{
						Text = definition.PartOfSpeech + " — " + definition.Text
					}
				});
			}

			return listItems.ToArray();
		}
	}
}
