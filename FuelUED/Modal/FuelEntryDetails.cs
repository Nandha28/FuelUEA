using FuelUED.CommonFunctions;

namespace FuelUED.Modal
{
    public class FuelEntryDetails
    {        
        public string VID { get; set; }
        public string BillNumber { get; set; }
        public string CurrentDate { get; set; }
        public string FuelType { get; set; }
        public string FuelStockType { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleType { get; set; }
        public string DriverName { get; set; }
        public string FuelInLtrs { get; set; }
        public string OpeningKMS { get; set; }
        public string ClosingKMS { get; set; }
        public string TotalKM { get; set; }
        public string Kmpl { get; set; }
        public string DriverID_PK { get; set; }
        public string FilledBy { get; set; }
        public string PaymentType { get; set; }
        public string RatePerLtr { get; set; }
        public string Price { get; set; }
        public string Remarks { get; set; }       
        public string MeterFault { get; set; }
        public string IsExcess { get; set; } = ConstantValues.ZERO;
        public decimal ExcessLtr { get; set; } = 0.00m;
        public string IsShortage { get; set; } = ConstantValues.ZERO;
        public decimal ShortageLtr { get; set; } = 0.00m;
    }
}