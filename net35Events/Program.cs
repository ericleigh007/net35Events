using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace net35Events
{
    class Sensor
    {
        public string time;
        public string sessionStart;
        public string sessionEnd;
        public string stringState;
        public string dspl;
        public string crewID;
        public int zero = 0;           // eventually, thees may be queries
        public int top_percent = 100;  // 
        public int temp;
        public int hmdt;
        public int heartRate;
        public double runtimeSeconds;
        public double distanceSinceGoal;
        public int coll;
        public int aggColl;
        public double timeToShot;
        public double timeToHit;
        public double timeToKill;
        public double timeToDied;
        public int shot;
        public int aggShot;
        public int hit;
        public int aggHit;
        public int kill;
        public int aggKill;
        public int iDied;
        public int aggiDied;
        public double x_pos;
        public double z_pos;
        public double heading_deg;
        public double incrDistance;
        public double distance;
        public double speed;
        public bool reachedGoal;
        public int goalsReached;
        public double goalDistance;
    };

    class Program
    {
        static void Main(string[] args)
        {
            // Generate a SAS key with the Signature Generator.: https://github.com/sandrinodimattia/RedDog/releases
            // looks like: "SharedAccessSignature sr=https%3a%2f%2freddogeventhub.servicebus.windows.net%2ftemperature%2fpublishers%2fbathroom%2fmessages&sig=OsO5iA%2btDyxGLmFCcHNHmTRtMTr03VyZjAtdC5FFPEw%3d&se=1405562933&skn=SenderDevice";
            var sas = args[0];

            // Namespace info.
            var serviceNamespace = "raydoneventhubnamespace-01";
            var hubName = "raydontrainingdatahub-01";
            var deviceName = "device-01";

            Console.WriteLine("Starting device: {0}", deviceName);

            var uri = new Uri(String.Format("https://{0}.servicebus.windows.net/{1}/publishers/{2}/messages", serviceNamespace, hubName, deviceName));

            String crewGUID = Guid.NewGuid().ToString();
            int count = 0;

            // Keep sending.
            while (true)
            {
                var eventData = new Sensor()
                {
                    time = DateTime.UtcNow.ToString("O"),
                    sessionStart = DateTime.UtcNow.AddHours(1.0).ToString("O"),
                    sessionEnd = DateTime.UtcNow.AddHours(1.0).ToString("O"),
                    stringState = "running",
                    dspl = deviceName,
                    crewID = crewGUID,
                    zero = 0,           // eventually, thees may be queries
                    top_percent = 100,  // 
                    temp = 1,
                    hmdt = 20,
                    heartRate = 100,
                    runtimeSeconds = 10.0,
                    distanceSinceGoal = 1.0,
                    coll = 1,
                    aggColl = 2,
                    timeToShot = 1,
                    timeToHit = 1,
                    timeToKill = 1,
                    timeToDied = 1,
                    shot = 1,
                    aggShot = 2
                };

                var req = WebRequest.Create(uri);
                req.Method = "POST";
                req.Headers.Add("Authorization", sas);
                req.ContentType = "application/atom+xml;type=entry;charset=utf-8";

                string jsonString = JsonConvert.SerializeObject(eventData);
                using (var writer = new StreamWriter(req.GetRequestStream()))
                {
                    writer.Write(jsonString);
                }

                using (var response = req.GetResponse() as HttpWebResponse)
                {
                    Console.WriteLine("Sent message {0} using legacy HttpWebRequest: {1}", count++, jsonString);
                    Console.WriteLine(" > Response: {0} :: {1}", response.StatusCode, (int) response.StatusCode);

                }

                Thread.Sleep(500);
            }
        }
    }
}
