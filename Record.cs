using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;
using Accord.Audio;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace TestSignal
{
    class Record
    {
        public int id { get; set; }
        public String label { get; set; }
        public String path { get; set; }
        public List<RecordFrame> frames { get; set; }

        // get record from file
        public Record(String path, String label)
        {
            WaveChannel32 wave = new WaveChannel32(new WaveFileReader(path));
            this.path = path;
            this.label = label;
            this.frames = new List<RecordFrame>();

            byte[] buffer = new byte[16384];
            int read = 0;
            List<double> listSamples = new List<double>();
            int sampleRate = 16000;

            while (wave.Position < wave.Length)
            {
                read = wave.Read(buffer, 0, 16384);
                for (int i = 0; i < read / 4; i++)
                {
                    double sample = BitConverter.ToSingle(buffer, i * 4);
                    listSamples.Add(sample);
                }
            }

            Signal signal = Signal.FromArray(listSamples.ToArray(), sampleRate);

            MelFrequencyCepstrumCoefficient mfcc = new MelFrequencyCepstrumCoefficient();
            IEnumerable<MelFrequencyCepstrumCoefficientDescriptor> features = mfcc.Transform(signal);
            foreach (var t in features)
            {
                RecordFrame recordFrame = new RecordFrame();
                recordFrame.coefficients = new List<double>(t.Descriptor);
                recordFrame.label = label;

                this.frames.Add(recordFrame);
            }
        }

        public void saveToDatabase(SqlConnection connection)
        {
            // Save record
            try
            {
                string sql = "Insert into Record (Label, FilePath) OUTPUT Inserted.ID "
                                                 + " values (@label, @filepath) ";

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.Parameters.Add("@label", SqlDbType.VarChar).Value = this.label;
                cmd.Parameters.Add("@filepath", SqlDbType.VarChar).Value = this.path;

                //cmd.ExecuteNonQuery();

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {

                        while (reader.Read())
                        {
                            this.id = reader.GetInt32(reader.GetOrdinal("ID"));
                        }
                    }
                }

                foreach (RecordFrame fr in this.frames)
                {
                    cmd = connection.CreateCommand();
                    cmd.CommandText = "Insert into RecordFrame (Label, RecordId) OUTPUT Inserted.ID "
                        + "values(@label, @recordid)";

                    cmd.Parameters.Add("@label", SqlDbType.VarChar).Value = fr.label;
                    cmd.Parameters.Add("@recordid", SqlDbType.Int).Value = this.id;

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {

                            while (reader.Read())
                            {
                                fr.id = reader.GetInt32(reader.GetOrdinal("ID"));
                            }
                        }
                    }

                    foreach (double d in fr.coefficients)
                    {
                        cmd = connection.CreateCommand();
                        cmd.CommandText = "Insert into Coefficient (Coefficient, FrameId) "
                            + "values(@coefficient, @frameid)";

                        cmd.Parameters.Add("@coefficient", SqlDbType.Float).Value = d;
                        cmd.Parameters.Add("@frameid", SqlDbType.Int).Value = fr.id;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                Console.WriteLine(e.StackTrace);
            }

            connection.Close();
        }
        
    }

    
}
