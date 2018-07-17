using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi_v1.Models
{
    #region Abstract Classes
    public abstract class SpacecraftInfo
    {
        public List<Info> Parameters = new List<Info>();

        public void AddInfo(string name, string type, string units, string fill)
        {
            RequiredInfo info = new RequiredInfo
            {
                Name = name,
                Type = type,
                Units = units,
                Fill = fill
            };

            Parameters.Add(info);
        }
    }

    public abstract class Info
    {
    }
    #endregion

    #region Inherited "Info" Classes
    public class RequiredInfo : Info
    {
        public string Name { get; set; }           // required
        public string Type { get; set; }           // required
        public string Units { get; set; }           // required
        public string Fill { get; set; }            // required
    }

    public class OptionalInfo : Info
    {
        public string Description { get; set; }     // optional
        public Bins[] Bins { get; set; }            // optional
    }

    public class StringInfo : Info
    {
        public int Length { get; set; }             // required for string and isotime NOT others
    }

    public class ArrayInfo : Info
    {
        public int[] Size { get; set; }             // required for array parameters NOT others
    }
    
    public abstract class Bins
    {
        public string Name { get; set; }            // required
        public double[] Centers { get; set; }       // required
        public double[,] Ranges { get; set; }       // required
        public string Units { get; set; }           // required
    }

    public class OptionalBins : Bins
    {
        public string Description { get; set; }     // optional
    }
    #endregion

    #region Inherited "SpacecraftInfo" Classes
    public class RBSpiceA : SpacecraftInfo
    {
        public RBSpiceA()
        {
            AddInfo("UTC", "isotime", null, "24");
            AddInfo("SCLOCK_Full", "isotime", null, "24");
            AddInfo("ET", "isotime", null, "24");
            AddInfo("OrbitNumber", "isotime", null, "24");
            AddInfo("Seconds", "isotime", null, "24");
            AddInfo("Subseconds", "isotime", null, "24");
            AddInfo("Spin", "isotime", null, "24");
            AddInfo("SpinTime", "isotime", null, "24");
            AddInfo("SpinPeriod", "isotime", null, "24");
            AddInfo("SpinPhase", "isotime", null, "24");
            AddInfo("MagPhase", "isotime", null, "24");
            AddInfo("Mag0Second1", "isotime", null, "24");
            AddInfo("Mag0Subsecond1", "isotime", null, "24");
            AddInfo("Mag0Second2", "isotime", null, "24");
            AddInfo("Mag0Subsecond2", "isotime", null, "24");
            AddInfo("IntegrationSectors", "isotime", null, "24");
            AddInfo("IntegrationMultiplier1", "isotime", null, "24");
            AddInfo("IntegrationMultiplier2", "isotime", null, "24");
            AddInfo("IntegrationSpins", "isotime", null, "24");
            AddInfo("MagPhaseValid", "isotime", null, "24");
            AddInfo("MagDataValid", "isotime", null, "24");
            AddInfo("SpinDataValid", "isotime", null, "24");
            AddInfo("ElectronPixel", "isotime", null, "24");
            AddInfo("IonEnergyPixel", "isotime", null, "24");
            AddInfo("IonSpeciesPixel", "isotime", null, "24");
            AddInfo("Subsector1", "isotime", null, "24");
            AddInfo("Subsector2", "isotime", null, "24");
            AddInfo("Subsector3", "isotime", null, "24");
            AddInfo("SpinDuration", "isotime", null, "24");
        }
    }
    #endregion
}