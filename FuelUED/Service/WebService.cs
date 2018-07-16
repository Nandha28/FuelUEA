using FuelApp.Modal;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FuelUED.Service
{
    public static class WebService
    {
        const string BaseURL = "http://49.207.180.49/GenIT/GenData.asmx/";

        public static string GetDataFromWebService(string subURl)
        {
            var client = new RestClient(BaseURL + subURl);
            var request = new RestRequest(Method.GET);
            // request.AddHeader("postman-token", "6e4a4235-cc51-e3fb-79df-3c66df033c77");
            //request.AddHeader("cache-control", "no-cache");
            IRestResponse response = client.Execute(request);
            System.Console.WriteLine(response.Content);
            //var data = new String("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<string xmlns=\"http://tempuri.org/\">[{\"VehicleID_PK\":\"1635\",\"RegNo\":\"TN21AX4273\",\"DriverID_PK\":\"158\",\"DriverName\":\"MITTU\",\"TypeName\":\"Line Vehicle\"},{\"VehicleID_PK\":\"1635\",\"RegNo\":\"TN21AX4273\",\"DriverID_PK\":\"159\",\"DriverName\":\"TULLU\",\"TypeName\":\"Line Vehicle\"},{\"VehicleID_PK\":\"1636\",\"RegNo\":\"TN19K1207\",\"DriverID_PK\":\"160\",\"DriverName\":\"AMYAJENA\",\"TypeName\":\"Line Vehicle\"},{\"VehicleID_PK\":\"1637\",\"RegNo\":\"TN19H3430\",\"DriverID_PK\":\"161\",\"DriverName\":\"PRADEEPGOUD\",\"TypeName\":\"Line Vehicle\"}]</string>");
            return Between(response.Content, "org/\">", "</string>");
        }

        public static string Between(string Text, string FirstString, string LastString)
        {
            try
            {
                string STR = Text;
                string STRFirst = FirstString;
                string STRLast = LastString;
                string FinalString;
                string TempString;
                int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
                int Pos2 = STR.IndexOf(LastString);
                FinalString = STR.Substring(Pos1, Pos2 - Pos1);
                return FinalString;
            }
            catch { }
            return null;
        }
        public static void PostDataToWebService()
        {

        }
    }
}