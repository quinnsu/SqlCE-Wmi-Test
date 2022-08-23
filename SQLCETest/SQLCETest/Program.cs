using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;
using System.IO;
using System.Diagnostics;

namespace SQLCETest
{
    class Program
    {
        const int InsertTestCount = 100;
        static void Main(string[] args)
        {
            //create null connection
            SqlCeConnection conn = null;

            string databaseName = "TestSQLCE.sdf";
            if (File.Exists(databaseName))
                File.Delete(databaseName);

            //create database file
            SqlCeEngine eng = new SqlCeEngine($"Data Source='{databaseName}'");
            eng.CreateDatabase();

            try
            {
                //connect to the database
                conn = new SqlCeConnection($"Data Source='{databaseName}'");
                conn.Open();

                //CREATE TABLE
                SqlCeCommand cmd = conn.CreateCommand();
                cmd.CommandText = "CREATE TABLE TestTbl(Pluginid NVARCHAR(100) PRIMARY KEY, PluginName NVARCHAR(100), ExecuteTotalCount INT, ExecuteExceptionCount INT,ExecuteMinDuration BIGINT, ExecuteAvgDuration BIGINT, ExecuteMaxDuration BIGINT)";
                int affectRow = cmd.ExecuteNonQuery();

                Console.WriteLine($"affectRow: {affectRow}");

                //INSERT
                insertRecords(cmd,conn);
                //UPDATE
                updateRecords(cmd);
                //SEARCH
                SearchAndCastRecords(cmd);
                //DELETE
                deleteRecords(cmd);


                conn.Close();
            }
            catch (SqlCeException ex)
            {
                Console.WriteLine($"exception when execute {ex.Message} {ex.ToString()}");
                foreach (SqlCeError error in ex.Errors)
                {
                    Console.WriteLine(error.Message);
                }
            }
            finally
            {
                conn.Close();
                Console.WriteLine("Press any key to continue...");
            }
        }

        public static void insertRecords(SqlCeCommand cmd, SqlCeConnection conn)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < InsertTestCount; i++)
            {
                string pluginId = Guid.NewGuid().ToString();
                cmd = new SqlCeCommand($"INSERT INTO TestTbl  VALUES ('{pluginId}','test-name', 0 , 0 , 0 , 0 , 0)", conn);
                int affectRow = cmd.ExecuteNonQuery();
            }

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elaspedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                                ts.Hours, ts.Minutes, ts.Seconds,
                                                ts.Milliseconds);
            Console.WriteLine($"Inserting {InsertTestCount} metrics execute result takes {elaspedTime} ");

        }
        public static void deleteRecords(SqlCeCommand cmd)
        {
            //delete
            Stopwatch deletewatch = new Stopwatch();
            deletewatch.Start();
            cmd.CommandText = "DELETE FROM TestTbl WHERE PluginName = 'test-name'";
            int affectRow = cmd.ExecuteNonQuery();
            deletewatch.Stop();
            TimeSpan ts = deletewatch.Elapsed;
            string elaspedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                                    ts.Hours, ts.Minutes, ts.Seconds,
                                                    ts.Milliseconds);
            Console.WriteLine($"delete {affectRow} lines records from the table, takes {elaspedTime} ms");
        }
        public static void updateRecords(SqlCeCommand cmd)
        {
            //UPDATE
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            cmd.CommandText = "UPDATE TestTbl SET ExecuteTotalCount = 1, ExecuteExceptionCount = 1, ExecuteMinDuration = 1, ExecuteAvgDuration = 1, ExecuteMaxDuration = null WHERE PluginName = 'test-name'";
            int affectRow = cmd.ExecuteNonQuery();
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elaspedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                                    ts.Hours, ts.Minutes, ts.Seconds,
                                                    ts.Milliseconds);
            Console.WriteLine($"update {affectRow} lines records from the table, takes {elaspedTime} ms");

        }
        public static void SearchAndCastRecords(SqlCeCommand cmd)
        {
            //READ DATA
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            cmd.CommandText = ("SELECT * FROM TestTbl");
            SqlCeDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var metrics = new PluginExecuteMetrics
                {
                    PluginId = reader["PluginId"].ToString(),
                    PluginName = reader["PluginName"].ToString(),
                    ExecuteTotalCount = Convert.ToInt32(reader["ExecuteTotalCount"]),
                    ExecuteExceptionCount = Convert.ToInt32(reader["ExecuteExceptionCount"]),
                    ExecuteMinDuration = Convert.ToInt64(reader["ExecuteMinDuration"]),
                    ExecuteAvgDuration = Convert.ToInt64(reader["ExecuteAvgDuration"]),
                    ExecuteMaxDuration = Convert.ToInt64(reader["ExecuteMaxDuration"])
                };
            }
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elaspedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                                    ts.Hours, ts.Minutes, ts.Seconds,
                                                    ts.Milliseconds);
            Console.WriteLine($"search {InsertTestCount} lines records from the table, takes {elaspedTime} ms");
        }
        public static void AlterTable_add(SqlCeCommand cmd)
        {
            cmd.CommandText = ("ALTER TABLE TestTbl Add description NVARCHAR(100)");
            int affectRow = cmd.ExecuteNonQuery();
            Console.WriteLine($"{affectRow} rows have been affected.");
        }
        public static void AlterTable_drop(SqlCeCommand cmd)
        {
            cmd.CommandText = ("ALTER TABLE TestTbl DROP COLUMN description");
            int affectRow = cmd.ExecuteNonQuery();
            Console.WriteLine($"{affectRow} rows have been affected.");
        }
    }

    public class PluginExecuteMetrics
    {
        public string PluginId { get; set; }
        public string PluginName { get; set; }
        public int ExecuteTotalCount { get; set; }
        public int ExecuteExceptionCount { get; set; }
        public long ExecuteMinDuration { get; set; }
        public long ExecuteAvgDuration { get; set; }
        public long ExecuteMaxDuration { get; set; }

        public override string ToString() => this.PluginId + this.PluginName + this.ExecuteTotalCount;

    }
}
