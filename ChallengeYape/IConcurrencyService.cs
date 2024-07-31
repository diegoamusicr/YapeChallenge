using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ChallengeYape
{

    [ServiceContract]
    public interface IConcurrencyService
    {

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "convert?amount={amount}&sourceCurrency={sourceCurrency}&targetCurrency={targetCurrency}")]
        ConversionResult ConvertCurrency(double amount, string sourceCurrency, string targetCurrency);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "upload", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        UploadResult UploadImage(Stream image);
    }

    public class ConversionResult
    {
        public double Amount { get; set; }
        public double ConvertedAmount { get; set; }
        public string SourceCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public double ExchangeRate { get; set; }
    }

    public class UploadResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

}
