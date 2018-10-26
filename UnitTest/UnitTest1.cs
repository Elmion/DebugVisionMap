using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DebugClient.Geometry;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestInPolygon()
        {
            Contour c = new Contour(new List<Vertex>{
                new Vertex(0,0),new Vertex(0,50), new Vertex(50,50), new Vertex(50,0)
                });
          Assert.AreEqual(true, c.IsPointInPolygon(new Vertex(25, 25)));
        }
        [TestMethod]
        public void TestInPolygon2()
        {
            Contour c = new Contour(new List<Vertex> { new Vertex(450, 190),
                                                        new Vertex(560, 170),
                                                        new Vertex(540, 270),
                                                        new Vertex(430, 290)
                                                       });
            Assert.AreEqual(false, c.IsPointInPolygon(new Vertex(25, 25)));
        }

}
}
