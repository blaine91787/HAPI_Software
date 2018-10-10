
namespace WebApi_v1.HAPI.HapiDataProducts.SpaceCraft.RBSPA.RBSpice.Products.ESRHELT
{
    public class Esrhelt
    {
        public string Epoch { get; set; }
        public string UTC { get; set; }
        public string DDOY { get; set; }
        public string ET { get; set; }
        public string MidET { get; set; }
        public string StopET { get; set; }
        public string Duration { get; set; }
        public string OrbitNumber { get; set; }
        public string Spin { get; set; }
        public string FEDU { get; set; }
        public string FEDUWeight { get; set; }
        public string FEDUPerpPressure { get; set; }
        public string FEDUParaPressure { get; set; }
        public string FEDUDensity { get; set; }
        public string FEDUIntegralFlux { get; set; }
        public string FEDUOmniFlux { get; set; }
        public string FEDUMinimFlux { get; set; }
        public string FEDUMaximFlux { get; set; }
        public string FEDUError { get; set; }
        public string FEDUEnergy { get; set; }
        public string FEDUEnergyRange { get; set; }
        public string FEDUQuality { get; set; }
        public string Position { get; set; }
        public string PositionSM { get; set; }
        public string PositionGSM { get; set; }
        public string PositionQuality { get; set; }
        public string L { get; set; }
        public string MLT { get; set; }
        public string LEq { get; set; }
        public string LStar { get; set; }
        public string I { get; set; }
        public string PAMidpoint { get; set; }
        public string PARange { get; set; }
        public string Channel { get; set; }
        public string Bin { get; set; }
        public string Axis { get; set; }
        public string MinMaxRange { get; set; }
    }
    public class EsrheltRecords : ProductRecords
    {
        public EsrheltRecords(Hapi hapi)
        {
            if (hapi != null)
                Hapi = hapi;
        }
    }
}