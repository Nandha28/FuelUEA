using RestSharp;

namespace FuelUED.Service
{
    public static class WebService
    {
        //= "49.207.180.49";
        public static string IPADDRESS;
        const string BASEURL = "http://";
        const string SUBURL = "/GenIT/GenData.asmx";


        public static string GetDataFromWebService(string subURL)
        {
            var client = new RestClient(BASEURL + IPADDRESS + SUBURL + subURL);
            var request = new RestRequest(Method.GET);
            // request.AddHeader("postman-token", "6e4a4235-cc51-e3fb-79df-3c66df033c77");
            //request.AddHeader("cache-control", "no-cache");
            IRestResponse response = client.Execute(request);
            System.Console.WriteLine(response.Content);
            //var data = new String("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<string xmlns=\"http://tempuri.org/\">
            //[{\"VehicleID_PK\":\"1635\",\"RegNo\":\"TN21AX4273\",\"DriverID_PK\":\"158\",\"DriverName\":\"MITTU\",\"TypeName\":\
            //"Line Vehicle\"},{\"VehicleID_PK\":\"1635\",\"RegNo\":\"TN21AX4273\",\"DriverID_PK\":\"159\",\"DriverName\":\"TULLU\",\"
            //TypeName\":\"Line Vehicle\"},{\"VehicleID_PK\":\"1636\",\"RegNo\":\"TN19K1207\",\"DriverID_PK\":\"160\",\"DriverName\":\"
            //AMYAJENA\",\"TypeName\":\"Line Vehicle\"},{\"VehicleID_PK\":\"1637\",\"RegNo\":\"TN19H3430\",\"DriverID_PK\":\"161\",\"Dri
            //verName\":\"PRADEEPGOUD\",\"TypeName\":\"Line Vehicle\"}]</string>");   49.207.180.49         
            return Between(response.Content, "org/\">", "</string>");
        }

        public static string PostDeviceAndSiteIDToWebService(string method, string deviceID, string siteId)
        {
            var client = new RestClient(BASEURL + IPADDRESS + SUBURL);
            var request = new RestRequest(Method.POST);
            //request.AddHeader("postman-token", "d7911b33-e1ce-7598-4f24-7dbc2bfc2e2c");
            //request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "text/xml");
            request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<soap:Envelope " +
                "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" +
                " xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n  " +
                "<soap:Body>\r\n    " +
                "<" + method + " xmlns=\"http://tempuri.org/\">\r\n   " +
                "   <DID>" + deviceID + "</DID>\r\n    " +
                "  <SiteID>" + siteId + "</SiteID>\r\n" +
                "    </" + method + ">\r\n " +
                " </soap:Body>\r\n" +
                "</soap:Envelope>", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            //var resp = new string("<GetVDResult>" +
            //    "[{\"VID\":\"5000.00\",\"RegNo\":\"FE\",\"DriverID_PK\":\"1\",\"DriverName\":\"1\",\"TypeName\":\"1A\",\"OpeningKM\":\"0\"}," +
            //    "{\"VID\":\"1635\",\"RegNo\":\"TN21AX4273\",\"DriverID_PK\":\"159\",\"DriverName\":\"TULLU\",\"TypeName\":\"Line Vehicle\"}," +
            //    "{\"VID\":\"1636\",\"RegNo\":\"TN19K1207\",\"DriverID_PK\":\"160\",\"DriverName\":\"AMYAJENA\",\"TypeName\":\"Line Vehicle\"}," +
            //    "{\"VID\":\"1637\",\"RegNo\":\"TN19H3430\",\"DriverID_PK\":\"161\",\"DriverName\":\"PRADEEPGOUD\",\"TypeName\":\"Line Vehicle\"}]" +
            //    "</GetVDResult>");

            return Between(response.Content, "<GetVDResult>", "</GetVDResult>");
        }

        public static string PostAllDataToWebService(string method, string json)
        {
            var client = new RestClient(BASEURL + IPADDRESS + SUBURL);
            var request = new RestRequest(Method.POST);
            //request.AddHeader("postman-token", "d7911b33-e1ce-7598-4f24-7dbc2bfc2e2c");
            //request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "text/xml");
            request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<soap:Envelope " +
                "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" +
                " xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n  " +
                "<soap:Body>\r\n    " +
                "<" + method + " xmlns=\"http://tempuri.org/\">\r\n   " +
                //"   <DID>" + deviceID + "</DID>\r\n    " +
                "  <JRes>" + json + "</JRes>\r\n" +
                "    </" + method + ">\r\n " +
                " </soap:Body>\r\n" +
                "</soap:Envelope>", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            //var resp = new string("<GetVDResult>" +
            //    "[{\"VID\":\"5000.00\",\"RegNo\":\"FE\",\"DriverID_PK\":\"1\",\"DriverName\":\"1\",\"TypeName\":\"1A\",\"OpeningKM\":\"0\"}," +
            //    "{\"VID\":\"1635\",\"RegNo\":\"TN21AX4273\",\"DriverID_PK\":\"159\",\"DriverName\":\"TULLU\",\"TypeName\":\"Line Vehicle\"}," +
            //    "{\"VID\":\"1636\",\"RegNo\":\"TN19K1207\",\"DriverID_PK\":\"160\",\"DriverName\":\"AMYAJENA\",\"TypeName\":\"Line Vehicle\"}," +
            //    "{\"VID\":\"1637\",\"RegNo\":\"TN19H3430\",\"DriverID_PK\":\"161\",\"DriverName\":\"PRADEEPGOUD\",\"TypeName\":\"Line Vehicle\"}]" +
            //    "</GetVDResult>");

            return Between(response.Content, "<UPFStockResult>", "</UPFStockResult>");
        }

        public static string Between(string Text, string FirstString, string LastString)
        {
            try
            {
                string STR = Text;
                string STRFirst = FirstString;
                string STRLast = LastString;
                string FinalString;
                int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
                int Pos2 = STR.IndexOf(LastString);
                FinalString = STR.Substring(Pos1, Pos2 - Pos1);
                return FinalString;
            }
            catch { }
            return null;
        }
    }
}