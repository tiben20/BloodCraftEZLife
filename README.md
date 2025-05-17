# BloodCraft UI (OnlyFams!)

Currently BloodCraftUI is in development and provides UI `only for Familiars`. It is coded to send Bloodcraft commands and parse chatlog response messages. Considering the 'unreliable' nature of message parsing approach it may sometimes produce errors in parsing boxes and its content if some other server messages interfere with the routine.

### Supported Bloodcraft `v1.9.7`

## Installation
1. Install [BepInExRC2](https://github.com/decaprime/VRising-Modding/releases/tag/1.733.2)
2. Download mod archive from [Releases](https://github.com/panthernet/BloodCraftUI/releases) and unpack its content into `VRising` root folder
3. Run the game
NOTE: Requires [BloodCraft](https://github.com/mfoltz/Bloodcraft) mod to be installed and running.

## Showcase and FAQ
You will see control panel on top of the screen and can drag it around as you like and pin with checkbox on the left.
<img src="https://github.com/user-attachments/assets/8c8277ec-cc64-4e6a-b45b-8509bf365c6d" width="750">


### Box List button
Will open list of available boxes. Update occur on opening the panel. Press '-' button to close panel.
You can drag this panel by the title area. Clicking the box in the list will open the Box Content panel

<img src="https://github.com/user-attachments/assets/9b549f17-8738-46b6-a1d7-72cec6753221" width="200">

### Box Content panel
Will open list of available familiars in the box. Update occur on opening the panel. Press '-' button to close panel.
You can drag this panel by the title area. Clicking familiar in the list will perform two actions:
1. Try to unbind current familiar
2. Bind selected familiar
Action might take several seconds due to server response lag.

<img src="https://github.com/user-attachments/assets/276351ca-b4ed-4645-ab48-a4813787c4a9" width="200">

### Fam Stats button
Will open familiar stats panel. Press the button again to hide panel. You can drag this panel by the title area and pin with checkbox in the top left corner.

<img src="https://github.com/user-attachments/assets/7fa6e0bf-7e80-4792-9ebd-dd37705e2395" width="200">

### Unbind button
Will try to unbind current familiar.

### Bind Last button
Will try to bind familiar, the last one that you have binded before. 
Will not work if:
- You checked content of another box (as it selects it as current box to get fam list)
- Server was restarted and reset your current selected box
  
In all other cases it would persist even when client is restarted.

### Combat Mode checkbox
Toggle current familiar combat mode

### * button
Will cycle the UI scaling for all panels. You can select the scale that better suits your screen resolution.

## Additional settings
Mod config file path: `VRising\BepInEx\config\panthernet.BloodCraftUI.cfg`

- `UseHorizontalContentLayout` when set to False will render main panel vertically.
- `UITransparency` change all panels transparency from 1.0f as opaque to 0f as invisible
- `FamStatsQueryIntervalInSeconds` time in seconds between queries to server to update fam stats, minimum value is 10 sec
- `ClearServerMessages` when set to False will make server response messages visible, which can clutter your chat 
- `IsFamStatsPanelEnabled` enable or disable corresponding panel and button
- `IsBoxPanelEnabled` enable or disable corresponding panel and button
- `IsBindButtonEnabled` enable or disable corresponding buttons
- `IsCombatButtonEnabled` enable or disable corresponding button
