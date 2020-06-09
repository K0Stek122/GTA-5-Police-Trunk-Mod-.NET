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
    public class Main : Script
    {
        private bool specialFunctionality = true;
        private bool keyLock = false;

        private const float DISTANCE_TO_TRUNK = 1.5f;
        private const float TRUNK_OFFSET = -2.5f;

        List<Model> policeCars = new List<Model>()
        {
            "POLICE", "POLICE2", "POLICE3", "POLICE4", "POLICE5", "POLICE6", "POLICE7", "POLICE8", "POLICE9",
            "SHERIFF", "SHERIFF2", "SHERIFF3", "SHERIFF4", "SHERIFF5", "SHERIFF21", "SHERIFF31", "SHERIFF51",
            "FBI", "FBI2"
        };
        List<Cone> cones = new List<Cone>();
        List<Spike> spikes = new List<Spike>();

        private MenuPool menuPool;
        private TrunkMenu trunkMenu;

        private ScriptSettings iniFile;

        public Main()
        {
            this.KeyUp += onKeyUp;
            this.Tick += onTick;

            iniFile = ScriptSettings.Load("scripts\\PoliceTrunk.ini");

            menuPool = new MenuPool();

            trunkMenu = new TrunkMenu();
            trunkMenu.Menu.OnItemSelect += onItemSelect;

            menuPool.Add(trunkMenu.Menu);

            UI.Notify("~r~PoliceTrunk ~w~By ~r~Kostek ~w~has been initialized successfully");
        }

        public void onTick(object sender, EventArgs e)
        {
            //Menu Opening
            if (!this.keyLock)
            {
                //Menu
                Vehicle closestVehicle = World.GetClosestVehicle(Game.Player.Character.Position, 5.0f);
                if (closestVehicle != null)
                {
                    Vector3 trunkOffset = closestVehicle.GetOffsetInWorldCoords(new Vector3(0, TRUNK_OFFSET, 0)); //Get vehicle offset of the trunk
                    if (Game.Player.Character.Position.DistanceTo(trunkOffset) < DISTANCE_TO_TRUNK && closestVehicle.IsDoorOpen(VehicleDoor.Trunk) && policeCars.Contains(closestVehicle.Model)) //If player is close enough to the trunk
                    {
                        if (!trunkMenu.Menu.Visible)
                            trunkMenu.Menu.Visible = true;
                    }
                    else
                        trunkMenu.Menu.Visible = false;
                }
            }

            //Cones
            if (specialFunctionality)
            {
                foreach (Cone cone in cones)
                {
                    //If There are some nearby vehicles near cones, then freeze the position of the car near the cone
                    cone.FreezeNearbyVehicles(true);
                }
            }

            //Spike Strips
            for (int i = 0; i < spikes.Count; i++)
            {
                if (spikes[i].GetVehiclesOnTop().Count > 0)
                {
                    foreach (Vehicle vehicle in spikes[i].GetVehiclesOnTop())
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            vehicle.BurstTire(j);
                        }
                    }
                    spikes[i].spike.Delete();
                    spikes.RemoveAt(i);
                }
            }

            menuPool.ProcessMenus();
        }

        public void onKeyUp(object sender, KeyEventArgs e)
        {
            if (!this.keyLock)
            {
                if (e.KeyCode == iniFile.GetValue<Keys>("Keys", "OpenTrunkKey", Keys.F7))
                {
                    Vehicle closestVehicle = World.GetClosestVehicle(Game.Player.Character.Position, 5.0f);
                    if (closestVehicle != null)
                    {
                        Vector3 trunkOffset = closestVehicle.GetOffsetInWorldCoords(new Vector3(0, TRUNK_OFFSET, 0)); //Get vehicle offset of the trunk
                        if (Game.Player.Character.Position.DistanceTo(trunkOffset) < DISTANCE_TO_TRUNK && !Game.Player.Character.IsInVehicle() && policeCars.Contains(closestVehicle.Model)) //If player is close enough to the trunk and it is a police vehicle
                            closestVehicle.OpenDoor(VehicleDoor.Trunk, false, false);
                    }
                }

                if (e.KeyCode == iniFile.GetValue<Keys>("Keys", "InteractionKey", Keys.F5))
                {
                    //Cones
                    if (trunkMenu.ConesItem.Checked)
                    {
                        cones.Add(new Cone(Game.Player.Character.GetOffsetInWorldCoords(new Vector3(0, 0.8f, 0))));
                    }

                    //Spike Strips
                    else if (trunkMenu.SpikeStripItem.Checked)
                    {
                        Game.Player.Character.Task.PlayAnimation("mp_weapons_deal_sting", "crackhead_bag_loop");
                        spikes.Add(new Spike(Game.Player.Character.GetOffsetInWorldCoords(new Vector3(0, 2.5f, 0))));
                    }
                }

                if (e.KeyCode == iniFile.GetValue<Keys>("Keys", "ToggleSpecialFunctionality", Keys.F3))
                {
                    specialFunctionality = !specialFunctionality;
                    string str = specialFunctionality ? "~g~ENABLED~w~" : "~r~DISABLED~w~";
                    //UI.Notify("Special Functionality: " + str);
                    UI.ShowHelpMessage("Special Functionality: " + str);
                    foreach (Cone cone in cones)
                        cone.FreezeNearbyVehicles(false);
                }
            }

            if (e.KeyCode == iniFile.GetValue<Keys>("Keys", "ToggleKeyLockKey", Keys.F4))
            {
                this.keyLock = !this.keyLock;
                string str = this.keyLock ? "~r~LOCKED" : "~g~UNLOCKED";
                UI.ShowHelpMessage("Keys: " + str);
            }
        }

        private void onItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            if (sender == trunkMenu.Menu)
            {
                //Clear Scene
                if (selectedItem == trunkMenu.ClearSceneItem)
                {
                    foreach (Cone cone in cones)
                    {
                        cone.FreezeNearbyVehicles(false);
                        cone.cone.Delete();
                    }
                    cones.Clear();

                    foreach (Spike spike in spikes)
                    {
                        spike.spike.Delete();
                    }
                    spikes.Clear();
                }

                //Close Trunk
                if (selectedItem == trunkMenu.CloseTrunkItem)
                {
                    Vehicle closestVehicle = World.GetClosestVehicle(Game.Player.Character.Position, 5.0f);
                    if (closestVehicle != null)
                    {
                        Vector3 trunkOffset = closestVehicle.GetOffsetInWorldCoords(new Vector3(0, TRUNK_OFFSET, 0)); //Get vehicle offset of the trunk
                        if (Game.Player.Character.Position.DistanceTo(trunkOffset) < DISTANCE_TO_TRUNK && policeCars.Contains(closestVehicle.Model)) //If player is close enough to the trunk and it is a police vehicle
                        {
                            trunkMenu.ConesItem.Checked = false;
                            trunkMenu.SpikeStripItem.Checked = false;
                            closestVehicle.CloseDoor(VehicleDoor.Trunk, false);
                        }
                    }
                }
            }
        }

    }
}