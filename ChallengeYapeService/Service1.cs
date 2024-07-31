using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using log4net;

namespace ChallengeYapeService
{
    public partial class Service1 : ServiceBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Service1));

        private const string INTERNAL_ROUTE_1 = @"C:\RutaInterna1\";
        private const string INTERNAL_ROUTE_2 = @"C:\RutaInterna2\";
        private Timer _timer;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            
            _timer = new Timer(120000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _logger.Debug("Starting copy evaluation");
            if (!Directory.Exists(INTERNAL_ROUTE_2))
            {
                _logger.Debug($"Target directory not found, creating a new one in path: {INTERNAL_ROUTE_2}");
                Directory.CreateDirectory(INTERNAL_ROUTE_2);
            }

            if (!Directory.Exists(INTERNAL_ROUTE_1))
            {
                _logger.Debug($"Source directory {INTERNAL_ROUTE_1} not found, nothing to copy");
                return;
            }

            foreach (var file in Directory.GetFiles(INTERNAL_ROUTE_1))
            {
                Task.Run(() => ProcessFile(file, INTERNAL_ROUTE_2)).Wait();
            }
        }

        //TODO: usar Azure Key Vault o similar para almacenar el key de encriptacion
        private void ProcessFile(string file, string targetPath)
        {
            _logger.Debug($"Copying file {file} to {targetPath}");
            string encriptedFile = Path.Combine(targetPath, Path.GetFileName(file) + ".enc");
            using (FileStream inputFileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (FileStream outputFileStream = new FileStream(encriptedFile, FileMode.Create, FileAccess.Write))
            using (var cryptoStream = new CryptoStream(outputFileStream, new TripleDESCryptoServiceProvider().CreateEncryptor(), CryptoStreamMode.Write))
            {
                inputFileStream.CopyTo(cryptoStream);
            }
            _logger.Debug($"Finished copying file {file} to {targetPath}, waiting an extra 15 seconds");
            Task.Delay(15000);
            _logger.Debug($"Finished processing file {file}");
        }

        protected override void OnStop()
        {
            _timer.Stop();
        }
    }
}
