using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net;
using System.IO.Compression;

using CefAdapter.Dependencies;

namespace CefAdapter
{
    public class DependenciesInstaller
    {
        private readonly List<IDependency> _dependencies;

        public DependenciesInstaller(string dependenciesDirectory = "./Dependencies")
        {
            var installDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            _dependencies = new List<IDependency>();
            _dependencies.Add(new CefAdapterDependency(Path.Combine(dependenciesDirectory, "CefAdapter"), installDirectory));
        }

        public bool CheckDependencies(bool downloadIfNeeded = true)
        {
            foreach (var item in _dependencies)
            {
                try 
                {
                    if (!CheckDependency(item, downloadIfNeeded))
                    {
                        return false;
                    }
                }
                catch(Exception)
                {
                    return false;
                }
            }   

            return true;
        }  

        private bool CheckDependency(IDependency dependency, bool downloadIfNeeded)
        {
            if (dependency.IsInstalled())
            {
                return true;
            }

            if (!dependency.IsDownloaded())
            {
                if (!downloadIfNeeded)
                {
                    return false;
                }

                dependency.Download();
            }

            dependency.Install();

            return true;
        }      
    }
}
    