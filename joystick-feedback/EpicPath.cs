﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace joystick_feedback
{
    public static class EpicPath
    {
        public static string FindEpicEliteDirectory()
        {
            var epic64 = "SOFTWARE\\Wow6432Node\\Epic Games\\";
            var key64 = Registry.LocalMachine.OpenSubKey(epic64);

            if (key64 != null)
            {
                using (var subKey = key64.OpenSubKey("EpicGamesLauncher"))
                {
                    var epic64Path = subKey?.GetValue("AppDataPath").ToString() ?? "???";
                    if (Directory.Exists(epic64Path))
                    {
                        var config64Path = Path.Combine(epic64Path, "Manifests");
                        if (Directory.Exists(config64Path))
                        {
                            foreach (var file in Directory.EnumerateFiles(config64Path, "*.item"))
                            {
                                var o1 = JObject.Parse(File.ReadAllText(file));

                                var displayName = o1["DisplayName"]?.ToString() ?? "???";
                                var installLocation = o1["InstallLocation"]?.ToString() ?? "???";

                                App.Log.Info( "EPIC : " + displayName + " - " + installLocation);

                                if (displayName.ToLower().Contains("elite") &&
                                    displayName.ToLower().Contains("dangerous"))
                                {
                                    var controlSchemePath = Path.Combine(installLocation, @"Products\elite-dangerous-64\ControlSchemes\");

                                    if (Directory.Exists(controlSchemePath))
                                    {
                                        return controlSchemePath;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;

        }

    }
}
