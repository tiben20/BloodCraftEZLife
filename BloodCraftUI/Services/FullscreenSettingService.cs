
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using static ProjectM.UI.SimpleStunButton;
using ProjectM.UI;
using BloodCraftEZLife.Utils;
using MS.Internal.Xml.XPath;
using ProjectM;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.ModContent;
using BloodCraftEZLife.Config;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static RootMotion.FinalIK.AimPoser;

namespace BloodCraftEZLife.Services
{

    internal static class FullscreenSettingService
    {
        /// <summary>
        /// Holds references to one template of each type of option in the panel.
        /// </summary>
        public class OptionTemplates
        {
            public GameObject ComboBox;
            public GameObject Slider;
            public GameObject Toggle;
            public GameObject Header;
            public GameObject OptionsSlider;
        }

        public static OptionTemplates _templates;
        public static GameObject _newPanel;

        public static event System.Action OnHeaderButtonClicked;
        public static Vector2 DeltaRect;
        public static Vector2 AnchorRect;
        public static Rect PanelLayout;

        public static SettingsVblood VbloodList;
        internal static EntityQuery VBloodCarriers;

        private static void PrintRect(string rectname, RectTransform rect)
        {
            LogUtils.LogInfo(rectname+"anchors width:" + rect.rect.width.ToString() + " height:" + rect.rect.height.ToString());
            LogUtils.LogInfo(rectname + "position x:" + rect.anchoredPosition.x.ToString() + " y:" + rect.anchoredPosition.y.ToString());
        }

        //Unitialise vbloods
        public static void InitialiseVbloodData(string uniqueid)
        {
            
            // Load or create
            string thepath = System.IO.Path.Combine(Settings.CONFIG_PATH, uniqueid) + ".json";
            VbloodList = ConfigSaveManager.Load(thepath);


            UpdateVbloodQuery();

        }

        public static void UpdateVbloodQuery()
        {
            VBloodCarriers = Plugin.EntityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadOnly<VBloodConsumeSource>() },
                None = new[] { ComponentType.ReadOnly<PlayerCharacter>() }
            });

        }
        //Unitialise vbloods
        public static void SaveVbloodData()
        {
            // Save
            ConfigSaveManager.Save(VbloodList);
        }

        //Header clicked
        public static void RaiseHeaderButtonClicked(OptionsMenu __instance)
        {
            //PanelLayout = __instance.GeneralPanel.ContentNode.rect;
            //PrintRect("GeneralPanel", __instance.GeneralPanel.Rect);

            var panel = Plugin.UIManager.GetPanel<SettingsPanel>();
            if (panel != null)
            {
                panel.SetActive(true);
                return;

            }
            Plugin.UIManager.AddPanel(PanelType.SettingsPanel);
        }

        

        /// <summary>
        /// Clones a ScrollView panel, clears its content, and stores one of each type of option.
        /// </summary>
        /// <param name="originalPanel">Original panel (ScrollView) to copy.</param>
        /// <returns>A tuple with the cloned panel and its option templates.</returns>
        public static void ClonePanelAndGetTemplates(GameObject originalPanel)
        {
            PrintHierarchy(originalPanel.transform,1);

            _templates = new OptionTemplates();
            if (originalPanel == null)
            {
                Debug.LogError("Original panel is null!");
                return;
            }

            // Instantiate a copy of the scrollview
            _newPanel = Object.Instantiate(originalPanel, originalPanel.transform.parent);
            _newPanel.name = originalPanel.name + "_Clone";

            

            ScrollRect scrollRect = _newPanel.GetComponentInChildren<ScrollRect>();
            if (scrollRect == null)
            {
                Debug.LogError("Clone does not have a ScrollRect component!");
                return;
            }

            Transform content = scrollRect.content;
            if (content == null)
            {
                Debug.LogError("ScrollRect content is missing!");
                return;
            }

            OptionTemplates templates = new OptionTemplates();
            Transform theader = content.transform.FindChild("CustomGameSettings_Label(Clone)");
            if (theader != null)
            {
                templates.Header = theader.gameObject;
                templates.Header.name = "CustomGameSettings_Header_EZLife";
            }

            Transform tdropdown = content.transform.FindChild("CustomGameSettings_Dropdown(Clone)");
            if (tdropdown != null)
            {
                templates.ComboBox = tdropdown.gameObject;
                templates.ComboBox.name = "CustomGameSettings_ComboBox_EZLife";
            }

            Transform tcheckbox = content.transform.FindChild("CustomGameSettings_Checkbox(Clone)");
            if (tcheckbox != null)
            {
                templates.Toggle = tcheckbox.gameObject;
                templates.Toggle.name = "CustomGameSettings_Toggle_EZLife";
            }

            Transform tslider = content.transform.FindChild("CustomGameSettings_Slider(Clone)");
            if (theader != null)
            {
                templates.Slider = theader.gameObject;
                templates.Slider.name = "CustomGameSettings_Slider_EZLife";
                
            }

            Transform toptslider = content.transform.FindChild("Options_Control_Slider");
            if (toptslider != null)
            {
                templates.OptionsSlider = toptslider.gameObject;
                templates.OptionsSlider.name = "Options_Control_Slider_EZLife";
            }

            DestroyLoop(content.gameObject,"CustomGameSettings_Label(Clone)");
            DestroyLoop(content.gameObject, "CustomGameSettings_Checkbox(Clone)");
            DestroyLoop(content.gameObject, "CustomGameSettings_Slider(Clone)");
            DestroyLoop(content.gameObject, "CustomGameSettings_Dropdown(Clone)");
        }

        static public void DestroyLoop(GameObject go, string objname)
        {
            int destroyed = 1;
            for (; ; )
            {
                Transform tt = go.transform.FindChild(objname);
                if (tt != null)
                {
                    Object.Destroy(tt.gameObject);
                    Debug.LogError(destroyed.ToString()+" destroyed");
                }
                else
                    return;
                destroyed++;
            }
        }

        static public void PrintHierarchy(Transform t, int depth)
        {
            // Prefix with dashes based on depth
            string prefix = new string('-', depth);

            // Print GameObject name
            Debug.Log(prefix + t.gameObject.name);

            
            for (int i = 0; i < t.childCount; i++)
            {
                Transform child = t.GetChild(i);
                PrintHierarchy(child, depth + 1);
            }
        }

        

        static public void CheckVblood()
        {
            if (!Settings.KeepTrackOfVbloodKills)
                return;
            var entities = VBloodCarriers.ToEntityArray(Unity.Collections.Allocator.Temp);
            try
            {
                foreach (var entity in entities)
                {
                    if (entity.TryGetComponent<VBloodConsumeSource>(out var comp))
                    {
                        //ProjectM.Health
                        float3 vbloodpos = entity.GetPosition();
                        float3 pos2 = Plugin.LocalCharacter.GetPosition();
                        float dist = math.distance(vbloodpos, pos2);
                        if (dist < 10.0f && entity.TryGetComponent < Health>(out var healthData) && entity.TryGetComponent<BuffBuffer>(out var buff))
                        {
                            if (healthData.IsDead)
                            {
                                var vbloodString = comp.Source.GetLocalizedName();
                                VbloodList.AddVbloodKill(vbloodString);
                                SaveVbloodData();
                            }
                        }
                        
                    }
                }
            }
            finally
            {
                if (entities.IsCreated) entities.Dispose();
            }
        }

    
    }
}
