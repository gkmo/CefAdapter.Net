using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net;
using System.IO.Compression;

namespace CefAdapter.Dependencies
{
    class CefAdapterDependency : IDependency
    {
        private const string BASE_CEFADAPTER_ADDRESS = "https://github.com/gkmo/CefAdapter/releases/download/v0.0.1-alpha/";
        private const string WINDOWS_BINARY_FILE = "cefadapter_binary_0.0.1_windows32.zip";
        private const string LINUX_BINARY_FILE = "cefadapter_binary_0.0.1_linux64.zip";    

        private readonly string _downloadDirectory;
        private readonly string _installDirectory;
        private readonly string _executableName;
        private readonly string _zipFilename;

        public CefAdapterDependency(string downloadDirectory, string installDirectory)
        {
            _downloadDirectory = downloadDirectory;
            _installDirectory = installDirectory;

            _zipFilename = LINUX_BINARY_FILE;

            var exeExtension = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                exeExtension = ".exe";
                _zipFilename = WINDOWS_BINARY_FILE;
            }

            _executableName = "CefAdapter" + exeExtension;            
        }        

        public string Name 
        { 
            get { return "CefAdapter"; }            
        }
        
        public bool IsInstalled()
        {        
            return File.Exists(CefAdapterExecutableFullPath);            
        }

        public bool IsDownloaded()
        {
            return File.Exists(ZipFullPath);
        }

        public void Download()
        {
            var webClient = new WebClient
            {
                BaseAddress = BASE_CEFADAPTER_ADDRESS
            };
            
            if (!Directory.Exists(_downloadDirectory))
            {
                Directory.CreateDirectory(_downloadDirectory);
            }

            webClient.DownloadFile(_zipFilename, ZipFullPath);
        }

        public void Install()
        {
            ZipFile.ExtractToDirectory(ZipFullPath, _installDirectory);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {                     
                ExecuteLinuxCommand($"chmod a+x {CefAdapterExecutableFullPath}");                    
            }
        }

        private string ZipFullPath
        {
            get 
            {
                return Path.Combine(_downloadDirectory, _zipFilename);
            }
        }        

        private string CefAdapterExecutableFullPath
        {
            get 
            {
                return Path.Combine(_installDirectory, _executableName);
            }
        }

        private static string ExecuteLinuxCommand(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }  
    }
}