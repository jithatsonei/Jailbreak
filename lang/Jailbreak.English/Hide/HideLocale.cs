using CounterStrikeSharp.API.Modules.Utils;
using Jailbreak.Formatting.Base;
using Jailbreak.Formatting.Core;
using Jailbreak.Formatting.Logistics;
using Jailbreak.Formatting.Objects;
using Jailbreak.Formatting.Views;

namespace Jailbreak.English.Hide;

public class HideLocale : IHideLocale, ILanguage<Formatting.Languages.English> {
  public static readonly FormatObject PREFIX =
    new HiddenFormatObject($" {ChatColors.DarkBlue}Hide>") {
      //	Hide in panorama and center text
      Plain = false, Panorama = false, Chat = true
    };

  public IView HideEnabled => new SimpleView {
      {
        PREFIX,
        $"All teammates are now hidden. Type {ChatColors.BlueGrey}!unhide{ChatColors.Grey} to disable."
      }
    };

  public IView HideDisabled => new SimpleView {
      {
        PREFIX,
        $"All teammates are now unhidden. Type {ChatColors.BlueGrey}!hide{ChatColors.Grey} to re-enable."
      }
  };
}