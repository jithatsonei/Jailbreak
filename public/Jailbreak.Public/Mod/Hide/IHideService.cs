using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace Jailbreak.Public.Mod.Hide;

public interface IHideService {
  void Toggle(CCSPlayerController player, CommandInfo info);
  void UnHideAll();
}