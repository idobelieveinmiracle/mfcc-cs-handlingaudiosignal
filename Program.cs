using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Accord.Audio;
using Accord.Math;

using System.Numerics;
using System.Data.SqlClient;

namespace TestSignal
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection conn = DBUtils.GetDBConnection();

            try
            {
                Console.WriteLine("Openning Connection ...");

                conn.Open();

                Record record = new Record(@"C:\Users\Admin\Downloads\Music\2.wav", "record 2");

                record.saveToDatabase(conn);

                Console.WriteLine("Done.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            Console.ReadKey();
        }
    }
}
