using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObfuscarCLI
{
    internal class Program
    {
        public static void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
            using (BinaryReader r = new BinaryReader(s))
            using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
            using (BinaryWriter w = new BinaryWriter(fs))
                w.Write(r.ReadBytes((int)s.Length));
        }
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string a = Path.GetFileName(args[0]);
                string text2 = Path.GetTempPath() + "\\Obfuscation\\input\\" + a;

                
                if (!Directory.Exists(Path.GetTempPath() + "\\Obfuscation"))
                {
                    Extract("ObfuscarCLI", Path.GetTempPath(), "Resources", "Obfuscation.zip");
                    ZipFile.ExtractToDirectory(Path.GetTempPath() + "\\Obfuscation.zip", Path.GetTempPath());
                }
                File.Copy(args[0], text2);
                string addr = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Obfuscator>
	<Var name=""InPath"" value="".\input"" />
	<Var name=""OutPath"" value="".\output"" />
	<Var name=""RenameProperties"" value=""true"" />
	<Var name=""RenameEvents"" value=""true"" />
	<Var name=""RenameFields"" value=""true"" />
	<Var name=""KeepPublicApi"" value=""false"" />
	<Var name=""HidePrivateApi"" value=""true"" />
	<Var name=""UseUnicodeNames"" value=""true"" />
	<Var name=""HideStrings"" value=""true"" />
	<Var name=""OptimizeMethods"" value=""true"" />
	<Var name=""SuppressIldasm"" value=""true"" />
	<Module file=""$(InPath)\repme"" />
</Obfuscator>
";

                addr = addr.Replace("repme", a);

                File.WriteAllText($"{Path.GetTempPath()}\\Obfuscation\\obfuscar.xml", addr);
                Process p = new Process();
                p.StartInfo.FileName = $"cmd.exe";
                p.StartInfo.Arguments = $"/c Obfuscar.Console.exe obfuscar.xml";
                p.StartInfo.WorkingDirectory = $"{Path.GetTempPath()}\\Obfuscation";
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                p.Start();
                p.WaitForExit();

                File.Move(Path.GetTempPath() + "\\Obfuscation\\output\\" + a, Directory.GetCurrentDirectory() + "\\Secured.exe");

                File.Delete(Path.GetTempPath() + "\\Obfuscation\\input\\" + a);

                if (File.Exists(Path.GetTempPath() + "\\Obfuscation\\obfuscar.xml"))
                {
                    File.Delete(Path.GetTempPath() + "\\Obfuscation\\obfuscar.xml");
                }
                
            }
        }  
    }
}
