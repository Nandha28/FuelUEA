namespace FuelUED.Modal
{
    public class BillDetails
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public string AvailableLiters { get; set; }
        public string BillPrefix { get; set; }
        public string BillCurrentNumber { get; set; }
        public string DeviceStatus { get; set; }
    }
}