using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MML2NEUTRINO;

namespace UnitTest
{
    [TestClass]
    public class GenerateTest
    {
        #region Utility
        private void WriteXml(string methodName, XElement xElement)
        {
            File.WriteAllText($"../../xml/{methodName}.xml", xElement.ToString());
        }
        private string ReadXml(string methodName)
        {
            return File.ReadAllText($"../../xml/{methodName}.xml");
        }
        private void AssertMusicXmlAreEqual(string methodName, string mml, bool create)
        {
            MMLParser parser = new MMLParser();
            IElement[] parsed = parser.Parse(mml);
            MusicXMLGenerator g = new MusicXMLGenerator();
            var xElement = g.GenerateFromElements(parsed);
            if (create)
            {
                WriteXml(methodName, xElement);
            }
            var expected = ReadXml(methodName);
            Assert.AreEqual(expected, xElement.ToString());
        }
        private void AssertMusicXmlAreEqual(string methodName, string mml)
        {
            AssertMusicXmlAreEqual(methodName, mml, false);
        }
        #endregion

        [TestMethod]
        public void NoteTest0()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string mml = "CあRRR";
            AssertMusicXmlAreEqual(methodName, mml);
        }
        [TestMethod]
        public void NoteTest1()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string mml = "C4あRRR";
            AssertMusicXmlAreEqual(methodName, mml);
        }
        [TestMethod]
        public void NoteTest2()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string mml = "C2.あR4C4.いC8.う";
            AssertMusicXmlAreEqual(methodName, mml);
        }
    }
}
