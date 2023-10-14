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
    static class HudPatch
    {
        public static TMP_FontAsset font = null!;
        private static GameObject root = null!;
        public static int compassIconCount;
        public static Sprite fireIcon = null!;
        public static Sprite shipIcon = null!;
        public static Sprite playerIcon = null!;
        public static Sprite spawnIcon = null!;
        public static Sprite merchantIcon = null!;
        public static Sprite clothIcon = null!;
        public static Sprite houseIcon = null!;
        public static Sprite anchorIcon = null!;
        public static Sprite circleIcon = null!;
        public static Sprite portalIcon = null!;
        public static Sprite deathIcon = null!;
        public static Sprite bedIcon = null!;
        public static Sprite yellowTargetIcon = null!;
        public static Sprite bossIcon = null!;
        public static Sprite vikingIcon = null!;
        public static Sprite exclamationIcon = null!;
        public static Sprite redRadiusIcon = null!;
        public static Sprite blueTargetIcon = null!;
        public static Sprite questionMarkIcon = null!;
        private static void Postfix(Hud __instance)
        {
            root = __instance.m_rootObject;
            font = root.transform.Find("staminapanel").Find("Stamina").Find("StaminaText")
                .GetComponent<TextMeshProUGUI>().font;
            
            fireIcon = Minimap.instance.m_icons[0].m_icon;
            houseIcon = Minimap.instance.m_icons[1].m_icon;

            anchorIcon = Minimap.instance.m_icons[2].m_icon;
            circleIcon = Minimap.instance.m_icons[3].m_icon;
            portalIcon = Minimap.instance.m_icons[4].m_icon;
            deathIcon = Minimap.instance.m_icons[5].m_icon;
            bedIcon = Minimap.instance.m_icons[6].m_icon;
            yellowTargetIcon = Minimap.instance.m_icons[7].m_icon;
            bossIcon = Minimap.instance.m_icons[8].m_icon;
            vikingIcon = Minimap.instance.m_icons[9].m_icon;
            exclamationIcon = Minimap.instance.m_icons[10].m_icon;
            redRadiusIcon = Minimap.instance.m_icons[11].m_icon;
            blueTargetIcon = Minimap.instance.m_icons[12].m_icon;
            questionMarkIcon = Minimap.instance.m_icons[13].m_icon;

            shipIcon = Minimap.instance.m_largeShipMarker.GetComponent<Image>().sprite;
            playerIcon = Minimap.instance.m_largeMarker.GetComponent<Image>().sprite;
            spawnIcon = Minimap.instance.m_locationIcons[0].m_icon;
            merchantIcon = Minimap.instance.m_locationIcons[1].m_icon;
            clothIcon = Minimap.instance.m_locationIcons[2].m_icon;

            CreateCompassCircle();
            CreateCompassBar();
        }
        
        private static void CreateCompassBar()
        {
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
        private static List<Minimap.PinData> BarPins = new();
        
        private static bool compassCircle;
        private static bool biomesName;
        private static bool windIcon;

        private static bool compassBarActive;
        private static bool barPinActive;

        static void Postfix(Hud __instance)
        {
            root = __instance.m_rootObject;
            playerRotation = Utils.GetMainCamera().transform.rotation;
            playerPosition = Utils.GetMainCamera().transform.position;
            
            UpdateCompassCircle();
            UpdateCompassBar();
            UpdateBarPins();
            if (RustyCompassPlugin._useCompassTokens.Value == RustyCompassPlugin.Toggle.On) CheckPlayerInventory();
        }

        private static void CheckPlayerInventory()
        {
            if (Player.m_localPlayer == null) return;
            List<ItemDrop.ItemData> equippedItems = Player.m_localPlayer.GetInventory().GetEquippedItems();
            foreach (var item in equippedItems)
            {
                if (item.m_shared.m_name
                    is "$item_compass_token_brass"
                    or "$item_compass_token_gold"
                    or "$item_compass_token_silver"
                    or "$item_compass_token_darkgold")
                {
                    SetCompassActive(item.m_shared.m_name);
                    break;
                }
                compassBarActive = false;
                compassCircle = false;
                barPinActive = false;
                biomesName = false;
                windIcon = false;
            }
        }
        private static void SetCompassActive(string itemName)
        {
            compassBarActive = false;
            compassCircle = false;
            barPinActive = false;
            biomesName = false;
            windIcon = false;
            
            if (itemName == "$item_compass_token_brass")
            {
                compassCircle = true;
            }

            if (itemName == "$item_compass_token_gold")
            {
                compassBarActive = true;
            }

            if (itemName == "$item_compass_token_silver")
            {
                compassCircle = true;
                biomesName = true;
                windIcon = true;
            }

            if (itemName == "$item_compass_token_darkgold")
            {
                compassBarActive = true;
                barPinActive = true;
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
                RectTransform iconRect = iconElement.GetComponent<RectTransform>();
                Image iconImage = iconElement.GetComponent<Image>();

                float normalizedRotation = (cameraRotationY + i * (distance / HudPatch.compassIconCount)) % distance;
                float positionX = (normalizedRotation / distance) * totalWidth - totalWidth / 2;
                
                // Set values
                iconRect.sizeDelta = RustyCompassPlugin._CompassBarIconSize.Value;
                iconRect.anchoredPosition = new Vector2(positionX, positionY);
                iconImage.color = RustyCompassPlugin._CompassBarColor.Value;
                // Enable/Disable
                if (RustyCompassPlugin._useCompassTokens.Value == RustyCompassPlugin.Toggle.On)
                {
                    iconElement.gameObject.SetActive(
                        RustyCompassPlugin._isModActive.Value == RustyCompassPlugin.Toggle.On
                        && compassBarActive
                    );
                }
                else
                {
                    iconElement.gameObject.SetActive(
                        RustyCompassPlugin._isModActive.Value == RustyCompassPlugin.Toggle.On 
                        && RustyCompassPlugin._CompassType.Value == RustyCompassPlugin.CompassType.Bar
                        );
                }
            }
        }

        private static void UpdateBarPins()
        {
            if (Camera.main == null || Player.m_localPlayer == null) return;
            
            List<Minimap.PinData>? tempBarPins = UpdateBarTempPins();

            if (tempBarPins == null) return;

            try
            {
                float totalWidth = HudPatch.compassIconCount * RustyCompassPlugin._CompassBarIconSpacing.Value;
                float distance = 360f;
                float positionY = RustyCompassPlugin._CompassBarPosition.Value;
                float maxDistance = RustyCompassPlugin._CompassPinsMaxDistance.Value;
                float maxSize = RustyCompassPlugin._CompassPinsMaxSize.Value;

                foreach (Minimap.PinData tempPins in tempBarPins)
                {
                    Minimap.PinData pinData = tempPins;
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

                    GameObject pin = BarPins.Contains(pinData) ? root.transform.Find($"BarPin ({pinPos.x}{pinPos.y}{pinPos.z})").gameObject : CreateBarPin(pinData);
                    
                    RectTransform rect = pin.GetComponent<RectTransform>();
                    Image pinImage = pin.GetComponent<Image>();
                    TextMeshProUGUI pinText = pin.transform.Find("text").GetComponent<TextMeshProUGUI>();
                    RectTransform textRect = pin.transform.Find("text").GetComponent<RectTransform>();

                    // Set values
                    rect.anchoredPosition = new Vector2(positionX, positionY);
                    pinImage.color = RustyCompassPlugin._CompassPinsColor.Value;
                    name = name switch
                    {
                        "mapicon_trader" => "$rusty_compass_haldor",
                        "mapicon_start" => "$rusty_compass_spawn",
                        "mapicon_hildir" => "$rusty_compass_hildr",
                        _ => name
                    };
                    pinText.text = Localization.instance.Localize(name);

                    if (RustyCompassPlugin._useCompassTokens.Value == RustyCompassPlugin.Toggle.On)
                    {
                        pin.SetActive(
                            totalDistance < maxDistance
                            &&
                            RustyCompassPlugin._isModActive.Value == RustyCompassPlugin.Toggle.On
                            && 
                            barPinActive
                        );
                    }
                    else
                    {
                        pin.SetActive(
                            totalDistance < maxDistance 
                            && 
                            RustyCompassPlugin._CompassPinsEnabled.Value == RustyCompassPlugin.Toggle.On
                            &&
                            RustyCompassPlugin._CompassType.Value == RustyCompassPlugin.CompassType.Bar
                            &&
                            RustyCompassPlugin._isModActive.Value == RustyCompassPlugin.Toggle.On
                        );
                    }
                    
                    // Set size of pin
                    float percentage = maxSize / totalDistance;
                    float size = maxSize * percentage;
                    size = Mathf.Clamp(size, 5f, 50f);
                    
                    rect.sizeDelta = new Vector2(size, size);
                    textRect.sizeDelta = new Vector2(100f * percentage, 25f * percentage);
                }
                // Find differences between temp pins and loaded pins
                List<Minimap.PinData> LoadedBarPins = new List<Minimap.PinData>(BarPins);
                IEnumerable<Minimap.PinData> differences = BarPins.Except(tempBarPins);
                foreach (Minimap.PinData difference in differences)
                {
                    Vector3 pos = difference.m_pos;
                    GameObject? differentPin = root.transform.Find($"BarPin ({pos.x}{pos.y}{pos.z})").gameObject;
                    if (differentPin == null) continue;
                    UnityEngine.Object.Destroy(differentPin);
                    LoadedBarPins.Remove(difference);
                }

                BarPins = LoadedBarPins;
                
            } catch (NullReferenceException) {}
        }

        private static GameObject CreateBarPin(Minimap.PinData pinData)
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

            BarPins.Add(pinData);
            newPin.SetActive(false);

            return newPin;
        }

        private static List<Minimap.PinData>? UpdateBarTempPins()
        {
            List<Minimap.PinData> tempBarPins = new List<Minimap.PinData>();
            // custom player pins
            List<ZNet.PlayerInfo> playerInfo = Minimap.instance.m_tempPlayerInfo; 
            for (int i = 0; i < playerInfo.Count; ++i)
            {
                var info = playerInfo[i];
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
                tempBarPins.Add(playerPinData);
            }
            // possibly all pins ??
            List<Minimap.PinData> pins = Minimap.instance.m_pins;
            for (int i = 0; i < pins.Count; ++i)
            {
                var info = pins[i];
                var type = info.m_type;
                if (type
                    is Minimap.PinType.EventArea
                    or Minimap.PinType.Shout
                    or Minimap.PinType.RandomEvent
                    or Minimap.PinType.Ping
                    // or Minimap.PinType.Player
                   ) continue;
                tempBarPins.Add(info);
            }
            // locations ex: haldor
            Dictionary<Vector3, Minimap.PinData> locationPins = Minimap.instance.m_locationPins; 
            List<KeyValuePair<Vector3,Minimap.PinData>> locationPinsList = locationPins.ToList();
            for (int i = 0; i < locationPinsList.Count; ++i)
            {
                var info = locationPinsList[i].Value;
                tempBarPins.Add(info);
            }
            // Possible players ??
            List<Minimap.PinData> playerPins = Minimap.instance.m_playerPins;
            for (int i = 0; i < playerPins.Count; ++i)
            {
                var info = playerPins[i];
                var type = info.m_type;
                if (type is Minimap.PinType.None
                    or Minimap.PinType.Ping
                    or Minimap.PinType.Shout
                    or Minimap.PinType.EventArea
                    or Minimap.PinType.RandomEvent) continue;
                tempBarPins.Add(info);
            }
            // Validate pin data
            List<Minimap.PinData> outputList = new();

            foreach (var pin in tempBarPins)
            {
                if (string.IsNullOrEmpty(pin.m_name) && pin.m_icon.name is "mapicon_start" or "mapicon_trader" or "mapicon_hildir")
                {
                    outputList.Add(pin);
                }
                else if (!string.IsNullOrEmpty(pin.m_name))
                {
                    outputList.Add(pin);
                }
            }

            // if (outputList.Count == 0)
            // {
            //     var all = ZNetView.FindObjectOfType<GameObject>();
            // }
            return outputList;
        }
        private static void UpdateCompassCircle()
        {
            var compassContainer = root.transform.Find("CompassContainer");
            var background = compassContainer.Find("background");
            var hand = compassContainer.Find("hand");
            var biomes = compassContainer.transform.Find("biomes");
            var windMarker = compassContainer.Find("windMarker");

            RectTransform compassRect = compassContainer.GetComponent<RectTransform>();
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            Image backgroundImage = background.GetComponent<Image>();
            Image handImage = hand.GetComponent<Image>();
            TextMeshProUGUI biomesText = biomes.GetComponent<TextMeshProUGUI>();
            Image windMarkerImage = windMarker.GetComponent<Image>();
            
            // Set container
            compassRect.anchoredPosition = RustyCompassPlugin._CompassPosition.Value;
            // Set compass background
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
            // Set hand
            handImage.color = RustyCompassPlugin._HandColor.Value;
            // Set biomes
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
            // Set wind marker
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
            if (RustyCompassPlugin._useCompassTokens.Value == RustyCompassPlugin.Toggle.On)
            {
                compassContainer.gameObject.SetActive(
                    RustyCompassPlugin._isModActive.Value == RustyCompassPlugin.Toggle.On
                    && compassCircle);
                
            }
            else
            {
                compassContainer.gameObject.SetActive(
                    RustyCompassPlugin._isModActive.Value == RustyCompassPlugin.Toggle.On 
                    && RustyCompassPlugin._CompassType.Value == RustyCompassPlugin.CompassType.Circle
                );
            }
            
        }
    }
}