using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RustyCompass.Patches;

public static class HUDPatches
{
    [HarmonyPatch(typeof(Hud), nameof(Hud.Awake))]
    public static class HudPatch
    {
        public static TMP_FontAsset font = null!;
        private static GameObject root = null!;
        public static int compassIconCount;
        public static readonly Sprite fireIcon = Minimap.instance.m_icons[0].m_icon;
        // public static readonly Sprite shipIcon = Minimap.instance.m_largeShipMarker.GetComponent<Image>().sprite;
        // public static readonly Sprite playerIcon = Minimap.instance.m_largeMarker.GetComponent<Image>().sprite;
        public static readonly Sprite spawnIcon = Minimap.instance.m_locationIcons[0].m_icon;
        public static readonly Sprite merchantIcon = Minimap.instance.m_locationIcons[1].m_icon;
        public static readonly Sprite clothIcon = Minimap.instance.m_locationIcons[2].m_icon;
        public static readonly Sprite houseIcon = Minimap.instance.m_icons[1].m_icon;
        public static readonly Sprite anchorIcon =  Minimap.instance.m_icons[2].m_icon;
        public static readonly Sprite circleIcon = Minimap.instance.m_icons[3].m_icon;
        public static readonly Sprite portalIcon = Minimap.instance.m_icons[4].m_icon;
        public static readonly Sprite deathIcon = Minimap.instance.m_icons[5].m_icon;
        public static readonly Sprite bedIcon = Minimap.instance.m_icons[6].m_icon;
        public static readonly Sprite yellowTargetIcon = Minimap.instance.m_icons[7].m_icon;
        public static readonly Sprite bossIcon = Minimap.instance.m_icons[8].m_icon;
        public static readonly Sprite vikingIcon = Minimap.instance.m_icons[9].m_icon;
        public static readonly Sprite exclamationIcon = Minimap.instance.m_icons[10].m_icon;
        // public static readonly Sprite redRadiusIcon = Minimap.instance.m_icons[11].m_icon;
        public static readonly Sprite blueTargetIcon = Minimap.instance.m_icons[12].m_icon;
        public static readonly Sprite questionMarkIcon = Minimap.instance.m_icons[13].m_icon;
        private static void Postfix(Hud __instance)
        {
            root = __instance.m_rootObject;
            font = root.transform.Find("staminapanel").Find("Stamina").Find("StaminaText")
                .GetComponent<TextMeshProUGUI>().font;

            CreateCompassCircle();
            CreateCompassBar();
        }
        
        private static void CreateCompassBar()
        {
            if (RustyCompassPlugin.RuneIcon == null
                || RustyCompassPlugin.SouthIcon == null
                || RustyCompassPlugin.EastIcon == null
                || RustyCompassPlugin.NorthIcon == null
                || RustyCompassPlugin.WestIcon == null
                ) return;
            
            List<Sprite> compassIcons = new List<Sprite>()
            {
                RustyCompassPlugin.SouthIcon,
                RustyCompassPlugin.RuneIcon,
                RustyCompassPlugin.RuneIcon,
                RustyCompassPlugin.RuneIcon,
                RustyCompassPlugin.EastIcon,
                RustyCompassPlugin.RuneIcon,
                RustyCompassPlugin.RuneIcon,
                RustyCompassPlugin.RuneIcon,
                RustyCompassPlugin.NorthIcon,
                RustyCompassPlugin.RuneIcon,
                RustyCompassPlugin.RuneIcon,
                RustyCompassPlugin.RuneIcon,
                RustyCompassPlugin.WestIcon,
                RustyCompassPlugin.RuneIcon,
                RustyCompassPlugin.RuneIcon,
                RustyCompassPlugin.RuneIcon,
            };
            compassIconCount = compassIcons.Count;
                
            float positionY = RustyCompassPlugin._CompassBarPosition.Value;
            float iconSpacing = RustyCompassPlugin._CompassBarIconSpacing.Value;

            float totalWidth = compassIcons.Count * iconSpacing;
            float startX = -totalWidth / 2;

            for (int i = 0; i < compassIcons.Count; ++i)
            {
                float positionX = startX + i * iconSpacing;

                GameObject iconElement = new GameObject($"barIcon ({i})");
                RectTransform iconRect = iconElement.AddComponent<RectTransform>();
                iconRect.SetParent(root.transform);
                iconRect.sizeDelta = new Vector2(50f, 50f);
                iconRect.anchoredPosition = new Vector2(positionX, positionY);

                Image iconImage = iconElement.AddComponent<Image>();
                iconImage.sprite = compassIcons[i];
                iconImage.color = Color.white;
            }
        }

        private static void CreateCompassCircle()
        {
            GameObject CompassContainer = new GameObject("CompassContainer");
            RectTransform containerRect = CompassContainer.AddComponent<RectTransform>();
            containerRect.SetParent(root.transform);
            containerRect.anchoredPosition = RustyCompassPlugin._CompassPosition.Value;
            containerRect.sizeDelta = new Vector2(200f, 200f);
            
            GameObject background = new GameObject("background");
            RectTransform backgroundRect = background.AddComponent<RectTransform>();
            backgroundRect.SetParent(CompassContainer.transform);
            backgroundRect.anchoredPosition = new Vector2(0f, 0f);
            backgroundRect.sizeDelta = RustyCompassPlugin._CompassSize.Value;

            Image backgroundImage = background.AddComponent<Image>();
            backgroundImage.sprite = RustyCompassPlugin.CompassBW;
            backgroundImage.color = Color.white;
            backgroundImage.maskable = true;

            GameObject compassHand = new GameObject("hand");
            RectTransform handRect = compassHand.AddComponent<RectTransform>();
            handRect.SetParent(CompassContainer.transform);
            handRect.anchoredPosition = new Vector2(0f, 0f);
            handRect.sizeDelta = new Vector2(100f, 100f);

            Image handImage = compassHand.AddComponent<Image>();
            handImage.sprite = RustyCompassPlugin.CompassArrow;
            handImage.color = Color.white;

            GameObject biomes = new GameObject("biomes");
            RectTransform biomesRect = biomes.AddComponent<RectTransform>();
            biomesRect.SetParent(CompassContainer.transform);
            biomesRect.anchoredPosition = new Vector2(0f, 100f);
            biomesRect.sizeDelta = new Vector2(250f, 25f);

            TextMeshProUGUI biomesText = biomes.AddComponent<TextMeshProUGUI>();
            biomesText.color = Color.white;
            biomesText.font = font;
            biomesText.text = "";
            biomesText.fontSize = 20f;
            biomesText.fontSizeMax = 20f;
            biomesText.fontSizeMin = 2f;
            biomesText.enableAutoSizing = true;

            RectTransform m_windMarker = Minimap.m_instance.m_windMarker;
            Image windPin = m_windMarker.gameObject.GetComponent<Image>();
            GameObject windMarker = new GameObject("windMarker");
            RectTransform windMarkerRect = windMarker.AddComponent<RectTransform>();
            windMarkerRect.SetParent(CompassContainer.transform);
            windMarkerRect.anchoredPosition = new Vector2(-100f, -100f);
            windMarkerRect.sizeDelta = new Vector2(32f, 32f);

            Image windMarkerImage = windMarker.AddComponent<Image>();
            windMarkerImage.sprite = windPin.sprite;
            windMarkerImage.color = RustyCompassPlugin._WindMarkerColor.Value;
        }
    }
    [HarmonyPatch(typeof(Hud), nameof(Hud.Update))]
    public static class HudUpdatePatch
    {
        private static GameObject root = null!;
        private static Quaternion playerRotation;
        private static Vector3 playerPosition;

        private static readonly List<PinData> BarPins = new();
        private static readonly List<PinData> CirclePins = new();

        private static bool compassCircle;
        private static bool biomesName;
        private static bool windIcon;
        private static bool compassBarActive;
        private static bool barPinActive;
        private static bool circlePinActive;

        static void Postfix(Hud __instance)
        {
            root = __instance.m_rootObject;
            playerRotation = Utils.GetMainCamera().transform.rotation;
            playerPosition = Utils.GetMainCamera().transform.position;

            UpdateCompassCircle();
            UpdateCompassBar();
            UpdatePins();
            if (RustyCompassPlugin._useCompassTokens.Value == RustyCompassPlugin.Toggle.On) CheckPlayerInventory();
        }

        private static void CheckPlayerInventory()
        {
            if (Player.m_localPlayer == null) return;
            List<ItemDrop.ItemData> equippedItems = Player.m_localPlayer.GetInventory().GetEquippedItems();
            compassBarActive = false;
            compassCircle = false;
            barPinActive = false;
            biomesName = false;
            windIcon = false;
            circlePinActive = false;
            foreach (var item in equippedItems)
            {
                if (item.m_shared.m_name
                    is not ("$item_compass_token_brass"
                    or "$item_compass_token_gold"
                    or "$item_compass_token_silver"
                    or "$item_compass_token_darkgold")) continue;
                SetCompassActive(item.m_shared.m_name);
                break;
            }
        }
        private static void SetCompassActive(string itemName)
        {
            switch (itemName)
            {
                case "$item_compass_token_brass":
                    compassCircle = true;
                    break;
                case "$item_compass_token_gold":
                    compassBarActive = true;
                    break;
                case "$item_compass_token_silver":
                    compassCircle = true;
                    biomesName = true;
                    windIcon = true;
                    circlePinActive = true;
                    break;
                case "$item_compass_token_darkgold":
                    compassBarActive = true;
                    barPinActive = true;
                    break;
            }
        }
        private static void UpdateCompassBar()
        {
            float cameraRotationY = playerRotation.eulerAngles.y; // Value between 0 and 360
            float totalWidth = HudPatch.compassIconCount * RustyCompassPlugin._CompassBarIconSpacing.Value;
            float distance = 360f;
            float positionY = RustyCompassPlugin._CompassBarPosition.Value;
            
            for (int i = 0; i < HudPatch.compassIconCount; ++i)
            {
                Transform iconElement = root.transform.Find($"barIcon ({i})");
                
                iconElement.TryGetComponent(out RectTransform iconRect);
                iconElement.TryGetComponent(out Image iconImage);

                if (!iconRect || !iconImage) return;

                float normalizedRotation = (cameraRotationY + i * (distance / HudPatch.compassIconCount)) % distance;
                float positionX = (normalizedRotation / distance) * totalWidth - totalWidth / 2;
                
                // Set values
                iconRect.sizeDelta = RustyCompassPlugin._CompassBarIconSize.Value;
                iconRect.anchoredPosition = new Vector2(positionX, positionY);
                iconImage.color = RustyCompassPlugin._CompassBarColor.Value;
                // Enable/Disable
                iconElement.gameObject.SetActive(
                    RustyCompassPlugin._isModActive.Value == RustyCompassPlugin.Toggle.On &&
                    (
                        (RustyCompassPlugin._useCompassTokens.Value == RustyCompassPlugin.Toggle.On && compassBarActive) ||
                        (RustyCompassPlugin._useCompassTokens.Value != RustyCompassPlugin.Toggle.On &&
                         RustyCompassPlugin._CompassType.Value == RustyCompassPlugin.CompassType.Bar)
                    )
                );
            }
        }

        private static void UpdatePins()
        {
            if (Camera.main == null || Player.m_localPlayer == null) return;
            
            Transform compassContainer = root.transform.Find("CompassContainer");
            Transform barCompassIcon0 = root.transform.Find($"barIcon (0)");
            List<PinData> latestPinData = GetLatestPinData();
            
            // Delete all pins
            foreach (PinData barPin in BarPins) UnityEngine.Object.Destroy(barPin.m_uiElement);
            foreach (PinData circlePin in CirclePins) UnityEngine.Object.Destroy(circlePin.m_uiElement);
            // Clear pin lists
            BarPins.Clear();
            CirclePins.Clear();
            
            // General Settings
            float maxDistance = RustyCompassPlugin._CompassPinsMaxDistance.Value;
            
            if (compassContainer.gameObject.activeInHierarchy)
            {
                if (RustyCompassPlugin._CompassPinsEnabled.Value == RustyCompassPlugin.Toggle.Off) return;
                // Create new circle pins
                foreach (PinData pinData in latestPinData)
                {
                    GameObject newCirclePin = CreateCirclePin(pinData);
                    pinData.m_uiElement = newCirclePin;
                    CirclePins.Add(pinData);
                }
                // Iterate through circle pins and set data
                foreach (PinData pinData in CirclePins)
                {
                    Vector3 pinPos = pinData.m_pos;
                    Vector3 distanceFromPlayer = pinPos - playerPosition;
                    float totalDistance = (Vector3.Distance(playerPosition, pinPos));
                    float maxSize = RustyCompassPlugin._CirclePinsMaxSize.Value;
                    
                    string name = pinData.m_name == "" ? pinData.m_icon.name : pinData.m_name;
                    name = name switch
                    {
                        "mapicon_trader" => "$rusty_compass_haldor",
                        "mapicon_start" => "$rusty_compass_spawn",
                        "mapicon_hildir" => "$rusty_compass_hildir",
                        _ => name
                    };
                    string localizedName = Localization.instance.Localize(name);

                    GameObject pin = pinData.m_uiElement;
                    Transform pinText = pin.transform.Find("text");
                    
                    pin.TryGetComponent(out RectTransform rect);
                    pin.TryGetComponent(out Image pinImage);
                    pinText.TryGetComponent(out TextMeshProUGUI textMesh);
                    pinText.TryGetComponent(out RectTransform textRect);
                    if (!rect || !pinImage || !textMesh || !textRect) return;
                    
                    // Set values
                    rect.anchoredPosition = new Vector2(distanceFromPlayer.x, distanceFromPlayer.z);
                    pinImage.color = RustyCompassPlugin._CirclePinsColor.Value;
                    textMesh.text = $"{localizedName} (<color=orange>{Mathf.Round(totalDistance)}</color>)";
                    textMesh.color = RustyCompassPlugin._CirclePinsColor.Value;
                    
                    // Set size of pin
                    float percentage = maxSize / totalDistance;
                    float size = maxSize * percentage;
                    size = Mathf.Clamp(size, 5f, maxSize);
                    
                    rect.sizeDelta = new Vector2(size, size);
                    textRect.sizeDelta = new Vector2(100f * percentage, 25f * percentage);
                    
                    // Use Token feature ??
                    pin.SetActive(
                        totalDistance < maxDistance &&
                        RustyCompassPlugin._CompassPinsEnabled.Value == RustyCompassPlugin.Toggle.On &&
                        RustyCompassPlugin._isModActive.Value == RustyCompassPlugin.Toggle.On &&
                        (
                            (RustyCompassPlugin._useCompassTokens.Value == RustyCompassPlugin.Toggle.On && 
                             circlePinActive) 
                            ||
                            (RustyCompassPlugin._useCompassTokens.Value != RustyCompassPlugin.Toggle.On && 
                             RustyCompassPlugin._CompassType.Value == RustyCompassPlugin.CompassType.Circle)
                        )
                    );
                }
            }

            if (barCompassIcon0.gameObject.activeInHierarchy)
            {
                if (RustyCompassPlugin._CompassPinsEnabled.Value == RustyCompassPlugin.Toggle.Off) return;
                // Create new bar pins
                foreach (var pinData in latestPinData)
                {
                    GameObject newBarPin = CreateBarPin(pinData);
                    pinData.m_uiElement = newBarPin;
                    // Register new bar pin with ui element
                    BarPins.Add(pinData);
                }
                
                float totalWidth = HudPatch.compassIconCount * RustyCompassPlugin._CompassBarIconSpacing.Value;
                float distance = 360f;
                float positionY = RustyCompassPlugin._CompassBarPosition.Value;
                float maxSize = RustyCompassPlugin._CompassPinsMaxSize.Value;
                // Iterate through bar pins and set data
                foreach (PinData pinData in BarPins)
                {
                    Vector3 pinPos = pinData.m_pos;
                    Vector3 distanceFromPlayer = pinPos - playerPosition;
                    float totalDistance = (Vector3.Distance(playerPosition, pinPos));
                    float angle = Vector3.SignedAngle(
                        playerRotation * Vector3.forward,
                        distanceFromPlayer,
                        Vector3.up
                    );
                    float positionX = ((angle % distance) / distance) * -totalWidth;

                    string name = pinData.m_name == "" ? pinData.m_icon.name : pinData.m_name;
                    name = name switch
                    {
                        "mapicon_trader" => "$rusty_compass_haldor",
                        "mapicon_start" => "$rusty_compass_spawn",
                        "mapicon_hildir" => "$rusty_compass_hildir",
                        _ => name
                    };
                    string localizedName = Localization.instance.Localize(name);
                    
                    GameObject pin = pinData.m_uiElement;
                    Transform pinText = pin.transform.Find("text");

                    pin.TryGetComponent(out RectTransform rect);
                    pin.TryGetComponent(out Image pinImage);
                    pinText.TryGetComponent(out TextMeshProUGUI textMesh);
                    pinText.TryGetComponent(out RectTransform textRect);
                    if (!rect || !pinImage || !textMesh || !textRect) return;
                    
                    // Set values
                    rect.anchoredPosition = new Vector2(positionX, positionY);
                    pinImage.color = RustyCompassPlugin._CompassPinsColor.Value;
                    textMesh.text = $"{localizedName} (<color=orange>{Mathf.Round(totalDistance)}</color>)";
                    textMesh.color = RustyCompassPlugin._CompassPinsColor.Value;
                    
                    // Set size of pin
                    float percentage = maxSize / totalDistance;
                    float size = maxSize * percentage;
                    size = Mathf.Clamp(size, 5f, maxSize);
                    
                    rect.sizeDelta = new Vector2(size, size);
                    textRect.sizeDelta = new Vector2(100f * percentage, 25f * percentage);
                    
                    // Use Token feature ??
                    pin.SetActive(
                        totalDistance < maxDistance &&
                        RustyCompassPlugin._CompassPinsEnabled.Value == RustyCompassPlugin.Toggle.On &&
                        RustyCompassPlugin._isModActive.Value == RustyCompassPlugin.Toggle.On &&
                        (
                            (RustyCompassPlugin._useCompassTokens.Value == RustyCompassPlugin.Toggle.On && barPinActive) ||
                            (RustyCompassPlugin._useCompassTokens.Value != RustyCompassPlugin.Toggle.On &&
                             RustyCompassPlugin._CompassType.Value == RustyCompassPlugin.CompassType.Bar)
                        )
                    );
                }
            }
        }

        private static GameObject CreateBarPin(PinData pinData)
        {
            string name = pinData.m_name == "" ? pinData.m_icon.name : pinData.m_name;
            Vector3 pos = pinData.m_pos;
            GameObject newPin = new GameObject($"BarPin ({pos.x}{pos.y}{pos.z})");
            RectTransform rect = newPin.AddComponent<RectTransform>();
            rect.SetParent(root.transform);
            rect.anchoredPosition = new Vector2(0f, 0f);
            rect.sizeDelta = new Vector2(50f, 50f);

            Image image = newPin.AddComponent<Image>();
            image.sprite = pinData.m_type switch
            {
                Minimap.PinType.Boss => HudPatch.bossIcon,
                Minimap.PinType.Player => HudPatch.vikingIcon,
                Minimap.PinType.Death => HudPatch.deathIcon,
                Minimap.PinType.Bed => HudPatch.bedIcon,
                Minimap.PinType.Icon0 => HudPatch.fireIcon,
                Minimap.PinType.Icon1 => HudPatch.houseIcon,
                Minimap.PinType.Icon2 => HudPatch.anchorIcon,
                Minimap.PinType.Icon3 => HudPatch.circleIcon,
                Minimap.PinType.Icon4 => HudPatch.portalIcon,
                Minimap.PinType.Hildir1 => HudPatch.questionMarkIcon,
                Minimap.PinType.Hildir2 => HudPatch.questionMarkIcon,
                Minimap.PinType.Hildir3 => HudPatch.questionMarkIcon,
                Minimap.PinType.Ping => HudPatch.blueTargetIcon,
                Minimap.PinType.Shout => HudPatch.yellowTargetIcon,
                Minimap.PinType.EventArea => HudPatch.exclamationIcon,
                Minimap.PinType.RandomEvent => HudPatch.exclamationIcon,
                _ => HudPatch.circleIcon
            };

            image.sprite = pinData.m_icon.name switch
            {
                "mapicon_start" => HudPatch.spawnIcon,
                "mapicon_trader" => HudPatch.merchantIcon,
                "mapicon_hildir" => HudPatch.clothIcon,
                _ => image.sprite
            };
            image.color = Color.white;

            GameObject pinText = new GameObject("text");
            RectTransform textRect = pinText.AddComponent<RectTransform>();
            textRect.SetParent(newPin.transform);
            textRect.anchoredPosition = new Vector2(0f, -35f);
            textRect.sizeDelta = new Vector2(100f, 25f);

            TextMeshProUGUI textMesh = pinText.AddComponent<TextMeshProUGUI>();
            textMesh.font = HudPatch.font;
            textMesh.fontSize = 15f;
            textMesh.fontSizeMax = 15f;
            textMesh.fontSizeMin = 5f;
            textMesh.enableAutoSizing = true;
            textMesh.color = Color.white;
            textMesh.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textMesh.verticalAlignment = VerticalAlignmentOptions.Middle;
            textMesh.richText = true;
            
            textMesh.text = Localization.instance.Localize(name);
            
            newPin.SetActive(false);

            return newPin;
        }
        
        private static GameObject CreateCirclePin(PinData pinData)
        {
            Transform compassContainer = root.transform.Find("CompassContainer");
            Transform background = compassContainer.Find("background");
            
            string name = pinData.m_name == "" ? pinData.m_icon.name : pinData.m_name;
            Vector3 pos = pinData.m_pos;
            
            GameObject newPin = new GameObject($"CirclePin ({pos.x}{pos.y}{pos.z})");
            RectTransform rect = newPin.AddComponent<RectTransform>();
            rect.SetParent(background);
            rect.anchoredPosition = new Vector2(0f, 0f);
            rect.sizeDelta = new Vector2(50f, 50f);

            Image image = newPin.AddComponent<Image>();
            image.sprite = pinData.m_type switch
            {
                Minimap.PinType.Boss => HudPatch.bossIcon,
                Minimap.PinType.Player => HudPatch.vikingIcon,
                Minimap.PinType.Death => HudPatch.deathIcon,
                Minimap.PinType.Bed => HudPatch.bedIcon,
                Minimap.PinType.Icon0 => HudPatch.fireIcon,
                Minimap.PinType.Icon1 => HudPatch.houseIcon,
                Minimap.PinType.Icon2 => HudPatch.anchorIcon,
                Minimap.PinType.Icon3 => HudPatch.circleIcon,
                Minimap.PinType.Icon4 => HudPatch.portalIcon,
                Minimap.PinType.Hildir1 => HudPatch.questionMarkIcon,
                Minimap.PinType.Hildir2 => HudPatch.questionMarkIcon,
                Minimap.PinType.Hildir3 => HudPatch.questionMarkIcon,
                Minimap.PinType.Ping => HudPatch.blueTargetIcon,
                Minimap.PinType.Shout => HudPatch.yellowTargetIcon,
                Minimap.PinType.EventArea => HudPatch.exclamationIcon,
                Minimap.PinType.RandomEvent => HudPatch.exclamationIcon,
                _ => HudPatch.circleIcon
            };

            image.sprite = pinData.m_icon.name switch
            {
                "mapicon_start" => HudPatch.spawnIcon,
                "mapicon_trader" => HudPatch.merchantIcon,
                "mapicon_hildir" => HudPatch.clothIcon,
                _ => image.sprite
            };
            image.color = Color.white;

            GameObject pinText = new GameObject("text");
            RectTransform textRect = pinText.AddComponent<RectTransform>();
            textRect.SetParent(newPin.transform);
            textRect.anchoredPosition = new Vector2(0f, -25f);
            textRect.sizeDelta = new Vector2(100f, 25f);

            TextMeshProUGUI textMesh = pinText.AddComponent<TextMeshProUGUI>();
            textMesh.font = HudPatch.font;
            textMesh.fontSize = 15f;
            textMesh.fontSizeMax = 15f;
            textMesh.fontSizeMin = 5f;
            textMesh.enableAutoSizing = true;
            textMesh.color = Color.white;
            textMesh.horizontalAlignment = HorizontalAlignmentOptions.Center;
            textMesh.verticalAlignment = VerticalAlignmentOptions.Middle;
            textMesh.richText = true;
            
            textMesh.text = Localization.instance.Localize(name);
            
            newPin.SetActive(false);

            return newPin;
        }
        
        private static List<PinData> GetLatestPinData()
        {
            List<Minimap.PinData> minimapPins = new List<Minimap.PinData>();
            // custom player pins
            List<ZNet.PlayerInfo> playerInfo = Minimap.instance.m_tempPlayerInfo; 
            foreach (ZNet.PlayerInfo info in playerInfo)
            {
                Minimap.PinData playerPinData = new Minimap.PinData()
                {
                    m_name = info.m_name,
                    m_type = Minimap.PinType.Player,
                    m_icon = HudPatch.vikingIcon,
                    m_save = false,
                    m_checked = false,
                    m_pos = info.m_position,
                    m_ownerID = 0L,
                };
                minimapPins.Add(playerPinData);
            }
            // possibly all pins ?? definitely tame pins 
            List<Minimap.PinData> pins = Minimap.instance.m_pins;
            foreach (Minimap.PinData info in pins)
            {
                Minimap.PinType type = info.m_type;
                if (type
                    is Minimap.PinType.EventArea
                    or Minimap.PinType.RandomEvent
                   ) continue;
                minimapPins.Add(info);
            }
            // locations ex: haldor
            Dictionary<Vector3, Minimap.PinData> locationPins = Minimap.instance.m_locationPins; 
            List<KeyValuePair<Vector3,Minimap.PinData>> locationPinsList = locationPins.ToList();
            foreach (KeyValuePair<Vector3, Minimap.PinData> locationPin in locationPinsList)
            {
                Minimap.PinData info = locationPin.Value;
                minimapPins.Add(info);
            }
            // Possible players ??
            List<Minimap.PinData> playerPins = Minimap.instance.m_playerPins;
            foreach (Minimap.PinData info in playerPins)
            {
                Minimap.PinType type = info.m_type;
                if (type is Minimap.PinType.None
                    or Minimap.PinType.EventArea
                    or Minimap.PinType.RandomEvent
                   ) continue;
                minimapPins.Add(info);
            }
            
            // Validate pin data
            List<PinData> outputList = new List<PinData>();
            foreach (var pin in minimapPins)
            {
                PinData newPinData = new PinData()
                {
                    m_name = pin.m_name,
                    m_type = pin.m_type,
                    m_pos = pin.m_pos,
                    m_icon = pin.m_icon
                };
                if (string.IsNullOrEmpty(pin.m_name) 
                    && pin.m_icon.name 
                        is "mapicon_start" 
                        or "mapicon_trader" 
                        or "mapicon_hildir"
                    )
                {
                    newPinData.m_name = pin.m_icon.name;
                    outputList.Add(newPinData);
                }
                else if (!string.IsNullOrEmpty(pin.m_name))
                {
                    outputList.Add(newPinData);
                }
            }

            return outputList;
        }
        private class PinData
        {
            public string m_name = null!;
            public Sprite m_icon = null!;
            public GameObject m_uiElement = null!;
            public Minimap.PinType m_type;
            public Vector3 m_pos;
        }
        private static void UpdateCompassCircle()
        {
            Transform compassContainer = root.transform.Find("CompassContainer");
            Transform background = compassContainer.Find("background");
            Transform hand = compassContainer.Find("hand");
            Transform biomes = compassContainer.transform.Find("biomes");
            Transform windMarker = compassContainer.Find("windMarker");
            
            compassContainer.TryGetComponent(out RectTransform compassRect);
            background.TryGetComponent(out RectTransform backgroundRect);
            background.TryGetComponent(out Image backgroundImage);
            hand.TryGetComponent(out Image handImage);
            biomes.TryGetComponent(out TextMeshProUGUI biomesText);
            windMarker.TryGetComponent(out Image windMarkerImage);

            if (!compassRect || !backgroundRect || !backgroundImage || !handImage || !biomesText ||
                !windMarkerImage) return;
            
            // Set container position
            compassRect.anchoredPosition = RustyCompassPlugin._CompassPosition.Value;
            // Set compass settings
            backgroundRect.sizeDelta = RustyCompassPlugin._CompassSize.Value;
            backgroundImage.color = RustyCompassPlugin._CompassColor.Value;
            backgroundImage.sprite = RustyCompassPlugin._CompassSprite.Value switch
            {
                RustyCompassPlugin.Compass.Compass1 => RustyCompassPlugin.CompassSprite,
                RustyCompassPlugin.Compass.Compass2 => RustyCompassPlugin.CompassSimple,
                RustyCompassPlugin.Compass.Compass3 => RustyCompassPlugin.CompassBW,
                RustyCompassPlugin.Compass.Compass4 => RustyCompassPlugin.CompassViking,
                _ => backgroundImage.sprite
            };
            // Set compass rotation based on camera rotation
            background.rotation = Quaternion.Euler(0.0f, 0.0f, playerRotation.eulerAngles.y);
            // Set hand color
            handImage.color = RustyCompassPlugin._HandColor.Value;
            // Set biomes settings
            if (Player.m_localPlayer)
            {
                var currentBiomes = Player.m_localPlayer.m_currentBiome;
                biomesText.text = currentBiomes.ToString();
                biomesText.color = RustyCompassPlugin._BiomesColor.Value;
                if (RustyCompassPlugin._useCompassTokens.Value == RustyCompassPlugin.Toggle.On)
                {
                    biomesText.gameObject.SetActive(biomesName);
                }
            }
            // Set wind marker settings
            windMarkerImage.color = RustyCompassPlugin._WindMarkerColor.Value;
            windMarker.rotation = Quaternion.Euler(
                0.0f, 0.0f,
                -Quaternion.LookRotation(EnvMan.instance.GetWindDir()).eulerAngles.y
                );
            if (RustyCompassPlugin._useCompassTokens.Value == RustyCompassPlugin.Toggle.On)
            {
                windMarker.gameObject.SetActive(windIcon);
            }
            // Enable/Disable
            compassContainer.gameObject.SetActive(
                RustyCompassPlugin._isModActive.Value == RustyCompassPlugin.Toggle.On &&
                (
                    (RustyCompassPlugin._useCompassTokens.Value == RustyCompassPlugin.Toggle.On && compassCircle) ||
                    (RustyCompassPlugin._useCompassTokens.Value != RustyCompassPlugin.Toggle.On &&
                     RustyCompassPlugin._CompassType.Value == RustyCompassPlugin.CompassType.Circle)
                )
            );
        }
    }
}