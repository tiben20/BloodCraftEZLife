using System.Collections.Generic;
using System.Timers;
using BloodCraftUI.Config;
using BloodCraftUI.NewUI.UICore.UniverseLib.UI;
using BloodCraftUI.NewUI.UICore.UniverseLib.UI.Models;
using BloodCraftUI.NewUI.UICore.UniverseLib.UI.Widgets.ScrollView;
using BloodCraftUI.Services;
using UnityEngine;
using UnityEngine.UI;
using static BloodCraftUI.Patches.ClientChatPatch;

namespace BloodCraftUI._TRASH.Panels
{
    /* internal class BoxContentPanel : UEPanel
     {
         public override string Name { get; }
         public override PanelType PanelType => PanelType.BoxContent;
         public override int MinWidth => 250;
         public override int MinHeight => 100;
         public override Vector2 DefaultAnchorMin => new(0.4f, 0.4f);
         public override Vector2 DefaultAnchorMax => new(0.6f, 0.6f);
         public override bool NavButtonWanted => false;
         public override bool ShouldSaveActiveState => false;
         public override bool ShowByDefault => true;
         protected override string Id { get; }

         private readonly string _boxName;
         private bool _isInitialized;
         private ButtonRef _updateButton;

         public BoxContentPanel(UIBase owner, string name) : base(owner)
         {
             Name = $"Box '{name}' content";
             TitleBar.GetComponentInChildren<Text>().text = Name;
             _boxName = Id = name;
             SendUpdateCommand();
         }

         public override void SetActive(bool active)
         {
             var shouldUpdateData = _isInitialized && active && this.Enabled == false;
             _isInitialized = true;
             base.SetActive(active);
             if (shouldUpdateData)
                 SendUpdateCommand();
         }

         #region Commands
         private void SendUpdateCommand()
         {
             EnableAllButtons(false);
             if (string.IsNullOrEmpty(_boxName))
                 return;
             MessageService.QueueMessage(string.Format(Settings.BCCOM_SWITCHBOX, _boxName));
             var t = new Timer(1000);
             t.AutoReset = false;
             t.Elapsed += (_, _) =>
             {
                 try
                 {
                     MessageService.QueueMessage(Settings.BCCOM_BOXCONTENT);
                     t.Dispose();
                 }
                 finally
                 {
                     EnableAllButtons(true);
                 }
             };
             t.Start();
         }

         private void SendBindCommand(int number)
         {
             EnableAllButtons(false);
             MessageService.QueueMessage(Settings.BCCOM_UNBINDFAM);
             var t = new Timer(3000);
             t.AutoReset = false;
             t.Elapsed += (_, _) =>
             {
                 try
                 {
                     MessageService.QueueMessage(string.Format(Settings.BCCOM_BINDFAM, number));
                     t.Dispose();
                 }
                 finally
                 {
                     EnableAllButtons(true);

                 }
             };
             t.Start();
         }
         #endregion

         public void ClearList()
         {
             _dataList.Clear();
         }

         public void AddList(int number, string name, ColorNameData cnData)
         {
             _dataList.Add(new FamData { Number = number, Name = name, Data = cnData });
             _scrollDataHandler.RefreshData();
             _scrollPool.Refresh(true);
         }

         protected override void ConstructPanelContent()
         {
             var horGroup = UIFactory.CreateHorizontalGroup(ContentRoot, "ButtonGroup", true, false, true, false, 3,
                 default, new Color(1, 1, 1, 0), TextAnchor.MiddleLeft);
             _updateButton = UIFactory.CreateButton(horGroup, "butPopulate", "Populate Box Content");
             UIFactory.SetLayoutElement(_updateButton.GameObject, minWidth: 250, minHeight: 25, flexibleWidth: 9999);
             _updateButton.OnClick += SendUpdateCommand;

             var l = UIFactory.CreateLabel(ContentRoot, "Header", "", TextAnchor.UpperCenter);
             UIFactory.SetLayoutElement(l.gameObject, minWidth: 250, minHeight: 25, flexibleWidth: 0);

             _scrollDataHandler = new ButtonListHandler<FamData, ButtonCell>(_scrollPool, GetEntries, SetCell, ShouldDisplay, OnCellClicked);
             _scrollPool = UIFactory.CreateScrollPool<ButtonCell>(ContentRoot, "ContentList", out GameObject scrollObj,
                 out GameObject scrollContent, new Color(0.03f, 0.03f, 0.03f));
             _scrollPool.Initialize(_scrollDataHandler);
             UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);
         }

         private void EnableAllButtons(bool value)
         {
             _updateButton.Component.interactable = value;
             foreach (var a in _scrollPool.CellPool)
                 a.Button.Component.interactable = value;
         }

         #region ScrollPool handling

         private static ScrollPool<ButtonCell> _scrollPool;
         private static ButtonListHandler<FamData, ButtonCell> _scrollDataHandler;

         private List<FamData> GetEntries() => _dataList;

         private bool ShouldDisplay(FamData data, string filter) => true;

         private void OnCellClicked(int dataIndex)
         {
             var fam = _dataList[dataIndex];
             SendBindCommand(fam.Number);
         }

         private void SetCell(ButtonCell cell, int index)
         {
             if (index < 0 || index >= _dataList.Count)
             {
                 cell.Disable();
                 return;
             }

             var data = _dataList[index];
             cell.Button.ButtonText.text = data.Name;
         }

         private readonly List<FamData> _dataList = new();

         public class FamData
         {
             public int Number { get; set; }
             public string Name { get; set; }
             public ColorNameData Data { get; set; }
         }
         #endregion
     }*/
}
