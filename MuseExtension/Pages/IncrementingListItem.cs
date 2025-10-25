using DatamuseLib;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseExtension.Pages
{
	internal sealed partial class IncrementingListItem : ListItem
	{
		public IncrementingListItem(MuseExtensionPage page) :
			base(new NoOpCommand())
		{
			_page = page;
			//Command = new AnonymousCommand(action: _page.Increment) { Result = CommandResult.KeepOpen() };
			Title = "Increment";
		}

		private MuseExtensionPage _page;
	}
}
