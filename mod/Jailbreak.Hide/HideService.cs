using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using Jailbreak.Formatting.Extensions;
using Jailbreak.Formatting.Views;
using Jailbreak.Public.Behaviors;
using Jailbreak.Public.Mod.Hide;

namespace Jailbreak.Hide;

public class HideService(BasePlugin plugin, IHideLocale locale)
  : IPluginBehavior, IHideService {

  private Dictionary<int, bool> HideStatus { get; } = new();

  [ConsoleCommand("css_hide", "Toggle hiding of other players.")]
  public void Toggle(CCSPlayerController player, CommandInfo info) {
    if (HideStatus.ContainsKey(player.Slot)) {
      HideStatus[player.Slot] = !HideStatus[player.Slot];
      locale.HideDisabled.ToChat(player);
    } else {
      HideStatus.Add(player.Slot, false);
      locale.HideEnabled.ToChat(player);
    }
  }
  public void UnHideAll() {
    HideStatus.Clear();
  }
  
  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info) {
    UnHideAll();
    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info) {
    UnHideAll();
    return HookResult.Continue;
  }

  public void Start(BasePlugin BasePlugin) {
    plugin = BasePlugin;
    plugin.RegisterListener<Listeners.CheckTransmit>(
      infoList => { 
        List<CCSPlayerController> players = Utilities.GetPlayers();
        foreach ((CCheckTransmitInfo info, CCSPlayerController? player) in
          infoList) {
          if (player == null) continue;
          if (HideStatus[player.Slot] || !HideStatus.ContainsKey(player.Slot))
            continue;
          IEnumerable<CCSPlayerController> targetPlayers = players.Where(p
            => p.IsValid && p.Pawn.IsValid && p.Slot != player.Slot
            && p.Team == player.Team && p.PlayerPawn.Value?.LifeState
            == (byte)LifeState_t.LIFE_ALIVE);
          foreach (CCSPlayerController targetPlayer in targetPlayers) {
            info.TransmitEntities.Remove(targetPlayer.Pawn);
          }
        }
        
      });
  }
  
}