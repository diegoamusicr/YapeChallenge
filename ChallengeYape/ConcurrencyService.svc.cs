using System;
using System.Collections.Generic;
using System.IO;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ChallengeYape
{
    public class ConcurrencyService : IConcurrencyService
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ConcurrencyService));

        public const string INTERNAL_ROUTE = @"C:\RutaInterna1";

        // TODO: reemplazar por una base de datos relacional
        private readonly Dictionary<string, Dictionary<string, double>> _exchangeRates = new Dictionary<string, Dictionary<string, double>>()
        {
            {"sol", new Dictionary<string, double>() {
                { "dolar", 0.27 },
                { "euro", 0.25 },
                { "sol", 1.0 }
            }},
            {"dolar", new Dictionary<string, double>() {
                { "sol", 3.71 },
                { "euro", 0.92 },
                { "dolar", 1.0 }
            }},
            {"euro", new Dictionary<string, double>() {
                { "sol", 4.04 },
                { "dolar", 1.08 },
                { "euro", 1.0 }
            }},
        };
        
        public ConversionResult ConvertCurrency(double amount, string sourceCurrency, string targetCurrency)
        {

            if (_exchangeRates.TryGetValue(sourceCurrency, out Dictionary<string, double> sourceExchangeRate))
            {
                if (sourceExchangeRate.TryGetValue(targetCurrency, out double exchangeRate))
                {
                    return new ConversionResult
                    {
                        Amount = amount,
                        ConvertedAmount = amount * exchangeRate,
                        SourceCurrency = sourceCurrency,
                        TargetCurrency = targetCurrency,
                        ExchangeRate = exchangeRate
                    };
                }
                _logger.Debug("Invalid target currency requested");
                throw new Exception("Moneda destino invalida");
            }
            _logger.Debug("Invalid source currency requested");
            throw new Exception("Moneda origen invalida");      
        }

        public UploadResult UploadImage(Stream image)
        {
            try
            {
                if (!Directory.Exists(INTERNAL_ROUTE))
                {
                    Directory.CreateDirectory(INTERNAL_ROUTE);
                }
                string path = Path.Combine(INTERNAL_ROUTE, Guid.NewGuid().ToString() + ".jpg");
                using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    _logger.Debug($"Writing new image to {path}");
                    image.CopyTo(fileStream);
                }
                return new UploadResult { Success = true, Message = "Imagen subida correctamente" };
            }
            catch (Exception ex)
            {
                _logger.Error("Error while writing image file", ex);
                return new UploadResult { Success = false, Message = $"Error: {ex.Message}" };
            }
        }
    }
}
