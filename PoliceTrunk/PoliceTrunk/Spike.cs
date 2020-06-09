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
    public class Spike
    {
        public Prop spike;

        public Spike(Vector3 pos)
        {
            //spike = World.CreateProp("p_ld_stinger_s", pos, new Vector3(0, 0, 0), false, true);
            spike = World.CreateProp("p_ld_stinger_s", pos, Game.Player.Character.Rotation, false, true);
            spike.FreezePosition = true;
        }

        public List<Vehicle> GetVehiclesOnTop()
        {
            Vehicle[] vehicles = World.GetAllVehicles();
            List<Vehicle> closeVehicles = new List<Vehicle>();

            float closestDistance = 1.5f;
            foreach (Vehicle vehicle in vehicles)
            {
                if (vehicle.Position.DistanceTo(this.spike.Position) <= closestDistance) //Gets Distance in between cone position and vehicle position
                    closeVehicles.Add(vehicle); //If any vehicle is closer tha 2.5f, then it return this vehicle
            }
            return closeVehicles;
        }
    }
}
