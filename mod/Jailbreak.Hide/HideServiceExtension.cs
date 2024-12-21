using Jailbreak.Public.Extensions;
using Jailbreak.Public.Mod.Hide;
using Microsoft.Extensions.DependencyInjection;

namespace Jailbreak.Hide;

public static class HideServiceExtension {
  public static void AddJailbreakHide(this IServiceCollection services) {
    services.AddPluginBehavior<IHideService, HideService>();
  }
}