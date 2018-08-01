using RestSharp;
using System;
using System.Threading.Tasks;

namespace Utilities
{
    public class WebService
    {
        static WebService singleton;
        public static WebService Singleton
        {
            get
            {
                return singleton ?? (singleton = new WebService());
            }
        }
        public static string IPADDRESS;
        const string BASEURL = "http://";
        const string SUBURL = "/GenIT/GenData.asmx";

        public string GetDataFromWebService(string method, string deviceID, string siteId)
        {
            var client = new RestClient(BASEURL + IPADDRESS + SUBURL);
            var request = new RestRequest(Method.GET);
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
            Console.WriteLine(response.Content);
            return Between(response.Content, "org/\">", "</string>");
        }
        public string GetBillDetails(string method, string billNo, string siteId)
        {
            var client = new RestClient(BASEURL + IPADDRESS + SUBURL);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "text/xml");
            request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<soap:Envelope " +
                "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" +
                " xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n  " +
                "<soap:Body>\r\n    " +
                "<" + method + " xmlns=\"http://tempuri.org/\">\r\n   " +
                "   <LB>" + billNo + "</LB>\r\n    " +
                "  <SiteID>" + siteId + "</SiteID>\r\n" +
                "    </" + method + ">\r\n " +
                " </soap:Body>\r\n" +
                "</soap:Envelope>", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return Between(response.Content, "<CheckVEResult>", "</CheckVEResult>");
        }

        public string UVEDS(string billNo, string itemId, string itemName, string loadTime)
        {
            //11/01/2018 13:05:00 == loadTime
            var client = new RestClient(BASEURL + IPADDRESS + SUBURL);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "text/xml");
            request.AddParameter("text/xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<soap:Envelope " +
                "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" +
                " xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n  " +
                "<soap:Body>\r\n    " +
                "<UVEDS xmlns=\"http://tempuri.org/\">\r\n   " +
                "   <LB>" + billNo + "</LB>\r\n    " +
                "  <ItemID>" + itemId + "</ItemID>\r\n" +
                "  <ItemName>" + itemName + "</ItemName>\r\n" +
                "  <LoadTime>" + loadTime + "</LoadTime>\r\n" +
                "    </UVEDS>\r\n " +
                " </soap:Body>\r\n" +
                "</soap:Envelope>", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return Between(response.Content, "<UVEDSResult>", "</UVEDSResult>");
        }

        public Task<string> PostDataToWebService(string method, string deviceID, string siteId, string responseText)
        {
            var client = new RestClient(BASEURL + IPADDRESS + SUBURL);
            var request = new RestRequest(Method.POST);
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
            var response = client.Execute(request);
            //var resp = new string("<GetVDResult>" +
            //    "[{\"VID\":\"5000.00\",\"RegNo\":\"FE\",\"DriverID_PK\":\"1\",\"DriverName\":\"1\",\"TypeName\":\"1A\",\"OpeningKM\":\"0\"}," +
            //    "{\"VID\":\"1635\",\"RegNo\":\"TN21AX4273\",\"DriverID_PK\":\"159\",\"DriverName\":\"TULLU\",\"TypeName\":\"Line Vehicle\"}," +
            //    "{\"VID\":\"1636\",\"RegNo\":\"TN19K1207\",\"DriverID_PK\":\"160\",\"DriverName\":\"AMYAJENA\",\"TypeName\":\"Line Vehicle\"}," +
            //    "{\"VID\":\"1637\",\"RegNo\":\"TN19H3430\",\"DriverID_PK\":\"161\",\"DriverName\":\"PRADEEPGOUD\",\"TypeName\":\"Line Vehicle\"}]" +
            //    "</GetVDResult>");
            return Task.Run(() => Between(response.Content, $"<{responseText}>", $"</{responseText}>"));

            //  return Between(response.Content, $"<{responseText}>", $"</{responseText}>");
        }

        public string PostAllDataToWebService(string method, string json, string responseStr)
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

            return Between(response.Content, $"<{responseStr}>", $"</{responseStr}>");
        }

        public string Between(string Text, string FirstString, string LastString)
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
