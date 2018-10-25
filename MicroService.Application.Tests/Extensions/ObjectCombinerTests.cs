using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroService.Application.Extensions;

namespace MicroService.Application.Tests.Extensions {

   [TestClass]
   public class ObjectCombinerTests {


      class Source {

         public string Prop1 { get; set; }

         public string Prop2 { get; set; }

         public string Prop3 { get; set; }
      }

      class Destination {

         public string Prop1 { get; set; }

         public string Prop2 { get; set; }

         public string Prop4 { get; set; }
      }

      [TestMethod]
      public void ObjectCombinerTest_Success() {
         //Arrange
         var source = new Source {
            Prop1 = "p1",
            Prop2 = "p2",
            Prop3 = "p3"
         };
         var destination = new Destination();

         //Act
         destination.Update(source);

         //Assert
         Assert.AreEqual(source.Prop1, destination.Prop1);
         Assert.AreEqual(source.Prop2, destination.Prop2);
         Assert.IsTrue(string.IsNullOrEmpty(destination.Prop4));
      }
   }
}
