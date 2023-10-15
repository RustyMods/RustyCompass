using System;
using HarmonyLib;
using UnityEngine;

namespace RustyCompass.Patches;

public static class TamePatches
{
    [HarmonyPatch(typeof(Tameable), nameof(Tameable.Update))]
    static class TameUpdatePatch
    {
        private static void Postfix(Tameable __instance)
        {
            if (!__instance || !Player.m_localPlayer) return;
            try
            {
                __instance.TryGetComponent(out ZNetView zNet);
                if (!zNet) return;
                ZDO? zdo = zNet.GetZDO();
                if (!zdo.IsValid()) return;
                string? name = __instance.GetHoverName();
                if (name == null) return;
                if (!__instance.m_monsterAI) return;
                GameObject? target = __instance.m_monsterAI.GetFollowTarget();
                if (target != null)
                {
                    if (target.GetComponent<Player>().GetHoverName() == Player.m_localPlayer.GetHoverName())
                    {
                        if (MinimapPatches.MinimapAwakePatch.TempTames.ContainsKey(zdo)) return;
                        MinimapPatches.MinimapAwakePatch.TempTames.Add(zdo, name);
                    }
                }
                if (!__instance.HaveSaddle()) return;
                if (!__instance.m_saddle.isActiveAndEnabled) return;
                    
                if (MinimapPatches.MinimapAwakePatch.TempTames.ContainsKey(zdo)) return;
                MinimapPatches.MinimapAwakePatch.TempTames.Add(zdo, name);
                
            } catch (NullReferenceException) {}
        }
    }
    [HarmonyPatch(typeof(Tameable), nameof(Tameable.Awake))]
    static class TameAwakePatch
    {
        private static void Postfix(Tameable __instance)
        {
            if (!__instance || !Player.m_localPlayer) return;
            __instance.TryGetComponent(out ZNetView zNet);
            if (!zNet) return;
            ZDO zdo = zNet.GetZDO();
            if (!zdo.IsValid()) return;
            string name = __instance.GetHoverName();
            if (name == null) return;
            if (!__instance.m_monsterAI) return;
            GameObject target = __instance.m_monsterAI.GetFollowTarget();
            if (target != null)
            {
                if (target.GetComponent<Player>().GetHoverName() == Player.m_localPlayer.GetHoverName())
                {
                    if (MinimapPatches.MinimapAwakePatch.TempTames.ContainsKey(zdo)) return;
                    MinimapPatches.MinimapAwakePatch.TempTames.Add(zdo, name);
                }
            }

            if (!__instance.HaveSaddle()) return;
            if (!__instance.m_saddle.isActiveAndEnabled) return;
            if (MinimapPatches.MinimapAwakePatch.TempTames.ContainsKey(zdo)) return;
            MinimapPatches.MinimapAwakePatch.TempTames.Add(zdo, name);
        }
    }

    [HarmonyPatch(typeof(Tameable), nameof(Tameable.UnSummon))]
    static class TameUnsummonPatch
    {
        private static void Postfix(Tameable __instance)
        {
            MinimapPatches.MinimapAwakePatch.TempTames.Clear();
        }
    }

    [HarmonyPatch(typeof(Tameable), nameof(Tameable.RPC_SetSaddle))]
    static class TameSetSaddlePatch
    {
        private static void Postfix(Tameable __instance)
        {
            if (!__instance) return;
            __instance.TryGetComponent(out ZNetView zNet);
            if (!zNet) return;
            ZDO zdo = zNet.GetZDO();
            string name = __instance.GetHoverName();
            if (__instance.m_saddle.isActiveAndEnabled)
            {
                MinimapPatches.MinimapAwakePatch.TempTames.Add(zdo, name);
            }
            else
            {
                MinimapPatches.MinimapAwakePatch.TempTames.Remove(zdo);
            }
        }
    }
    
    [HarmonyPatch(typeof(Tameable), nameof(Tameable.RPC_Command))]
    static class TameCommandPatch
    {
        private static void Postfix(Tameable __instance)
        {
            if (!__instance) return;
            Player player = Player.m_localPlayer;
            if (!player) return;
            try
            {
                __instance.TryGetComponent(out ZNetView zNet);
                if (!zNet) return;
                ZDO zdo = zNet.GetZDO();
                if (!zdo.IsValid()) return;
                if (!__instance.m_monsterAI) return;
                GameObject? target = __instance.m_monsterAI.GetFollowTarget();
                if (target != null)
                {
                    string playerName = player.GetHoverName();
                    string targetName = target.GetComponent<Player>().GetHoverName();
                    if (playerName == targetName)
                    {
                        if (MinimapPatches.MinimapAwakePatch.TempTames.ContainsKey(zdo)) return;
                        MinimapPatches.MinimapAwakePatch.TempTames.Add(zdo, __instance.GetHoverName());
                    }
                    else
                    {
                        MinimapPatches.MinimapAwakePatch.TempTames.Remove(zdo);
                    }
                }
                else
                {
                    MinimapPatches.MinimapAwakePatch.TempTames.Remove(zdo);
                }
                
            } catch (NullReferenceException) {}
        }
    }

    [HarmonyPatch(typeof(Tameable), nameof(Tameable.RPC_SetName))]
    static class TameSetNamePatch
    {
        private static void Postfix(Tameable __instance)
        {
            if (!__instance) return;
            string name = __instance.GetHoverName();
            __instance.TryGetComponent(out ZNetView zNet);
            if (!zNet) return;
            ZDO zdo = zNet.GetZDO();
            if (!zdo.IsValid()) return;
            if (MinimapPatches.MinimapAwakePatch.TempTames.ContainsKey(zdo))
            {
                MinimapPatches.MinimapAwakePatch.TempTames[zdo] = name;
            }
        }
    }

    [HarmonyPatch(typeof(Tameable), nameof(Tameable.OnDeath))]
    static class TameOnDeathPatch
    {
        private static void Postfix(Tameable __instance)
        {
            if (!__instance) return;
            __instance.TryGetComponent(out ZNetView zNet);
            if (!zNet) return;
            ZDO zdo = zNet.GetZDO();
            if (!zdo.IsValid()) return;
            if (MinimapPatches.MinimapAwakePatch.TempTames.ContainsKey(zdo)) MinimapPatches.MinimapAwakePatch.TempTames.Remove(zdo);
        }
    }
}