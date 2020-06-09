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
    public class Cone
    {
        List<Model> policeCars = new List<Model>()
        {
            "POLICE", "POLICE2", "POLICE3", "POLICE4", "POLICE5", "POLICE6", "POLICE7", "POLICE8", "POLICE9",
            "SHERIFF", "SHERIFF2", "SHERIFF3", "SHERIFF4", "SHERIFF5", "SHERIFF21", "SHERIFF31", "SHERIFF51",
            "FBI", "FBI2"
        };

        public Prop cone;

        public Cone(Vector3 pos)
        {
            this.cone = World.CreateProp(new Model("prop_mp_cone_01"), pos, false, true);
            this.cone.FreezePosition = true;
        }

        public void FreezeNearbyVehicles(bool freeze)
        {
            if (this.GetNearbyVehicles().Count > 0)
            {
                foreach (Vehicle vehicle in this.GetNearbyVehicles())
                {
                    vehicle.FreezePosition = freeze;
                }
            }
        }

        public List<Vehicle> GetNearbyVehicles()
        {
            Vehicle[] vehicles = World.GetAllVehicles();
            List<Vehicle> closeVehicles = new List<Vehicle>();

            float closestDistance = 3.0f;
            foreach (Vehicle vehicle in vehicles)
            {
                if (vehicle.Position.DistanceTo(this.cone.Position) <= closestDistance && !policeCars.Contains(vehicle.Model)) //Gets Distance in between cone position and vehicle position
                    closeVehicles.Add(vehicle); //If any vehicle is closer tha 2.5f, then it return this vehicle
            }
            return closeVehicles;
        }
    }
}
