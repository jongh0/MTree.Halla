﻿using Configuration;
using DataStructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataValidator
{
    public class BeyondCompare
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string ResultOutput { get; set; }

        private string beyondComparePath = Config.Validator.BeyondComparePath;

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
                _logger.Info("Source and Destination data is null. result is true");
                return true;
            }

            if (src == null || dest == null)
            {
                _logger.Info("Source or Destination data is null. result is false");
                return false;
            }

            return DoCompareItem(src.ToString(nameof(src.Id)), dest.ToString(nameof(dest.Id)), showWindow);
        }

        public bool DoCompareItem(string src, string dest, bool showWindow = false)
        {
            if (File.Exists(beyondComparePath) == false)
            {
                _logger.Error("Beyond compare path is wrong");
                return false;
            }

            if (Directory.Exists(tempPath) == false)
            {
                Directory.CreateDirectory(tempPath);
            }

            string sourceFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
            using (var fs = new FileStream(sourceFile, FileMode.Create))
            using (var sourceSw = new StreamWriter(fs, Encoding.Default))
            {
                sourceSw.WriteLine(src);
                sourceSw.Flush();
                fs.Flush(true);
            }

            string destinationFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
            using (var fs = new FileStream(destinationFile, FileMode.Create))
            using (var destinationSw = new StreamWriter(fs, Encoding.Default))
            {
                destinationSw.WriteLine(dest);
                destinationSw.Flush();
                fs.Flush(true);
            }

            string param = $"{sourceFile} {destinationFile}";
            if (showWindow == false)
            {
                param = "/qc=binary /silent " + param;
            }

            try
            {
                int retCnt = 0;
                int exitCode = -1;
                do
                {
                    using (Process compareProcess = Process.Start(beyondComparePath, param))
                    {
                        if (compareProcess.WaitForExit(100 * 1000) == false)
                        {
                            compareProcess.Kill();
                            //_logger.Error($"Beyond Compare Timeout. Retry Count:{retCnt} ExitCode:{exitCode}");
                        }
                        exitCode = compareProcess.ExitCode;
                    }

                    if (retCnt > 3)
                    {
                        _logger.Error($"Beyond Compare Fail");
                        return false;
                    }
                    retCnt++;
                } while (exitCode == 100 || exitCode < 0);

                //if (exitCode != 0 && exitCode != 1 && exitCode != 2)
                //    _logger.Error($"ExitCode:{exitCode}");
                return exitCode == 1;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new Exception();
            }
            finally
            {
                if (File.Exists(sourceFile))
                    File.Delete(sourceFile);
                if (File.Exists(destinationFile))
                    File.Delete(destinationFile);
            }
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
        public void MakeReport(Subscribable src, Subscribable dest, string reportPath)
        {
            string code = src != null ? src.Code : dest.Code;
            StringBuilder srcString = new StringBuilder();
            StringBuilder destString = new StringBuilder();

            StringBuilder outputString = new StringBuilder();

            if (src != null)
            {
                srcString.AppendLine(src.ToString(nameof(src.Id), nameof(src.ReceivedTime)));
                outputString.AppendLine(src.ToString(nameof(src.ReceivedTime)));
            }

            string path = Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, codeDifferentPath);
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            using (var fs = new FileStream(Path.Combine(path, code + "_source.txt"), FileMode.Create))
            using (var sw = new StreamWriter(fs, Encoding.Default))
            {
                sw.WriteLine(outputString);
                sw.Flush();
                fs.Flush(true);
            }

            outputString.Clear();
            if (dest != null)
            {
                destString.AppendLine(dest.ToString(nameof(dest.Id), nameof(dest.ReceivedTime)));
                outputString.AppendLine(dest.ToString(nameof(dest.ReceivedTime)));
            }

            path = Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, codeDifferentPath);
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            using (var fs = new FileStream(Path.Combine(path, code + "_destination.txt"), FileMode.Create))
            using (var sw = new StreamWriter(fs, Encoding.Default))
            {
                sw.WriteLine(outputString);
                sw.Flush();
                fs.Flush(true);
            }

            MakeReport(srcString.ToString(), destString.ToString(), reportPath);
        }
        public void MakeReport(List<Subscribable> src, List<Subscribable> dest, string reportPath)
        {
            string code = src.Count > 0 ? src[0].Code : dest[0].Code;
            StringBuilder srcString = new StringBuilder();
            StringBuilder destString = new StringBuilder();

            StringBuilder outputString = new StringBuilder();

            foreach (Subscribable s in src)
            {
                srcString.AppendLine(s.ToString(nameof(s.Id), nameof(s.ReceivedTime)));
                outputString.AppendLine(s.ToString(nameof(s.ReceivedTime)));
            }

            string path = Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, codeDifferentPath);
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            using (var fs = new FileStream(Path.Combine(path, code + "_source.txt"), FileMode.Create))
            using (var sw = new StreamWriter(fs, Encoding.Default))
            {
                sw.WriteLine(outputString);
                sw.Flush();
                fs.Flush(true);
            }

            outputString.Clear();
            foreach (Subscribable d in dest)
            {
                destString.AppendLine(d.ToString(nameof(d.Id), nameof(d.ReceivedTime)));
                outputString.AppendLine(d.ToString(nameof(d.ReceivedTime)));
            }

            path = Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, codeDifferentPath);
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            using (var fs = new FileStream(Path.Combine(path, code + "_destination.txt"), FileMode.Create))
            using (var sw = new StreamWriter(fs, Encoding.Default))
            {
                sw.WriteLine(outputString);
                sw.Flush();
                fs.Flush(true);
            }

            MakeReport(srcString.ToString(), destString.ToString(), reportPath);
        }

        public void MakeReport(string src, string dest, string reportPath)
        {
            if (File.Exists(beyondComparePath) == false)
            {
                _logger.Error("Beyond compare path is wrong");
                return;
            }

            string sourceFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
            using (var fs = new FileStream(sourceFile, FileMode.Create))
            using (var sourceSw = new StreamWriter(fs, Encoding.Default))
            {
                sourceSw.WriteLine(src);
                sourceSw.Flush();
                fs.Flush(true);
            }

            string destinationFile = Path.Combine(tempPath, Guid.NewGuid().ToString());
            using (var fs = new FileStream(destinationFile, FileMode.Create))
            using (var destinationSw = new StreamWriter(fs, Encoding.Default))
            {
                destinationSw.WriteLine(dest);
                destinationSw.Flush();
                fs.Flush(true);
            }

            if (File.Exists(reportPath) == true)
            {
                File.Delete(reportPath);
            }

            string param = $"/qc=binary /silent @CompareScript.txt {sourceFile} {destinationFile} {reportPath}";

            try
            {
                int retCnt = 0;
                int exitCode = -1;
                do
                {
                    using (Process compareProcess = Process.Start(beyondComparePath, param))
                    {
                        if (compareProcess.WaitForExit(100 * 1000) == false)
                        {
                            compareProcess.Kill();
                            //_logger.Error($"Beyond Compare Timeout. Retry Count:{retCnt} ExitCode:{exitCode}");
                        }
                        exitCode = compareProcess.ExitCode;
                    }

                    if (retCnt > 3)
                    {
                        _logger.Error($"Beyond Compare Fail");
                        return;
                    }
                    retCnt++;
                } while (exitCode == 100 || exitCode < 0);

                //if (exitCode != 0 && exitCode != 1 && exitCode != 2)
                //    _logger.Error($"ExitCode:{exitCode}");
                //_logger.Info($"Compare result report created at {reportPath}");

                return;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                if (File.Exists(sourceFile))
                    File.Delete(sourceFile);
                if (File.Exists(destinationFile))
                    File.Delete(destinationFile);
            }
        }
    }
}
