using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace CodaTest1
{
    public static class paymentsWithDbCodaCodes
    {
        [FunctionName("paymentsWithDbCodaCodes")]
        public static async Task Run(

                    [BlobTrigger("pm-func-demo2-1/LocalPM{name}")] Stream PmInFile,
                    string name,

                    [Blob("pm-func-demo2-2/LocalOUT{name}", FileAccess.Write)] Stream PmCmOutFile,

                    TraceWriter log)

        {

            IMyLogger loggit = new MyLogger(log);

            loggit.log("Start");
            loggit.log($"     C# Blob trigger function Processed blob\n Name:{name} \n Size: {PmInFile.Length} Bytes");

            //the Coda codes...

            //stubbed out for local development...
            //List<CodaCode> codes = await CodaCodes(log);
            List<CodaCode> codes = (new SqlDbStub()).CodaCodes(log);
            loggit.log("     Coda Mappings Loaded");

            //the Processor....
            PaymentProcessor theProcessor = new PaymentProcessor(loggit);

            //process...
            theProcessor.Process(PmInFile, PmCmOutFile, codes);
            loggit.log("Finish");

        }
                 
        //todo - accessibilty - privates below here.
        //todo - inject XmlWriter? (dunno for a func tbh)

        //public static async Task<List<CodaCode>> CodaCodes(TraceWriter log)
        //{

        //    List<CodaCode> codes = new List<CodaCode>();

        //    var str = ConfigurationManager.ConnectionStrings["sqldb_connection"].ConnectionString;
        //    using (SqlConnection conn = new SqlConnection(str))
        //    {
        //        conn.Open();
        //        var text = "select id,policyNumber,codaCode from lkp_codacodes;";

        //        using (SqlCommand cmd = new SqlCommand(text, conn))
        //        {
        //            // Execute the command and log the # rows affected.
        //            var reader = await cmd.ExecuteReaderAsync();

        //            int ctr = 0;

        //            // log.Info($"     coda mapping rows read from db : {reader.recordsAffected}");
        //            if (reader.HasRows)
        //            {

        //                while (reader.Read())
        //                {
        //                    ctr++;

        //                    // IDataRecord singlerow = CType(reader,IDataRecord);

        //                    var id = reader.GetInt32(0);
        //                    // var id = reader["id"];

        //                    string policyNumber = reader.GetString(1);
        //                    string codaCode = reader.GetString(2);

        //                    CodaCode cc = new CodaCode() { Id = id, PolicyNumber = policyNumber, Coda = codaCode };
        //                    codes.Add(cc);

        //                }
        //            }

        //            reader.Close();
        //            log.Info($"     Coda mapping rows loaded : {ctr}");
        //        }
        //    }

        //    return codes;
        //}

        public class PaymentProcessor
        {
            private readonly IMyLogger _log;

            public PaymentProcessor(IMyLogger log)      //interfaces -   ITraceWriter?
            {
                _log = log;
            }

            public void Process(Stream pmInFile, Stream pmCmOutFile, List<CodaCode> codes)
            {

                using (StreamReader sr = new StreamReader(pmInFile))
                {

                    // xmloutputgenerator - ideally a dependency? but wth.
                    using (var xmlOutputGenerator = new XmlOutputGenerator(pmCmOutFile))
                    {
                        // //         // using (StreamWriter sw = new StreamWriter(PmCmOutFile))
                        // //          // {
                        string paymentLine;
                        int ctr = 0;
                        while ((paymentLine = sr.ReadLine()) != null)
                        {
                            ctr++;
                            //split the line
                            string[] fields = paymentLine.Split(',');

                            //lift necessary fields from payment file record
                            string recordType = fields[0];


                            // // //valid line?
                            if ("23456".Contains(recordType))
                            {
                                string policyNumber = fields[4];


                                //get codacode from collection
                                CodaCode theCode = codes.FirstOrDefault(x => x.PolicyNumber == policyNumber);
                                if (theCode != null)
                                {
                                    //create processedPayment object  

                                    paymentLine = paymentLine + "," + theCode.Coda;


                                }
                                else
                                {
                                    //no coda code found in lookup
                                    paymentLine = paymentLine + ", CODA CODE NOT FOUND";
                                }
                            }

                            //WRITE XML OUTPUT - 
                            xmlOutputGenerator.WriteLine(paymentLine);

                            // _log.Info($"processed payment = {paymentLine}");

                        }
                        _log.log($"     Payment lines processed     : {ctr.ToString()}");
                    }
                }
            }
        }

        public class XmlOutputGenerator : IDisposable
        {
            XmlWriter xr;
            private readonly Stream paymentsOut;

            public XmlOutputGenerator(Stream paymentsOut)
            {
                this.paymentsOut = paymentsOut;
                xr = XmlWriter.Create(paymentsOut);
                WriteStart();
            }

            public void WriteLine(string someValue)
            {
                xr.WriteStartElement("Line");

                xr.WriteElementString("SomeValue", someValue);
                xr.WriteElementString("Number", "1");
                xr.WriteElementString("AccountCode", "421125.D99.PL99.PT99");
                xr.WriteElementString("DocValue", "24789.03");
                xr.WriteElementString("LineSense", "credit");
                xr.WriteElementString("LineType", "analysis");
                xr.WriteElementString("Description", "Annuity I/F TPA File PAYM 6132");
                xr.WriteElementString("ExtRef3", "");
                xr.WriteElementString("ExtRef4", "nT TPA File PAYM 6132");
                xr.WriteElementString("ExtRef5", "SAID 2302446");
                xr.WriteElementString("ExtRef6", "nTU Denis Flanagan");

                xr.WriteEndElement(); // Line
            }

            private void WriteStart()
            {
                xr.WriteStartDocument();

                xr.WriteStartElement("InputRequest");
                xr.WriteAttributeString("version", "11.3");

                xr.WriteStartElement("Post");
                xr.WriteAttributeString("postto", "intray");

                xr.WriteStartElement("Request");
                xr.WriteStartElement("Transaction");

                xr.WriteStartElement("Header");

                xr.WriteStartElement("Key");
                xr.WriteElementString("CmpCode", "PUKOPLACLPH");
                xr.WriteElementString("Code", "INJNANN");
                xr.WriteElementString("Number", "528789");
                xr.WriteEndElement(); // Key

                xr.WriteElementString("Period", "2017/10");
                xr.WriteElementString("CurCode", "GBP");
                xr.WriteElementString("Date", "2017-10-19");
                xr.WriteElementString("Description", "Annuity I/F TPA File PAYM 6132");

                xr.WriteEndElement(); // Header

                xr.WriteStartElement("Lines");
            }

            private void WriteEnd()
            {
                xr.WriteEndElement(); // Lines

                xr.WriteEndElement(); // Transaction
                xr.WriteEndElement(); // Request
                xr.WriteEndElement(); // Post
                xr.WriteEndElement(); // InputRequest

                xr.WriteEndDocument();
            }

            public void Dispose()
            {
                WriteEnd();
                xr.Close();
                xr.Dispose();
            }
        }
    }

    #region "logic classes"
    internal class SqlDbStub
    {
        public List<CodaCode> CodaCodes(TraceWriter log)
        {

            List<CodaCode> codes = new List<CodaCode>();

            int id;
            string policyNumber;
            string codaCode;

            try
            {
                //Pass the file path and file name to the StreamReader constructor
                using (StreamReader sr = new StreamReader("C:/temp/loadCodaMappings.csv"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] field = line.Split(',');

                        id = Convert.ToInt32(field[0]);
                        policyNumber = field[1];
                        codaCode = field[2];

                        CodaCode cc = new CodaCode() { Id = id, PolicyNumber = policyNumber, Coda = codaCode };
                        codes.Add(cc);

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            //finally
            //{
            //    Console.WriteLine("Executing finally block.");
            //}


            return codes;
        }

    }
    public interface IMyLogger
    {
        void log(string str);
    }
    internal class MyLogger : IMyLogger
    {
        private TraceWriter _log;
        internal MyLogger(TraceWriter log)
        {
            _log = log;
        }
        void IMyLogger.log(string str)
        {
            string message = str + "    :" + DateTime.Now.ToString("h:mm:ss tt");
            _log.Info(message);
        }
    }
    #endregion


    #region "data classes"
    public class CodaCode
    {
        internal int Id { get; set; }
        internal string PolicyNumber { get; set; }
        internal string Coda { get; set; }
    }
    #endregion

}
