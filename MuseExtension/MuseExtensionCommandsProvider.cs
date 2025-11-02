// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace MuseExtension;

public partial class MuseExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public MuseExtensionCommandsProvider()
    {
        DisplayName = "Muse";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        _commands = [
            new CommandItem(new MuseExtensionPage()) { Title = DisplayName, Subtitle = "Look up a word." },
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }

}
