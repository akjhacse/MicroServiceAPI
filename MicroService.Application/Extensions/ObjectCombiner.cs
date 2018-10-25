using System;
using Newtonsoft.Json;

namespace MicroService.Application.Extensions {

   public static class ObjectCombiner {

      /// <summary>
      /// update dest from source.
      /// Eg: dest.Update(source);
      /// </summary>
      /// <param name="dest"></param>
      /// <param name="source"></param>
      public static void Update(this object dest, object source) {
         // Convert source to JSON first.
         try {
            var jsonSource = JsonConvert.SerializeObject(source, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
            JsonConvert.PopulateObject(jsonSource, dest);
         }
         catch (Exception e) {
            throw new JsonReaderException(e.Message);
         }
      }
   }
}
