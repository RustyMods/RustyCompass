using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RustyCompass.Compass;

public static class CompassUI
{
    // public static GameObject root = null!;
    // public static TMP_FontAsset font = null!;
    // public static int compassIconCount;
    
    public static class RustyCompass
    {
        public static void CreateCompassCircle(GameObject root, TMP_FontAsset font)
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
        
        public static void CreateCompassBar(GameObject root)
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
            int compassIconCount = compassIcons.Count;
            
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
    }
}