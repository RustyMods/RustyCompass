using System.Collections.Generic;
using HarmonyLib;

namespace RustyCompass.Patches;

public class MinimapPatches
{
    [HarmonyPatch(typeof(Minimap), nameof(Minimap.Awake))]
    public static class MinimapAwakePatch
    {
        public static readonly Dictionary<ZDO, string> TempTames = new();
        public static readonly List<Minimap.PinData> TempTamePins = new();
    }

    [HarmonyPatch(typeof(Minimap), nameof(Minimap.OnDestroy))]
    static class MinimapOnDestroyPatch
    {
        private static void Postfix(Tameable __instance)
        {
            MinimapAwakePatch.TempTames.Clear();
            MinimapAwakePatch.TempTamePins.Clear();
        }
    }

    [HarmonyPatch(typeof(Minimap), nameof(Minimap.UpdatePlayerPins))]
    static class MinimapUpdatePatches
    {
        private static void Postfix(Minimap __instance)
        {
            if (!__instance || Player.m_localPlayer == null) return;
            var icon = __instance.m_icons[3];

            foreach (Minimap.PinData obj in MinimapAwakePatch.TempTamePins) __instance.RemovePin(obj);
            MinimapAwakePatch.TempTamePins.Clear();
            
            if (MinimapAwakePatch.TempTames.Count == 0) return;
            if (RustyCompassPlugin._TameTrackEnabled.Value != RustyCompassPlugin.Toggle.On) return;

            foreach (KeyValuePair<ZDO,string> item in MinimapAwakePatch.TempTames)
            {
                if (!item.Key.IsValid()) continue;
                Minimap.PinData pinData = new Minimap.PinData
                {
                    m_type = Minimap.PinType.Player,
                    m_pos = item.Key.GetPosition(),
                    m_icon = icon.m_icon,
                    m_save = false,
                    m_checked = false,
                    m_name = item.Value,
                    m_ownerID = 0L
                };
                pinData.m_NamePinData = new Minimap.PinNameData(pinData);
                __instance.CreateMapNamePin(pinData, __instance.m_pinNameRootSmall);
                MinimapAwakePatch.TempTamePins.Add(pinData);
            }
            foreach (Minimap.PinData data in MinimapAwakePatch.TempTamePins) __instance.m_pins.Add(data);
        }
    }
}