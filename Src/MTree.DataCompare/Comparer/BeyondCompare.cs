using MTree.Configuration;
using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataCompare
{
    public class BeyondCompare
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public string ResultOutput { get; set; }

        private string beyondComparePath = Config.Compare.BeyondComparePath;

        private string sourceFile = "source.txt";
        private string destinationFile = "destination.txt";

        public bool DoCompareItem(List<string> src, List<string> dest)
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

            return DoCompareItem(srcString.ToString(), destString.ToString());
        }
        
        public bool DoCompareItem(List<Subscribable> src, List<Subscribable> dest)
        {
            if (File.Exists(beyondComparePath) == false)
            {
                logger.Error("Beyond compare path is wrong");
                return false;
            }

            using (StreamWriter sourceSw = new StreamWriter(sourceFile, false))
            {
                foreach (Subscribable s in src)
                {
                    sourceSw.WriteLine(s.ToString());
                }
                sourceSw.Flush();
            }

            using (StreamWriter destinationSw = new StreamWriter(destinationFile, false))
            {
                foreach (Subscribable s in dest)
                {
                    destinationSw.WriteLine(s.ToString());
                }
                destinationSw.Flush();
            }

            Process compareProcess = Process.Start(beyondComparePath, $"/qc=binary /silent {sourceFile} {destinationFile}");
            compareProcess.WaitForExit();

            if (File.Exists(sourceFile))
                File.Delete(sourceFile);
            if (File.Exists(destinationFile))
                File.Delete(destinationFile);

            return compareProcess.ExitCode == 1;
        }

        public bool DoCompareItem(Subscribable src, Subscribable dest)
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

            return DoCompareItem(src.ToString(), dest.ToString());
        }

        public bool DoCompareItem(string src, string dest)
        {
            if (File.Exists(beyondComparePath) == false)
            {
                logger.Error("Beyond compare path is wrong");
                return false;
            }

            using (StreamWriter sourceSw = new StreamWriter(sourceFile, false))
            {
                sourceSw.WriteLine(src);
                sourceSw.Flush();
            }

            using (StreamWriter destinationSw = new StreamWriter(destinationFile, false))
            {
                destinationSw.WriteLine(dest);
                destinationSw.Flush();
            }

            Process compareProcess = Process.Start(beyondComparePath, $"/qc=binary /silent {sourceFile} {destinationFile}");
            compareProcess.WaitForExit();

            if (File.Exists(sourceFile))
                File.Delete(sourceFile);
            if (File.Exists(destinationFile))
                File.Delete(destinationFile);

            logger.Info($"Comparing complete result:{compareProcess.ExitCode}");
            return compareProcess.ExitCode == 1;
        }
    }
}
