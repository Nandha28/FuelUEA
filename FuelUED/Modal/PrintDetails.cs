using Newtonsoft.Json;
using System;

namespace FuelUED.Modal
{
    public class PrintDetails
    {
        //public string BillNum { get; set; }
        //public string DateTime { get; set; }
        //public FuelType FuelType { get; set; }
        //public string VehicleNumber { get; set; }
        //public string VehicleType { get; set; }
        //public string DriverName { get; set; }
        //public string FilledBy { get; set; }
        //public StockType StockType { get; set; }
        //public string Litre { get; set; }
        //public string Remarks { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BillNumber { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CurrentDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FuelType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FuelStockType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string VehicleNumber { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string VehicleType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DriverName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FuelInLtrs { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MeterFault { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OpeningKMS { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ClosingKMS { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Kmpl { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FilledBy { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PaymentType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RatePerLtr { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Price { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Remarks { get; set; }
    }
}