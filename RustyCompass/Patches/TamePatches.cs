using System;
using HarmonyLib;

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
                string? name = __instance.GetHoverName();
                ZDO? zdo = __instance.gameObject.GetComponent<ZNetView>().GetZDO();
                var target = __instance.m_monsterAI.GetFollowTarget();
                if (target != null)
                {
                    if (target.GetComponent<Player>().GetHoverName() == Player.m_localPlayer.GetHoverName())
                    {
                        if (MinimapPatches.MinimapAwakePatch.TempTames.ContainsKey(zdo)) return;
                        MinimapPatches.MinimapAwakePatch.TempTames.Add(zdo, name);
                    }
                }

                if (__instance.HaveSaddle())
                {
                    if (__instance.m_saddle.isActiveAndEnabled)
                    {
                        if (MinimapPatches.MinimapAwakePatch.TempTames.ContainsKey(zdo)) return;
                        MinimapPatches.MinimapAwakePatch.TempTames.Add(zdo, name);
                    }
                }
            }
            catch (ArgumentNullException)
            {
                // MinimapAwakePatch.TempTames.Clear();
                // MinimapAwakePatch.TempTamePins.Clear();
            }
        }
    }
    [HarmonyPatch(typeof(Tameable), nameof(Tameable.Awake))]
    static class TameAwakePatch
    {
        private static void Postfix(Tameable __instance)
        {
            if (!__instance || !Player.m_localPlayer) return;
            var name = __instance.GetHoverName();
            if (name == null) return;
            var zdo = __instance.gameObject.GetComponent<ZNetView>().GetZDO();
            if (!zdo.IsValid()) return;
            var target = __instance.m_monsterAI.GetFollowTarget();
            if (target != null)
            {
                if (target.GetComponent<Player>().GetHoverName() == Player.m_localPlayer.GetHoverName())
                {
                    if (MinimapPatches.MinimapAwakePatch.TempTames.ContainsKey(zdo)) return;
                    MinimapPatches.MinimapAwakePatch.TempTames.Add(zdo, name);
                }
            }

            if (__instance.HaveSaddle())
            {
                if (__instance.m_saddle.isActiveAndEnabled)
                {
                    if (MinimapPatches.MinimapAwakePatch.TempTames.ContainsKey(zdo)) return;
                    MinimapPatches.MinimapAwakePatch.TempTames.Add(zdo, name);
                }
            };
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
            var name = __instance.GetHoverName();
            var zdo = __instance.gameObject.GetComponent<ZNetView>().GetZDO();
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
            var target = __instance.m_monsterAI.GetFollowTarget();
            var player = Player.m_localPlayer;
            if (!player) return;
            
            var zdo = __instance.gameObject.GetComponent<ZNetView>().GetZDO();
            if (target != null)
            {
                var playerName = player.GetHoverName();
                var targetName = target.GetComponent<Player>().GetHoverName();
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
        }
    }

    [HarmonyPatch(typeof(Tameable), nameof(Tameable.RPC_SetName))]
    static class TameSetNamePatch
    {
        private static void Postfix(Tameable __instance)
        {
            if (!__instance) return;
            var name = __instance.GetHoverName();
            var zdo = __instance.gameObject.GetComponent<ZNetView>().GetZDO();
            
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
            var zdo = __instance.gameObject.GetComponent<ZNetView>().GetZDO();
            if (MinimapPatches.MinimapAwakePatch.TempTames.ContainsKey(zdo)) MinimapPatches.MinimapAwakePatch.TempTames.Remove(zdo);
        }
    }
    

    
}