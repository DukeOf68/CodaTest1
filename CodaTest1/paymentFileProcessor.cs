using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace CodaTest1
{
    public static class paymentFileProcessor
    {
        [FunctionName("paymentFileProcessor")]
        public static void Run(
            [BlobTrigger("cehcontainer1/CODA01{inFilename}")]
            Stream inFileBlob, 
            string inFilename, 
            
            TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{inFilename} \n Size: {inFileBlob.Length} Bytes");
        }
    }
}
