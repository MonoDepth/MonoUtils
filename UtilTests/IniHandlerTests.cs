using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonoUtilities.Ini;

namespace UtilTests
{
    [TestClass]
    public class IniHandlerTests
    {
        [ClassInitialize]
        public static void Setup(TestContext ctx)
        {            
            new IniTestClass("temp_unittests.ini").SaveConfig();
        }

        [TestMethod]
        public void SaveWithDefaultValues_ShouldReturnDefaultValues()
        {
            IniTestClass iniHandler = new IniTestClass("temp_unittests.ini");
            //Overwrite any potential changed data since ClassInitialize
            iniHandler.SaveConfig();

            //Change the data then load the same file again, should contain the defaults from IniTestClass
            iniHandler.Integer++;
            iniHandler.Double++;
            iniHandler.Float++;
            iniHandler.Decimal++;
            iniHandler.String += "d";
            iniHandler.Char = 'e';
            iniHandler.DateTime.AddDays(1);
            iniHandler.Boolean = false;
            iniHandler.LoadConfig();

            Assert.AreEqual(-1, iniHandler.Integer);
            Assert.AreEqual(1.3d, iniHandler.Double);
            Assert.AreEqual(2.5f, iniHandler.Float);
            Assert.AreEqual(3.7m, iniHandler.Decimal);
            Assert.AreEqual("abc 123", iniHandler.String);
            Assert.AreEqual('d', iniHandler.Char);
            Assert.AreEqual(new DateTime(2000, 02, 06), iniHandler.DateTime);
            Assert.AreEqual(true, iniHandler.Boolean);
        }

        [TestMethod]
        public void ChangeDate_ShouldReturnChangedDate()
        {
            IniTestClass iniHandler = new IniTestClass("temp_unittests.ini");
            //Add two days and save to the file
            iniHandler.DateTime = iniHandler.DateTime.AddDays(2);
            iniHandler.SaveConfig();

            //Change the date, then load it back from file to reset it
            iniHandler.DateTime.AddDays(-15);
            iniHandler.LoadConfig();

            Assert.AreEqual(new DateTime(2000, 02, 08), iniHandler.DateTime);
        }

        [TestMethod]
        public void SaveToFile_ShouldOnlySavePublicProperties()
        {
            IniTestClass iniHandler = new IniTestClass("temp_unittests.ini");
            iniHandler.SaveConfig();
            iniHandler.ChangePrivates();
            iniHandler.LoadConfig();

            Assert.AreEqual(true, iniHandler.ArePrivateEmpty());
        }

        [TestMethod]
        public void SaveToFile_ShouldNotContainBaseClassProps()
        {
            IniTestClass iniHandler = new IniTestClass("temp_unittests.ini");            
            iniHandler.SaveConfig();          
            Assert.AreEqual(true, File.Exists("temp_unittests.ini"));

            string text = File.ReadAllText("temp_unittests.ini");
            Assert.AreEqual(false, text.Contains("FilePath"));
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            if (File.Exists("temp_unittests.ini"))
                File.Delete("temp_unittests.ini");
        }
    }

    public class IniTestClass : MonoIni
    {
        public IniTestClass(string aFilePath) : base(aFilePath) { }

        [Section("Numbers")]
        public int Integer { get; set; } = -1;
        public double Double { get; set; } = 1.3d;
        public float Float { get; set; } = 2.5f;
        public decimal Decimal { get; set; } = 3.7m;

        [Section("Text")]
        public string String { get; set; } = "abc 123";
        public char Char { get; set; } = 'd';

        [Section("Misc")]
        public DateTime DateTime { get; set; } = new DateTime(2000, 02, 06);
        public bool Boolean { get; set; } = true;

        private string PrivateVar = "a62ef8c";
        private string PrivateProp { get; set; } = "a62ef8c";

        public void ChangePrivates()
        {
            PrivateVar = "";
            PrivateProp = "";
        }

        public bool ArePrivateEmpty()
        {
            return (PrivateVar == "" && PrivateProp == "");
        }
    }
}
