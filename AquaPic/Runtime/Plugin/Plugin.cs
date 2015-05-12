﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace AquaPic.PluginRuntime
{
    public class Plugin
    {
        public static Dictionary<string, PluginScript> AllPlugins = new Dictionary<string, PluginScript> ();

        /* <TODO> I want to add some sort of json file adding plugins
         * Use that to control what plugins to load, and flags to set
         */
        public static void AddPlugins () {
            StringBuilder sb = new StringBuilder ();
            sb.Append (Environment.GetEnvironmentVariable ("AquaPic"));
            sb.Append (@"\AquaPicRuntimeProject\");
            var topPath = sb.ToString ();
            var files = Directory.GetFiles (topPath, "*.cs");

            foreach (var path in files) {
                string name = string.Empty;
                var idxBackslash = path.LastIndexOf ('\\') + 1;
                var idxPeriod = path.LastIndexOf ('.');
                if ((idxBackslash != -1) && (idxPeriod != -1))
                    name = path.Substring (idxBackslash, (idxPeriod - idxBackslash));

                //Console.WriteLine ("{0} at file path {1}", name, path);

                foreach (var line in File.ReadLines (path)) {
                    if (line.Contains ("IOutletScript")) {
                        AllPlugins.Add (name, new OutletScript (name, path));
                        AllPlugins [name].RunInitialize ();
                        break;
                    } else if (line.Contains ("IPluginScript")) {
                        AllPlugins.Add (name, new PluginScript (name, path));
                        AllPlugins [name].RunInitialize ();
                    }
                }
            }
        }

        public static void Run () {
            foreach (var p in AllPlugins.Values) {
                if (p.flags.HasFlag (PluginFlags.Cyclic)) {
                    p.RunPlugin ();
                }
            }
        }

        public static bool CompileCode (out Assembly pluginAssembly, string name, string sourceFileLocation) {
            CSharpCodeProvider provider = new CSharpCodeProvider ();
            CompilerParameters options = new CompilerParameters();

            options.GenerateExecutable = false; // create dll
            options.OutputAssembly = name + ".dll";
            options.GenerateInMemory = false;
            options.ReferencedAssemblies.Add (Assembly.GetExecutingAssembly ().Location);

            CompilerResults result = provider.CompileAssemblyFromFile(options, sourceFileLocation);

            if (result.Errors.HasErrors) {
                foreach (CompilerError error in result.Errors)
                    Console.WriteLine ("Error ({0}): {1}", error.ErrorNumber, error.ErrorText);

                pluginAssembly = null;
                return false;
            }

            pluginAssembly = Assembly.LoadFrom (name + ".dll");
            return true;
        }
    }
}

