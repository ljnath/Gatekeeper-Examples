using Newtonsoft.Json;
using System;
using System.Management;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace GatekeeperTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string applicationName = "GatekeeperTestApp";
            string applicationVersion = "1.0.1";
            string applicationId = "CSHARP_7599d4dd0bdd4c0db81c58ef4974df3a";
            string clientId = Hardware.GetId();

            string apiKey = "your-gatekeeper-api-key";

            while (true)
            {
                Console.WriteLine("\nChecking license in 3 seconds...");
                Thread.Sleep(3000);

                using (HttpClient client = new HttpClient())
                {

                    var requestUri = "https://app.ljnath.com/gatekeeper/v3/licenses/license";
                    var requestBody = new
                    {
                        application_id = applicationId,
                        application_name = applicationName,
                        application_version = applicationVersion,
                        client_id = clientId
                    };

                    string jsonBody = JsonConvert.SerializeObject(requestBody);
                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                    HttpResponseMessage response = client.PostAsync(requestUri, content).Result;
                    string responseContent = response.Content.ReadAsStringAsync().Result;

                    Console.WriteLine("License response is :" + responseContent);
                }
            }
        }
    }



    class Hardware
    {

        public static string GetId()
        {
            string hardwareId = String.Empty;
            string processorId = GetProcessorId();
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(processorId));
                foreach (byte b in hashValue)
                    hardwareId += $"{b:X2}";
            }
            return hardwareId;
        }

        private static string GetProcessorId()
        {
            string info = string.Empty;
            try
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");
                ManagementObjectCollection queryCollection = managementObjectSearcher.Get();
                foreach (ManagementObject managementObject in queryCollection)
                {
                    info = managementObject.ToString().Trim();
                    break;
                }
            }
            catch
            {
                info = string.Empty;
            }
            return info;
        }
    }

}
