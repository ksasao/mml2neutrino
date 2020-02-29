using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MML2NEUTRINO;

namespace UnitTest
{
    [TestClass]
    public class ParseTest
    {
        [TestMethod]
        public void EmptyTest0()
        {
            string mml = "";
            MMLParser parser = new MMLParser();
            IElement[] elements = new IElement[]
            {
            };
            IElement[] parsed = parser.Parse(mml);
            CollectionAssert.AreEqual(elements, parsed);
        }
        [TestMethod]
        public void EmptyTest1()
        {
            string mml = " c  あ ";
            MMLParser parser = new MMLParser();
            IElement[] elements = new IElement[]
            {
                new Note{ Octave = 4, Step = "C", Length = 4, Alter = 0, HasDot = false,  Lyric = "あ", MML="Cあ" },
            };
            IElement[] parsed = parser.Parse(mml);
            CollectionAssert.AreEqual(elements, parsed);
        }
        [TestMethod]
        public void NoteTest0()
        {
            string mml = "Cあ";
            MMLParser parser = new MMLParser();
            IElement[] elements = new IElement[]
            {
                new Note{ Octave = 4, Step = "C", Length = 4, Alter = 0, HasDot = false,  Lyric = "あ", MML="Cあ" },
            };
            IElement[] parsed = parser.Parse(mml);
            CollectionAssert.AreEqual(elements, parsed);
        }
        [TestMethod]
        public void NoteSharpTest1()
        {
            string mml = "C+あ";
            MMLParser parser = new MMLParser();
            IElement[] elements = new IElement[]
            {
                new Note{ Octave = 4, Step = "C", Length = 4, Alter = 1, HasDot = false,  Lyric = "あ", MML="C+あ" },
            };
            IElement[] parsed = parser.Parse(mml);
            CollectionAssert.AreEqual(elements, parsed);
        }
        [TestMethod]
        public void NoteSharpTest2()
        {
            string mml = "C#あ";
            MMLParser parser = new MMLParser();
            IElement[] elements = new IElement[]
            {
                new Note{ Octave = 4, Step = "C", Length = 4, Alter = 1, HasDot = false,  Lyric = "あ", MML="C#あ" },
            };
            IElement[] parsed = parser.Parse(mml);
            CollectionAssert.AreEqual(elements, parsed);
        }
        [TestMethod]
        public void NoteFlatTest1()
        {
            string mml = "C-あ";
            MMLParser parser = new MMLParser();
            IElement[] elements = new IElement[]
            {
                new Note{ Octave = 4, Step = "C", Length = 4, Alter = -1, HasDot = false,  Lyric = "あ", MML="C-あ" },
            };
            IElement[] parsed = parser.Parse(mml);
            CollectionAssert.AreEqual(elements, parsed);
        }

        [TestMethod]
        public void NoteTest1()
        {
            string mml = "CあDいEうFえGおAかBき>Cく";
            MMLParser parser = new MMLParser();
            IElement[] elements = new IElement[]
            {
                new Note{ Octave = 4, Step = "C", Length = 4, Alter = 0, HasDot = false,  Lyric = "あ", MML="Cあ" },
                new Note{ Octave = 4, Step = "D", Length = 4, Alter = 0, HasDot = false,  Lyric = "い", MML="Dい" },
                new Note{ Octave = 4, Step = "E", Length = 4, Alter = 0, HasDot = false,  Lyric = "う", MML="Eう" },
                new Note{ Octave = 4, Step = "F", Length = 4, Alter = 0, HasDot = false,  Lyric = "え", MML="Fえ" },
                new Note{ Octave = 4, Step = "G", Length = 4, Alter = 0, HasDot = false,  Lyric = "お", MML="Gお" },
                new Note{ Octave = 4, Step = "A", Length = 4, Alter = 0, HasDot = false,  Lyric = "か", MML="Aか" },
                new Note{ Octave = 4, Step = "B", Length = 4, Alter = 0, HasDot = false,  Lyric = "き", MML="Bき" },
                new Note{ Octave = 5, Step = "C", Length = 4, Alter = 0, HasDot = false,  Lyric = "く", MML="Cく" }
            };
            IElement[] parsed = parser.Parse(mml);
            CollectionAssert.AreEqual(elements, parsed);
        }
        [TestMethod]
        public void NoteTest2()
        {
            string mml = "Cあ>>Dい<<Eう";
            MMLParser parser = new MMLParser();
            IElement[] elements = new IElement[]
            {
                new Note{ Octave = 4, Step = "C", Length = 4, Alter = 0, HasDot = false,  Lyric = "あ", MML="Cあ" },
                new Note{ Octave = 6, Step = "D", Length = 4, Alter = 0, HasDot = false,  Lyric = "い", MML="Dい" },
                new Note{ Octave = 4, Step = "E", Length = 4, Alter = 0, HasDot = false,  Lyric = "う", MML="Eう" }
            };
            IElement[] parsed = parser.Parse(mml);
            CollectionAssert.AreEqual(elements, parsed);
        }
        [TestMethod]
        public void NoteTest3()
        {
            string mml = "CあL8DいE4う";
            MMLParser parser = new MMLParser();
            IElement[] elements = new IElement[]
            {
                new Note{ Octave = 4, Step = "C", Length = 4, Alter = 0, HasDot = false,  Lyric = "あ", MML="Cあ"  },
                new Note{ Octave = 4, Step = "D", Length = 8, Alter = 0, HasDot = false,  Lyric = "い", MML="Dい"  },
                new Note{ Octave = 4, Step = "E", Length = 4, Alter = 0, HasDot = false,  Lyric = "う", MML="E4う"  }
            };
            IElement[] parsed = parser.Parse(mml);
            CollectionAssert.AreEqual(elements, parsed);
        }
        [TestMethod]
        public void RestTest1()
        {
            string mml = "RR1L8R";
            MMLParser parser = new MMLParser();
            IElement[] elements = new IElement[]
            {
                new Rest{ Length = 4, HasDot = false, MML="R"},
                new Rest{ Length = 1, HasDot = false, MML="R1"},
                new Rest{ Length = 8, HasDot = false, MML="R"}
            };
            IElement[] parsed = parser.Parse(mml);
            CollectionAssert.AreEqual(elements, parsed);
        }
        [TestMethod]
        public void TempoTest1()
        {
            string mml = "T150Cあ";
            MMLParser parser = new MMLParser();
            IElement[] elements = new IElement[]
            {
                new Tempo{ Value = 150 },
                new Note{ Octave = 4, Step = "C", Length = 4, Alter = 0, HasDot = false,  Lyric = "あ", MML="Cあ" }
            };
            IElement[] parsed = parser.Parse(mml);
            CollectionAssert.AreEqual(elements, parsed);
        }
        [TestMethod]
        public void RemoveSpaceTest1()
        {
            string mml1 = "T150CあDいEうFえ";
            string mml2 = " T150Cあ \tDい　Eう\r\nFえ ";
            MMLParser parser = new MMLParser();
            var p1 = parser.Parse(mml1);
            var p2 = parser.Parse(mml2);
            CollectionAssert.AreEqual(p1,p2);
        }

        [TestMethod]
        public void DotTest1()
        {
            string mml = "C2.うC4.あC8.い";
            MMLParser parser = new MMLParser();
            IElement[] elements = new IElement[]
            {
                new Note{ Octave = 4, Step = "C", Length = 2, Alter = 0, HasDot = true,  Lyric = "う", MML="C2.う" },
                new Note{ Octave = 4, Step = "C", Length = 4, Alter = 0, HasDot = true,  Lyric = "あ", MML="C4.あ" },
                new Note{ Octave = 4, Step = "C", Length = 8, Alter = 0, HasDot = true,  Lyric = "い", MML="C8.い" }
            };
            IElement[] parsed = parser.Parse(mml);
            CollectionAssert.AreEqual(elements, parsed);
        }
        [TestMethod]
        public void TripletTest1()
        {
            string mml = "L6CあCあCあR2";
            MMLParser parser = new MMLParser();
            IElement[] elements = new IElement[]
            {
                new Note{ Octave = 4, Step = "C", Length = 6, Alter = 0, HasDot = false,  Lyric = "あ", MML="Cあ" },
                new Note{ Octave = 4, Step = "C", Length = 6, Alter = 0, HasDot = false,  Lyric = "あ", MML="Cあ" },
                new Note{ Octave = 4, Step = "C", Length = 6, Alter = 0, HasDot = false,  Lyric = "あ", MML="Cあ" },
                new Rest{ Length = 2, MML="R2"}
            };
            IElement[] parsed = parser.Parse(mml);
            CollectionAssert.AreEqual(elements, parsed);
        }
        //
        // Exception Test
        //
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void Exception1()
        {
            string mml = "あ";
            MMLParser parser = new MMLParser();
            IElement[] parsed = parser.Parse(mml);
        }
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void Exception2()
        {
            string mml = "C";
            MMLParser parser = new MMLParser();
            IElement[] parsed = parser.Parse(mml);
        }
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void Exception3()
        {
            string mml = "O9";
            MMLParser parser = new MMLParser();
            IElement[] parsed = parser.Parse(mml);
        }
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void Exception4()
        {
            string mml = "O0";
            MMLParser parser = new MMLParser();
            IElement[] parsed = parser.Parse(mml);
        }
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void Exception5()
        {
            string mml = "X";
            MMLParser parser = new MMLParser();
            IElement[] parsed = parser.Parse(mml);
        }
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void Exception6()
        {
            string mml = "O1<";
            MMLParser parser = new MMLParser();
            IElement[] parsed = parser.Parse(mml);
        }
        [TestMethod()]
        [ExpectedException(typeof(FormatException))]
        public void Exception7()
        {
            string mml = "O8>";
            MMLParser parser = new MMLParser();
            IElement[] parsed = parser.Parse(mml);
        }
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Exception8()
        {
            string mml = "Cあ>>Dい<<E";
            MMLParser parser = new MMLParser();
            IElement[] parsed = parser.Parse(mml);
        }
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void LengthException1()
        {
            string mml = "L";
            MMLParser parser = new MMLParser();
            IElement[] parsed = parser.Parse(mml);
        }
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void OctaveException1()
        {
            string mml = "O";
            MMLParser parser = new MMLParser();
            IElement[] parsed = parser.Parse(mml);
        }
    }
}
