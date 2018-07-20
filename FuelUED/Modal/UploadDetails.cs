namespace FuelUED.Modal
{
    public class UploadDetails
    {
        const string DEFAULTVALUE = "0";
        public string DID { get; set; } = DEFAULTVALUE;
        public string SID { get; set; } = DEFAULTVALUE;
        public string CID { get; set; } = DEFAULTVALUE;
        public string CStock { get; set; } = DEFAULTVALUE;
        public string FuelNo { get; set; } = DEFAULTVALUE;
        public string FuelDate { get; set; } = DEFAULTVALUE;
        public string TransType { get; set; } = DEFAULTVALUE;
        public string FuelSource { get; set; } = DEFAULTVALUE;
        public string VehicleID { get; set; } = DEFAULTVALUE;
        public string RegNo { get; set; } = DEFAULTVALUE;
        public string VType { get; set; } = DEFAULTVALUE;
        public string DriverID { get; set; } = DEFAULTVALUE;
        public string DriverName { get; set; } = DEFAULTVALUE;
        public string OpeningKM { get; set; } = DEFAULTVALUE;
        public string ClosingKM { get; set; } = DEFAULTVALUE;
        public string FuelLts { get; set; } = DEFAULTVALUE;
        public string KMPL { get; set; } = DEFAULTVALUE;
        public string FilledBy { get; set; } = DEFAULTVALUE;
        public string TotalKM { get; set; } = DEFAULTVALUE;
        public string Mode { get; set; } = DEFAULTVALUE;
        public string Rate { get; set; } = DEFAULTVALUE;
        public string TAmount { get; set; } = DEFAULTVALUE;
        public string Remarks { get; set; } = DEFAULTVALUE;
        public string MeterFault { get; set; } = DEFAULTVALUE;
    }
}