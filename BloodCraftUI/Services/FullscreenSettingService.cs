using UnityEngine.UI;
using UnityEngine;
using ProjectM.UI;
using BloodCraftEZLife.Utils;
using ProjectM;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.ModContent;
using BloodCraftEZLife.Config;
using Unity.Entities;
using Unity.Mathematics;
using static Il2CppSystem.Data.Common.ObjectStorage;


namespace BloodCraftEZLife.Services
{

    internal static class FullscreenSettingService
    {
        /// <summary>
        /// Holds references to one template of each type of option in the panel.
        /// </summary>
        public class OptionTemplates
        {
            public SettingsEntry_Dropdown ComboBox;
            public SettingsEntry_Slider Slider;
            public SettingsEntry_Checkbox Toggle;
            public SettingsEntry_Label Header;
            public SettingsEntry_Button Button;
            public SettingsEntry_Selector Selector;
            public SettingsEntry_Binding Binding;
        }

        public static OptionTemplates _templates;
        

        
        public static Vector2 DeltaRect;
        public static Vector2 AnchorRect;
        public static Vector2 ResFactor;
        

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
        public static void ClonePanelAndGetTemplates(OptionsPanel_Controls originalPanel)
        {
            //UnityHelper.PrintChilds(originalPanel.transform,1);
            
            _templates = new OptionTemplates();
            
            _templates.Header = originalPanel.HeaderPrefab;

            _templates.ComboBox = originalPanel.DropdownPrefab;// transform.FindChild("CustomGameSettings_Dropdown(Clone)").gameObject;
            
            _templates.Toggle = originalPanel.CheckboxPrefab;
            _templates.Slider = originalPanel.SliderPrefab;
            _templates.Selector = originalPanel.SelectorPrefab;
            _templates.Button = originalPanel.ButtonPrefab;
            //Transform toptslider = originalPanel.transform.FindChild("Options_Control_Slider").gameObject;
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
                            
                            
                            if (healthData.IsDead && entity.TryGetComponent<Team>(out var teamid))
                            {
                                //Clan 2 point to vblood so far
                                //If bugged IsUnitTeam might work
                                if (teamid.Clan == 2)
                                {
                                    var vbloodString = comp.Source.GetLocalizedName();
                                    VbloodList.AddVbloodKill(vbloodString);
                                    SaveVbloodData();
                                }
                                
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
