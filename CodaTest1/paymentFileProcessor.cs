using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace CodaTest1
{
    //public static class paymentFileProcessor
    //{
    //    [FunctionName("paymentFileProcessor")]
    //    public static async Task Run(
    //        [BlobTrigger("cehcontainer1/CODA01{inFilename}")]
    //        Stream inFileBlob,
    //        string inFilename,

    //        TraceWriter log)
    //    {



    //        //CURRENTLY....        react to CODA01* arriving in cehcontainer1
    //        //                              establish connection with codaTest1 database 
    //        //                              read table lkp_codacodes
    //        //                              put out log entry per row read - giving the field data.



    //        log.Info($"C# Blob trigger function Processed blob\n Name:{inFilename} \n Size: {inFileBlob.Length} Bytes");

    //        //var reader = await new myDal().getCodaCodes();

    //        //if (reader.HasRows)
    //        //{
    //        //    while (reader.Read())
    //        //    {
    //        //        int id = reader.GetInt32(0);
    //        //        string policyNumber = reader.GetString(1);
    //        //        string codaCode = reader.GetString(2);

    //        //        string str = $"C# Blob trigger function read db lkp_codacodes data\n id:" + id + "    Policy:" + policyNumber + "    CodeCode: " + codaCode + "\n  ";
    //        //        log.Info(str);
    //        //    }
    //        //}
    //        //else
    //        //{
    //        //        log.Info("No Data Read From lkp_codacodes table.");
    //        //}
    //        //reader.Close();



    //    //}
    //}


    //internal class myDal
    //{

    //    //public async Task<SqlDataReader> getCodaCodes()
    //    public async List<string> getCodaCodes()
    //    {
    //        var str = "Server=tcp:codatest1.database.windows.net,1433;Initial Catalog=CodaTest1Db;Persist Security Info=False;User ID={codaTest1Admin};Password={spikeAdmin09.};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    //        using (SqlConnection conn = new SqlConnection(str))
    //        {
    //            conn.Open();
    //            var text = "select * from lkp_codacodes;";

    //            using (SqlCommand cmd = new SqlCommand(text, conn))
    //            {
    //                // Execute the command and log the # rows affected.
    //                var rows = await cmd.ExecuteReaderAsync();

    //                return rows;

    //            }
    //        }
    //    }
    //}






    //internal class PaymentProcessor
    //{
    //    private readonly TraceWriter _log;
    //    //public PaymentProcessor(JhXmlWriter writer, TraceWriter log)      //interfaces -   ITraceWriter?
    //    public PaymentProcessor(TraceWriter log)      //interfaces -   ITraceWriter?
    //    {
    //        _log = log;
    //    }
    //    //***************
    //    //todo : construct with xml doc writer dependency and log writer 
    //    //***************


    //    // internal void Process(Stream PmInFile, Stream CmInFile, Stream PmCmOutFile, string filename)

    //    internal void Process(Stream pmInFile, List<CodaCode> codes)
    //    {

    //        ProcessedPayment pp;

    //        IList<string> dssd;
    //        IList<String> dddd;

    //        string paymentLinex="jj";

    //        string[] fieldsss = paymentLinex.Split(',');
    //        string cvb = fieldsss[0];





    //        using (StreamReader sr = new StreamReader(pmInFile))
    //        {
    //            // using (StreamWriter sw = new StreamWriter(PmCmOutFile))
    //            // {
    //            string paymentLine;
    //            while ((paymentLine = sr.ReadLine()) != null)
    //            {

    //                //split the line
    //                IList<String> fields = paymentLine.Split(',');

    //                //lift necessary fields from payment file record
    //                string recordType = fields[0];
    //                string policyNumber = fields[4];


    //                //valid line?
    //                if ("23456".Contains(recordType))
    //                {

    //                    //get codacode from collection
    //                    CodaCode theCode = codes.FirstOrDefault(x => x.PolicyNumber == policyNumber);
    //                    if (theCode != null)
    //                    {
    //                        //create processedPayment object  

    //                        paymentLine = paymentLine + "," + theCode.Coda;
                           

    //                    }
    //                    else
    //                    {
    //                        //no coda code found in lookup
    //                        paymentLine = paymentLine + ", CODA CODE NOT FOUND";
    //                    }
    //                }

    //                _log.Info($"processed payment = {paymentLine}");

    //                //line goes to XmlDoc here

    //                //xmlWriter.writeline(pp.PolicyNumber,pp.CodaCode)

    //            }
    //        }
    //    }
    //}

    //internal class ProcessedPayment
    //{
    //    // internal int Id { get; set; }
    //    internal string PolicyNumber { get; set; }
    //    internal string CodaCode { get; set; }
    //}
    //internal class CodaCode
    //{
    //    internal int Id { get; set; }
    //    internal string PolicyNumber { get; set; }
    //    internal string Coda { get; set; }
    //}


}
