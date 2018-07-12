using FuelApp.Modal;
using FuelUED.Modal;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace FuelUED
{
    public class FuelDB
    {
        private SQLiteConnection localDB;
        public string DBPath;
        static FuelDB singleton;

        public static FuelDB Singleton => singleton ?? (singleton = new FuelDB());

        public FuelDB()
        {
            DBPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "fuel.db");
        }
        public void CreateDatabase<T>()
        {
            localDB = new SQLiteConnection(DBPath);
            localDB.CreateTable<T>();
        }
        public void InsertValues(List<VehicleDetails> vehicleDetails)
        {
            foreach (var item in vehicleDetails)
            {
                localDB.Insert(item);
            }
            //GetValue();
        }
        public void InsertFuelEntryValues(FuelEntryDetails fuelEntryDetails)
        {
            localDB.Insert(fuelEntryDetails);            
        }

        public TableQuery<FuelEntryDetails> GetFuelValues()
        {
            if (DBExists())
            {
                localDB = new SQLiteConnection(DBPath);
                var table = localDB.Table<FuelEntryDetails>();
                foreach (var s in table)
                {
                    Console.WriteLine(s.DriverName + " " + s.VehicleNumber);
                }
               return table;
            }
            return null;
        }

        public TableQuery<VehicleDetails> GetValue()
        {
            if (DBExists())
            {
                localDB = new SQLiteConnection(DBPath);
                var table = localDB.Table<VehicleDetails>();
                foreach (var s in table)
                {
                    Console.WriteLine(s.RegNo + " " + s.DriverName);
                }
                return table;
            }
            return null;
        }
        public bool DBExists()
        {
            return File.Exists(DBPath);
        }
        public void DeleteTable<T>()
        {
            localDB = new SQLiteConnection(DBPath);
            localDB.DeleteAll<T>();
        }
        public void CreateTable<T>()
        {
            localDB = new SQLiteConnection(DBPath);
            localDB.CreateTable<T>();
        }
    }
}