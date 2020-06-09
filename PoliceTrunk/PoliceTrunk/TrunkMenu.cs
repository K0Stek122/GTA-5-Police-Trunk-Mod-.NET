using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;

namespace PoliceTrunk
{
    public class TrunkMenu
    {
        public UIMenu Menu;

        public UIMenuCheckboxItem ConesItem;
        public UIMenuCheckboxItem SpikeStripItem;

        public UIMenuItem ClearSceneItem;

        public UIMenuItem CloseTrunkItem;

        public TrunkMenu()
        {
            this.Menu = new UIMenu("Police Trunk", "Choose an option");

            this.Menu.AddItem(ConesItem = new UIMenuCheckboxItem("Cones", false));
            this.Menu.AddItem(SpikeStripItem = new UIMenuCheckboxItem("Spike Strip", false));

            this.Menu.AddItem(ClearSceneItem = new UIMenuItem("Clear Scene"));

            this.Menu.AddItem(CloseTrunkItem = new UIMenuItem("Close Trunk"));

            this.Menu.RefreshIndex();
            this.Menu.MouseControlsEnabled = false;
        }
    }
}
