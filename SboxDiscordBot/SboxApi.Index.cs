using System.Collections.Generic;
using Newtonsoft.Json;
using RSG;

namespace SboxDiscordBot
{
    public partial class SboxApi
    {
        public struct Category
        {
            public string Title { get; set; }
            public string Description { get; set; }
            
            public List<Package> Packages { get; set; }
        }

        public struct Index
        {
            public List<Category> Categories { get; set; }
        }

        public Promise<Index> GetIndex()
        {
            var promise = new Promise<Index>();
            // Endpoint: https://apix.facepunch.com/api/sbox/menu/index
            Request.Fetch("https://apix.facepunch.com/api/sbox/menu/index").Then(response =>
            {
                var index = response.Json<Index>();
                promise.Resolve(index);
            }).Catch(exception =>
            {
                promise.Reject(exception);
            });

            return promise;
        }
    }
}