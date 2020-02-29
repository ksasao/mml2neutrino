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
            System.Diagnostics.StackFrame callerFrame = new System.Diagnostics.StackFrame(1);
            System.Reflection.MethodBase callerMethod = callerFrame.GetMethod();
            return File.ReadAllText($"../../xml/{methodName}.xml");
        }
        private void AssertMusicXmlAreEqual(string mml, bool create)
        {
            System.Diagnostics.StackFrame callerFrame = new System.Diagnostics.StackFrame(1);
            System.Reflection.MethodBase callerMethod = callerFrame.GetMethod();
            string methodName = callerMethod.Name;

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
        private void AssertException(string mml)
        {
            MMLParser parser = new MMLParser();
            var parsed = parser.Parse(mml);
            MusicXMLGenerator g = new MusicXMLGenerator();
            g.GenerateFromElements(parsed);
        }
        #endregion

        [TestMethod]
        public void NoteTest0()
        {
            string mml = "CあRRR";
            AssertMusicXmlAreEqual(mml, false);
        }
        [TestMethod]
        public void NoteTest1()
        {
            string mml = "C4あRRR";
            AssertMusicXmlAreEqual(mml, false);
        }
        [TestMethod]
        public void NoteTest2()
        {
            string mml = "C2.あR4C4.いC8.う";
            AssertMusicXmlAreEqual(mml, false);
        }
        [TestMethod]
        public void EighthTripletTest()
        {
            string mml = "L12CあCあCあDいDいDいR2";
            AssertMusicXmlAreEqual(mml, false);
        }
        [TestMethod]
        public void QuaterTripletTest()
        {
            string mml = "L6CあCあCあDいDいDい";
            AssertMusicXmlAreEqual(mml, false);
        }
        [TestMethod]
        public void HalfTripletTest()
        {
            string mml = "L3CあCあCあDいDいDい";
            AssertMusicXmlAreEqual(mml, false);
        }
        [TestMethod]
        public void EighthDotTest()
        {
            string mml = "L8C.あR16R4R2";
            AssertMusicXmlAreEqual(mml, false);
        }
        [TestMethod]
        public void QuaterDotTest()
        {
            string mml = "L4C.あR8R2";
            AssertMusicXmlAreEqual(mml, false);
        }
        [TestMethod]
        public void HalfDotTest()
        {
            string mml = "L2C.あR4";
            AssertMusicXmlAreEqual(mml, false);
        }
        //
        // Exception Test
        //
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void ExceptionRest1()
        {
            string mml = "L2RR4R";
            AssertException(mml);
        }
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void ExceptionNote1()
        {
            string mml = "L2CあC4あCあ";
            AssertException(mml);
        }
    }
}
