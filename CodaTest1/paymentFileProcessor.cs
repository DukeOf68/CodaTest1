//#r "System.Configuration"
//#r "System.Data"

using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;


using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace CodaTest1
{
    public static class paymentFileProcessor
    {
        [FunctionName("paymentFileProcessor")]
        public static async Task Run(
            [BlobTrigger("cehcontainer1/CODA01{inFilename}")]
            Stream inFileBlob,
            string inFilename,

            TraceWriter log)
        {



            //CURRENTLY....        react to CODA01* arriving in cehcontainer1
            //                              establish connection with codaTest1 database 
            //                              read table lkp_codacodes
            //                              put out log entry per row read - giving the field data.



            log.Info($"C# Blob trigger function Processed blob\n Name:{inFilename} \n Size: {inFileBlob.Length} Bytes");

            var reader = await new myDal().getCodaCodes();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string policyNumber = reader.GetString(1);
                    string codeCode = reader.GetString(2);

                    string str = $"C# Blob trigger function read db lkp_codacodes data\n id:" + id + "    Policy:" + policyNumber + "    CodeCode: " + codeCode + "\n  ";
                    log.Info(str);
                }
            }
            else
            {
                    log.Info("No Data Read From lkp_codacodes table.");
            }
            reader.Close();



        }
    }


    internal class myDal
    {

        public async Task<SqlDataReader> getCodaCodes()
        {
            var str = "Server=tcp:codatest1.database.windows.net,1433;Initial Catalog=CodaTest1Db;Persist Security Info=False;User ID={codaTest1Admin};Password={spikeAdmin09.};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            using (SqlConnection conn = new SqlConnection(str))
            {
                conn.Open();
                var text = "select * from lkp_codacodes;";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    // Execute the command and log the # rows affected.
                    var rows = await cmd.ExecuteReaderAsync();

                    return rows;

                }
            }
        }
    }

}
