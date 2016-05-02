using MTree.Configuration;
using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataValidator
{
    public class BeyondCompare
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public string ResultOutput { get; set; }

        private string beyondComparePath = Config.Compare.BeyondComparePath;

        private const string tempPath = "Temp";
        private const string logBasePath = "Logs";
        private const string compareResultPath = "CompareResult";
        private const string codeDifferentPath = "DifferentRaw";

        public bool DoCompareItem(List<string> src, List<string> dest, bool showWindow = false)
        {
            StringBuilder srcString = new StringBuilder();
            StringBuilder destString = new StringBuilder();

            foreach (string s in src)
            {
                srcString.AppendLine(s);
            }

            foreach (string d in dest)
            {
                destString.AppendLine(d);
            }

            return DoCompareItem(srcString.ToString(), destString.ToString(), showWindow);
        }
        
        public bool DoCompareItem(List<Subscribable> src, List<Subscribable> dest, bool showWindow = false)
        {
            StringBuilder srcString = new StringBuilder();
            StringBuilder destString = new StringBuilder();

            foreach (Subscribable s in src)
            {
                srcString.AppendLine(s.ToString(nameof(s.Id), nameof(s.ReceivedTime)));
            }

            foreach (Subscribable d in dest)
            {
                destString.AppendLine(d.ToString(nameof(d.Id), nameof(d.ReceivedTime)));
            }

            return DoCompareItem(srcString.ToString(), destString.ToString(), showWindow);
        }

        public bool DoCompareItem(Subscribable src, Subscribable dest, bool showWindow = false)
        {
            if (src == null && dest == null)
            {
                logger.Info("Source and Destination data is null. result is true");
                return true;
            }

            if (src == null || dest == null)
            {
                logger.Info("Source or Destination data is null. result is false");
                return false;
            }

            return DoCompareItem(src.ToString(), dest.ToString(), showWindow);
        }

        public bool DoCompareItem(string src, string dest, bool showWindow = false)
        {
            if (File.Exists(beyondComparePath) == false)
            {
                logger.Error("Beyond compare path is wrong");
                return false;
            }

            if (Directory.Exists(tempPath) == false)
            {
                Directory.CreateDirectory(tempPath);
            }

            string sourceFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
            using (StreamWriter sourceSw = new StreamWriter(sourceFile, false))
            {
                sourceSw.WriteLine(src);
                sourceSw.Flush();
            }
            string destinationFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
            using (StreamWriter destinationSw = new StreamWriter(destinationFile, false))
            {
                destinationSw.WriteLine(dest);
                destinationSw.Flush();
            }

            string param = $"{sourceFile} {destinationFile}";
            if (showWindow == false)
            {
                param = "/qc=binary /silent " + param;
            }

            Process compareProcess = Process.Start(beyondComparePath, param);
            compareProcess.WaitForExit();

            if (File.Exists(sourceFile))
                File.Delete(sourceFile);
            if (File.Exists(destinationFile))
                File.Delete(destinationFile);

            return compareProcess.ExitCode == 1;
        }

        public void MakeReport(List<string> src, List<string> dest, string reportPath)
        {
            StringBuilder srcString = new StringBuilder();
            StringBuilder destString = new StringBuilder();

            foreach (string s in src)
            {
                srcString.AppendLine(s);
            }

            foreach (string d in dest)
            {
                destString.AppendLine(d);
            }

            MakeReport(srcString.ToString(), destString.ToString(), reportPath);
        }

        public void MakeReport(List<Subscribable> src, List<Subscribable> dest, string reportPath)
        {
            StringBuilder srcString = new StringBuilder();
            StringBuilder destString = new StringBuilder();

            StringBuilder outputString = new StringBuilder();

            foreach (Subscribable s in src)
            {
                srcString.AppendLine(s.ToString(nameof(s.Id), nameof(s.ReceivedTime)));
                outputString.AppendLine(s.ToString());
            }

            string path = Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, codeDifferentPath);
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter sw = new StreamWriter(Path.Combine(path, src[0].Code + "_source.txt"), false))
            {
                sw.WriteLine(outputString);
                sw.Flush();
            }

            outputString.Clear();
            foreach (Subscribable d in dest)
            {
                destString.AppendLine(d.ToString(nameof(d.Id), nameof(d.ReceivedTime)));
                outputString.AppendLine(d.ToString());
            }

            path = Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, codeDifferentPath);
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter sw = new StreamWriter(Path.Combine(path, dest[0].Code + "_destination.txt"), false))
            {
                sw.WriteLine(outputString);
                sw.Flush();
            }

            MakeReport(srcString.ToString(), destString.ToString(), reportPath);
        }

        public void MakeReport(string src, string dest, string reportPath)
        {
            if (File.Exists(beyondComparePath) == false)
            {
                logger.Error("Beyond compare path is wrong");
                return;
            }

            string sourceFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
            using (StreamWriter sourceSw = new StreamWriter(sourceFile, false))
            {
                sourceSw.WriteLine(src);
                sourceSw.Flush();
            }

            string destinationFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
            using (StreamWriter destinationSw = new StreamWriter(destinationFile, false))
            {
                destinationSw.WriteLine(dest);
                destinationSw.Flush();
            }

            string param = $"/qc=binary /silent @CompareScript.txt {sourceFile} {destinationFile} {reportPath}";

            Process compareProcess = Process.Start(beyondComparePath, param);
            compareProcess.WaitForExit();

            if (File.Exists(sourceFile))
                File.Delete(sourceFile);
            if (File.Exists(destinationFile))
                File.Delete(destinationFile);

            logger.Info($"Compare result report created at {reportPath}");
        }
    }
}
