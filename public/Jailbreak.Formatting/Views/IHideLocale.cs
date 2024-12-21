using Jailbreak.Formatting.Base;

namespace Jailbreak.Formatting.Views;

public interface IHideLocale {
  public IView HideEnabled { get; }
  public IView HideDisabled { get; }
}