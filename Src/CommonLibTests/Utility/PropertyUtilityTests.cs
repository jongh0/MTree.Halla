using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CommonLib.Utility.Tests
{
    public class TestClass
    {
        public string Name { get; set; }

        public IEnumerable<string> Names { get; set; }
    }


    [TestClass()]
    public class PropertyUtilityTests
    {
        [TestMethod()]
        public void PrintNameValuesTest()
        {
            var test = new TestClass();
            test.Name = "Test Class";
            var names = new List<string>();
            names.Add("haha");
            names.Add("hoho");
            test.Names = names; 

            Trace.WriteLine(PropertyUtility.PrintNameValues(test, Environment.NewLine));
            Debugger.Break();
        }
    }
}