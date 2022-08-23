using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Management;
using System.Diagnostics;

namespace TestWmi
{
    class Program
    {
        public const string WmiClientAgentPath = "\\root\\cmd\\clientagent";
        public const int InsertTestCount = 5000;
        static void Main(string[] args)
        {
            testAddWmiRecords();
            testUpdateWmiRecords();
            testSearchAndCastRecord();
            testdeleteAllWmiRecords();

        }

        public static void testdeleteAllWmiRecords()
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                       WmiClientAgentPath,
                       "SELECT * FROM PluginExecuteMetrics WHERE PluginName = 'testPluginName'")
                    {
                        Options = { Timeout = new TimeSpan(0, 0, 60) }
                    };

                foreach (var o in searcher.Get())
                {
                var provider = (ManagementObject)o;
                provider.Delete();
                }
                stopwatch.Stop();
                TimeSpan ts = stopwatch.Elapsed;
                string elaspedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                                    ts.Hours, ts.Minutes, ts.Seconds,
                                                    ts.Milliseconds);
                Console.WriteLine($"Deleting {InsertTestCount} metrics execute result takes {elaspedTime} ");
            }
            catch (Exception e)
            {
                Console.WriteLine($"failed to delete all wmi records: {e.ToString()}");
            }
             
        }

        public static void testAddWmiRecords()
        {
            Stopwatch stopwatch = new Stopwatch();
            
            var putOptions = new PutOptions
            {
                Timeout = new TimeSpan(0, 0, 60),
                Type = PutType.CreateOnly
            };
            stopwatch.Start();
            for (int i = 0; i < InsertTestCount; i++)
            {
                var metricsClass = new ManagementClass(WmiClientAgentPath, "PluginExecuteMetrics", new ObjectGetOptions());
                ManagementObject metricsInstance = metricsClass.CreateInstance();
                metricsInstance["PluginId"] = Guid.NewGuid().ToString();
                metricsInstance["PluginName"] = "testPluginName";
                metricsInstance["ExecuteTotalCount"] = 0;
                metricsInstance["ExecuteExceptionCount"] = 0;
                metricsInstance["ExecuteMinDuration"] = 0;
                metricsInstance["ExecuteAvgDuration"] = 0;
                metricsInstance["ExecuteMaxDuration"] = 0;
                metricsInstance.Put(putOptions);
            }

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elaspedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                                ts.Hours, ts.Minutes, ts.Seconds,
                                                ts.Milliseconds);
            Console.WriteLine($"Inserting {InsertTestCount} metrics execute result takes {elaspedTime} ");

        }

        public static void testUpdateWmiRecords()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var searcher = new ManagementObjectSearcher(
                   WmiClientAgentPath,
                   $"SELECT * FROM PluginExecuteMetrics WHERE PluginName = 'testPluginName'")
                {
                    Options = { Timeout = new TimeSpan(0, 0, 60) }
                };

                foreach (ManagementObject provider in searcher.Get())
                {
                    provider.SetPropertyValue("ExecuteTotalCount", 1);
                    provider.SetPropertyValue("ExecuteAvgDuration", 1);
                    provider.SetPropertyValue("ExecuteMinDuration", 1);
                    provider.SetPropertyValue("ExecuteMaxDuration", 1);
                    provider.SetPropertyValue("ExecuteExceptionCount", 1);

                    provider.Put();
                }

                stopwatch.Stop();
                TimeSpan ts = stopwatch.Elapsed;
                string elaspedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                                    ts.Hours, ts.Minutes, ts.Seconds,
                                                    ts.Milliseconds);
                Console.WriteLine($"Updating {InsertTestCount} metrics execute result takes {elaspedTime} ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"failed to update the records: {ex.ToString()}");
            }
            
        }

        public static void testSearchAndCastRecord()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var searcher = new ManagementObjectSearcher(
                       WmiClientAgentPath,
                       $"SELECT * FROM PluginExecuteMetrics WHERE PluginName = 'testPluginName'")
                {
                    Options = { Timeout = new TimeSpan(0, 0, 60) }
                };

                foreach (ManagementObject provider in searcher.Get())
                {

                    var metrics = new PluginExecuteMetrics
                    {
                        PluginId = provider["PluginId"].ToString(),
                        PluginName = provider["PluginName"].ToString(),
                        ExecuteTotalCount = Convert.ToInt32(provider["ExecuteTotalCount"]?.ToString()),
                        ExecuteExceptionCount = Convert.ToInt32(provider["ExecuteExceptionCount"]?.ToString()),
                        ExecuteMinDuration = Convert.ToInt64(provider["ExecuteMinDuration"]?.ToString()),
                        ExecuteAvgDuration = Convert.ToInt64(provider["ExecuteAvgDuration"]?.ToString()),
                        ExecuteMaxDuration = Convert.ToInt64(provider["ExecuteMaxDuration"]?.ToString())
                    };

                }
                stopwatch.Stop();
                TimeSpan ts = stopwatch.Elapsed;
                string elaspedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                                    ts.Hours, ts.Minutes, ts.Seconds,
                                                    ts.Milliseconds);
                Console.WriteLine($"Searching and casting {InsertTestCount} metrics execute result takes {elaspedTime} ");
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed to cast the record");
            }
        }
    }

    class PluginExecuteMetrics
    {
        public string PluginId { get; set; }
        public string PluginName { get; set; }
        public int ExecuteTotalCount { get; set; }
        public int ExecuteExceptionCount { get; set; }
        public long ExecuteMinDuration { get; set; }
        public long ExecuteAvgDuration { get; set; }
        public long ExecuteMaxDuration { get; set; }

        public override string ToString() => this.PluginId+this.PluginName+this.ExecuteTotalCount;

    }
}
