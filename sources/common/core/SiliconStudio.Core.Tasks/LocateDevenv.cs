// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.VisualStudio.Setup.Configuration;

namespace SiliconStudio.Core.Tasks
{
    public class LocateDevenv : Task
    {
        [Output]
        public String DevenvPath { get; set; }

        public override bool Execute()
        {
            DevenvPath = FindDevenv(null);
            return DevenvPath != null;
        }

        internal static string FindDevenv(string msbuildPath)
        {
            try
            {
                // Use Microsoft.VisualStudio.Setup.Configuration.Interop (works when running from Visual Studio)
                var setupConfiguration = new SetupConfiguration() as ISetupConfiguration;
                ISetupInstance instanceForCurrentProcess = !string.IsNullOrEmpty(msbuildPath)
                    ? setupConfiguration.GetInstanceForPath(msbuildPath)
                    : setupConfiguration.GetInstanceForCurrentProcess(); // Works when ran as MSBuild Task only
                return Path.Combine(instanceForCurrentProcess.GetInstallationPath(), "Common7\\IDE\\devenv.exe");
            }
            catch
            {
                return null;
            }
        }
    }
}
