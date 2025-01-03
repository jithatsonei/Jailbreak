﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Services;
using Jailbreak.Public;
using Jailbreak.Public.Behaviors;
using Jailbreak.Public.Extensions;
using Jailbreak.Public.Mod.LastRequest;
using Jailbreak.Public.Mod.Rebel;
using Microsoft.Extensions.DependencyInjection;

namespace Jailbreak.Rebel;

public class RebelListener(IRebelService rebelService,
  ILastRequestManager lastRequestManager) : IPluginBehavior {
  private readonly Dictionary<int, int> weaponScores = [];

  [GameEventHandler]
  public HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info) {
    var player = @event.Userid;
    if (player == null || !player.IsReal()) return HookResult.Continue;
    if (player.Team != CsTeam.CounterTerrorist) return HookResult.Continue;

    var attacker = @event.Attacker;
    if (attacker == null || !attacker.IsReal()) return HookResult.Continue;

    if (attacker.Team != CsTeam.Terrorist) return HookResult.Continue;
    if (lastRequestManager.IsInLR(attacker)) return HookResult.Continue;

    var weapon = "weapon_" + @event.Weapon;
    if (Tag.SNIPERS.Contains(weapon) && weapon != "weapon_ssg08")
      weaponScores[player.Slot] = 25;
    else if (Tag.RIFLES.Contains(weapon))
      weaponScores[player.Slot] = 20;
    else if (Tag.GUNS.Contains(weapon))
      weaponScores[player.Slot] = 15;
    else
      weaponScores[player.Slot] = 10;

    rebelService.MarkRebel(attacker);
    return HookResult.Continue;
  }

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart ev, GameEventInfo info) {
    weaponScores.Clear();
    return HookResult.Continue;
  }

  [GameEventHandler(HookMode.Pre)]
  public HookResult OnDeath(EventPlayerDeath ev, GameEventInfo info) {
    var player = ev.Userid;
    if (player == null || !player.IsValid || player.IsBot)
      return HookResult.Continue;
    if (player.Team != CsTeam.Terrorist) return HookResult.Continue;
    if (!rebelService.IsRebel(player)) return HookResult.Continue;

    var attacker = ev.Attacker;

    if (attacker == null || !attacker.IsValid || attacker.IsBot)
      return HookResult.Continue;

    var eco = API.Gangs?.Services.GetService<IEcoManager>();
    if (eco == null) return HookResult.Continue;

    var wrapper = new PlayerWrapper(attacker);
    if (!weaponScores.TryGetValue(player.Slot, out var weaponScore))
      weaponScore = 5;

    Task.Run(async ()
      => await eco.Grant(wrapper, weaponScore, true, "Rebel Kill"));
    return HookResult.Continue;
  }
}