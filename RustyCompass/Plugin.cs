using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ItemManager;
using JetBrains.Annotations;
using LocalizationManager;
using RustyCompass.Utilities;
using ServerSync;
using StatusEffectManager;
using UnityEngine;

namespace RustyCompass
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class RustyCompassPlugin : BaseUnityPlugin
    {
        internal const string ModName = "RustyCompass";
        internal const string ModVersion = "1.0.2";
        internal const string Author = "RustyMods";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);

        public static readonly ManualLogSource RustyCompassLogger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync ConfigSync = new(ModGUID)
        {
            DisplayName = ModName, 
            // CurrentVersion = ModVersion, 
            // MinimumRequiredVersion = ModVersion
        };

        public static Sprite? CompassSprite;
        public static Sprite? CompassArrow;
        public static Sprite? CompassSimple;
        public static Sprite? CompassBW;
        public static Sprite? CompassViking;

        public static Sprite? NorthIcon;
        public static Sprite? SouthIcon;
        public static Sprite? EastIcon;
        public static Sprite? WestIcon;
        public static Sprite? RuneIcon;

        public enum Toggle
        {
            On = 1, 
            Off = 0
        }

        public enum Compass
        {
            Compass1 = 0,
            Compass2 = 1,
            Compass3 = 2,
            Compass4 = 3
        }

        public enum CompassType
        {
            Circle = 0,
            Bar = 1,
        }
        
        public void Awake()
        {
            Localizer.Load();
            
            #region Configurations
            _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On,
                "If on, the configuration is locked and can be changed by server admins only.");
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);
            
            _isModActive = config(
                "1 - General Settings",
                "_Enabled",
                Toggle.On,
                "If on, mod is active",
                false
                );
            _CompassType = config("1 - General Settings", "Compass Type", CompassType.Bar, "Compass bar or circle",
                false);

            _useCompassTokens = config("1 - General Settings", "Use Compass Tokens", Toggle.Off,
                "If on, compass features are enabled using quest items", true);

            Vector2 defaultPos = new Vector2(820f, 400f);
            _CompassPosition = config(
                "2 - Circle Compass Settings",
                "Compass Position",
                defaultPos,
                "Alignment of compass",
                false
                );
            Vector2 defaultSize = new Vector2(200f, 200f);
            _CompassSize = config(
                "2 - Circle Compass Settings",
                "Compass Size",
                defaultSize,
                "Size of compass",
                false
                );

            _CompassColor = config(
                "2 - Circle Compass Settings",
                "Compass Color",
                Color.white, 
                "Color of the compass",
                false
                );
            _HandColor = config(
                "2 - Circle Compass Settings",
                "Hand Color",
                Color.yellow,
                "Color of the hand on compass",
                false
                );
            _WindMarkerColor = config(
                "2 - Circle Compass Settings",
                "Wind Marker Color",
                Color.white, 
                "Wind Marker Color",
                false
                );
            _BiomesColor = config(
                "2 - Circle Compass Settings",
                "Biomes Name Color",
                Color.white, 
                "Biomes Text Color",
                false
                );
            
            _CompassSprite = config(
                "2 - Circle Compass Settings",
                "Compass Image",
                Compass.Compass1,
                "Compass sprite options",
                false
                );

            _CompassBarColor = config(
                "3 - Bar Compass Settings", "Compass Bar Color",
                Color.white, "Compass Bar Color", false
                );

            Vector2 defaultBarIconSize = new Vector2(50f, 50f);
            _CompassBarIconSize = config(
                "3 - Bar Compass Settings", "Compass Icon Size", defaultBarIconSize, "Icon Size", false);

            _CompassBarIconSpacing = config("3 - Bar Compass Settings", "Compass Bar Icon Spacing", 25f,
                "Spacing between icons", false);
            
            _CompassBarPosition = config("3 - Bar Compass Settings", "Compass Bar Position", 200f,
                "Compass Bar Vertical Position", false);

            _CompassPinsColor = config("3 - Bar Compass Settings", "Compass Pins Color", Color.white,
                "Color of the pins on the bar", false);
            _CompassPinsEnabled = config("3 - Bar Compass Settings", "Bar Pins Enabled", Toggle.On,
                "If on, pins are added to the compass bar", true);
            _CompassPinsMaxDistance = config("3 - Bar Compass Settings", "Bar Pins Max Distance", 500f,
                "Max distance pins appear on bar", false);
            _CompassPinsMaxSize = config("3 - Bar Compass Settings", "Bar Pins Max Size", 50f,
                "Size of compass bar pins", false);
            #endregion
            
            #region Register sprites
            CompassSprite = SpriteManager.RegisterSprite("rustyCompassIcon.png");
            CompassArrow = SpriteManager.RegisterSprite("compassArrowIcon.png");
            CompassSimple = SpriteManager.RegisterSprite("rustyCompassSimple.png");
            CompassBW = SpriteManager.RegisterSprite("rustyCompassBW.png");
            CompassViking = SpriteManager.RegisterSprite("rustyCompassViking.png");

            NorthIcon = SpriteManager.RegisterSprite("compassNorth.png");
            SouthIcon = SpriteManager.RegisterSprite("compassSouth.png");
            EastIcon = SpriteManager.RegisterSprite("compassEast.png");
            WestIcon = SpriteManager.RegisterSprite("compassWest.png");
            RuneIcon = SpriteManager.RegisterSprite("compassRune.png");
            #endregion
            #region Register Prefabs
            Item CompassToken1 = new("compassbundle", "RS_Compass_Token_Gold");
            CompassToken1.Name.English("Lost Compass Token");
            CompassToken1.Description.English("Awakens the third eye, giving access to a compass bar");
            CompassToken1.Crafting.Add(CraftingTable.Forge, 2);
            CompassToken1.RequiredItems.Add("Bronze", 5);
            CompassToken1.RequiredItems.Add("Resin", 20);
            CompassToken1.RequiredItems.Add("Flint", 5);
            CompassToken1.RequiredItems.Add("CryptKey", 1);
            CompassToken1.CraftAmount = 1;
            CompassToken1.Configurable = Configurability.Recipe;

            Item CompassToken2 = new("compassbundle", "RS_Compass_Token_Brass");
            CompassToken2.Name.English("Rusty Compass Token");
            CompassToken2.Description.English("Enables the use of a circular compass");
            CompassToken2.Crafting.Add(CraftingTable.Workbench, 2);
            CompassToken2.RequiredItems.Add("Copper", 5);
            CompassToken2.RequiredItems.Add("Resin", 10);
            CompassToken2.RequiredItems.Add("Flint", 5);
            CompassToken2.RequiredItems.Add("HardAntler", 1);
            CompassToken2.CraftAmount = 1;
            CompassToken2.Configurable = Configurability.Recipe;

            Item CompassToken3 = new("compassbundle", "RS_Compass_Token_Silver");
            CompassToken3.Name.English("Silver Compass Token");
            CompassToken3.Description.English("Enables the use of a circular compass and a deeper understanding of their location as well as wind direction");
            CompassToken3.Crafting.Add(CraftingTable.ArtisanTable, 1);
            CompassToken3.RequiredItems.Add("Silver", 5);
            CompassToken3.RequiredItems.Add("Resin", 20);
            CompassToken3.RequiredItems.Add("Obsidian", 5);
            CompassToken3.RequiredItems.Add("Crystal", 10);
            CompassToken3.CraftAmount = 1;
            CompassToken3.Configurable = Configurability.Recipe;
            
            Item CompassToken4 = new("compassbundle", "RS_Compass_Token_DarkGold");
            CompassToken4.Name.English("Forgotten Compass Token");
            CompassToken4.Description.English("Grants full usage of the compass bar with senses of their surroundings");
            CompassToken4.Crafting.Add(CraftingTable.ArtisanTable, 1);
            CompassToken4.RequiredItems.Add("BlackMetal", 5);
            CompassToken4.RequiredItems.Add("Resin", 20);
            CompassToken4.RequiredItems.Add("DragonTear", 1);
            CompassToken4.RequiredItems.Add("Coins", 100);
            CompassToken4.CraftAmount = 1;
            CompassToken4.Configurable = Configurability.Recipe;
            #endregion
            #region Create custom Status Effects
            CustomSE CompassGoldEffect = new("se_compass_gold");
            CompassGoldEffect.Name.English("Lost Compass");
            CompassGoldEffect.Type = EffectType.Equip;
            CompassGoldEffect.IconSprite = EastIcon;
            CompassGoldEffect.Effect.m_startMessage = "Lost Compass is active";
            CompassGoldEffect.Effect.m_startMessageType = MessageHud.MessageType.Center;
            CompassGoldEffect.Effect.m_stopMessage = "";
            CompassGoldEffect.Effect.m_stopMessageType = MessageHud.MessageType.TopLeft;
            CompassGoldEffect.Effect.m_tooltip = "Your sense of direction is elevated";
            CompassGoldEffect.AddSEToPrefab(CompassGoldEffect, "RS_Compass_Token_Gold");
            
            CustomSE CompassBrassEffect = new("se_compass_brass");
            CompassBrassEffect.Name.English("Rusty Compass");
            CompassBrassEffect.Type = EffectType.Equip;
            CompassBrassEffect.IconSprite = WestIcon;
            CompassBrassEffect.Effect.m_startMessage = "Rusty Compass is active";
            CompassBrassEffect.Effect.m_startMessageType = MessageHud.MessageType.Center;
            CompassBrassEffect.Effect.m_stopMessage = "";
            CompassBrassEffect.Effect.m_stopMessageType = MessageHud.MessageType.TopLeft;
            CompassBrassEffect.Effect.m_tooltip = "Your sense of direction is elevated";
            CompassBrassEffect.AddSEToPrefab(CompassBrassEffect, "RS_Compass_Token_Brass");
            
            CustomSE CompassSilverEffect = new("se_compass_silver");
            CompassSilverEffect.Name.English("Silver Compass");
            CompassSilverEffect.Type = EffectType.Equip;
            CompassSilverEffect.IconSprite = SouthIcon;
            CompassSilverEffect.Effect.m_startMessage = "Silver Compass is active";
            CompassSilverEffect.Effect.m_startMessageType = MessageHud.MessageType.Center;
            CompassSilverEffect.Effect.m_stopMessage = "";
            CompassSilverEffect.Effect.m_stopMessageType = MessageHud.MessageType.TopLeft;
            CompassSilverEffect.Effect.m_tooltip = "Your sense of direction is elevated";
            CompassSilverEffect.AddSEToPrefab(CompassSilverEffect, "RS_Compass_Token_Silver");
            
            CustomSE CompassDarkGoldEffect = new("se_compass_darkgold");
            CompassDarkGoldEffect.Name.English("Forgotten Compass");
            CompassDarkGoldEffect.Type = EffectType.Equip;
            CompassDarkGoldEffect.IconSprite = SouthIcon;
            CompassDarkGoldEffect.Effect.m_startMessage = "Forgotten Compass is active";
            CompassDarkGoldEffect.Effect.m_startMessageType = MessageHud.MessageType.Center;
            CompassDarkGoldEffect.Effect.m_stopMessage = "";
            CompassDarkGoldEffect.Effect.m_stopMessageType = MessageHud.MessageType.TopLeft;
            CompassDarkGoldEffect.Effect.m_tooltip = "Your sense of direction is elevated";
            CompassDarkGoldEffect.AddSEToPrefab(CompassDarkGoldEffect, "RS_Compass_Token_DarkGold");
            #endregion
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        private void OnDestroy()
        {
            Config.Save();
        }

        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                RustyCompassLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                RustyCompassLogger.LogError($"There was an issue loading your {ConfigFileName}");
                RustyCompassLogger.LogError("Please check your config entries for spelling and format!");
            }
        }


        #region ConfigOptions

        private static ConfigEntry<Toggle> _serverConfigLocked = null!;
        
        // General Settings
        public static ConfigEntry<Toggle> _isModActive = null!;
        public static ConfigEntry<CompassType> _CompassType = null!;
        public static ConfigEntry<Toggle> _useCompassTokens = null!;

        // Circle Compass Settings
        public static ConfigEntry<Color> _BiomesColor = null!;
        public static ConfigEntry<Color> _CompassColor = null!;
        public static ConfigEntry<Compass> _CompassSprite = null!;
        public static ConfigEntry<Vector2> _CompassPosition = null!;
        public static ConfigEntry<Vector2> _CompassSize = null!;
        public static ConfigEntry<Color> _HandColor = null!;
        public static ConfigEntry<Color> _WindMarkerColor = null!;
        // Bar Compass Settings
        public static ConfigEntry<Color> _CompassBarColor = null!;
        public static ConfigEntry<Vector2> _CompassBarIconSize = null!;
        public static ConfigEntry<float> _CompassBarIconSpacing = null!;
        public static ConfigEntry<float> _CompassBarPosition = null!;

        public static ConfigEntry<Color> _CompassPinsColor = null!;
        public static ConfigEntry<Toggle> _CompassPinsEnabled = null!;
        public static ConfigEntry<float> _CompassPinsMaxDistance = null!;
        public static ConfigEntry<float> _CompassPinsMaxSize = null!;
        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private class ConfigurationManagerAttributes
        {
            [UsedImplicitly] public int? Order = null!;
            [UsedImplicitly] public bool? Browsable = null!;
            [UsedImplicitly] public string? Category = null!;
            [UsedImplicitly] public Action<ConfigEntryBase>? CustomDrawer = null!;
        }

        class AcceptableShortcuts : AcceptableValueBase
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                "# Acceptable values: " + string.Join(", ", UnityInput.Current.SupportedKeyCodes);
        }

        #endregion
    }
}