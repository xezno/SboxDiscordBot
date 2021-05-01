using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Frameworks;

namespace SboxDiscordBot.Tests
{
    [TestClass]
    public class FetchTests
    {
        [TestMethod]
        public void FetchJsonTest()
        {
            Request.Fetch("https://jsonplaceholder.typicode.com/todos/1").ContinueWith(task =>
            {
                if (task.Exception != null)
                    Assert.Fail();
                
                var response = task.Result;
                var jsonResponse = response.Json();
                var desiredObject = new Dictionary<string, string>
                {
                    {"userId", "1"},
                    {"id", "1"},
                    {"title", "delectus aut autem"},
                    {"completed", "false"}
                };
                
                Assert.IsTrue(jsonResponse.Count == desiredObject.Count && !jsonResponse.Except(desiredObject).Any());
            }).GetAwaiter().GetResult();
        }
        
        [TestMethod]
        public void FetchTextTest()
        {
            Request.Fetch("https://jsonplaceholder.typicode.com/todos/1").ContinueWith(task =>
            {
                if (task.Exception != null)
                    Assert.Fail();
                
                var response = task.Result;
                
                var textResponse = response.Text();
                Assert.IsTrue(textResponse ==
                              "{\n  \"userId\": 1,\n  \"id\": 1,\n  \"title\": \"delectus aut autem\",\n  \"completed\": false\n}");
            }).GetAwaiter().GetResult();
        }
    }
}