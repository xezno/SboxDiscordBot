using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SboxDiscordBot.Tests
{
    [TestClass]
    public class FetchTests
    {
        [TestMethod]
        public void FetchJsonTest()
        {
            Request.Fetch("https://jsonplaceholder.typicode.com/todos/1").Then(response =>
            {
                var jsonResponse = response.Json();
                var desiredObject = new Dictionary<string, string>()
                {
                    {"userId", "1"},
                    {"id", "1"},
                    {"title", "delectus aut autem"},
                    {"completed", "false"}
                };
                Assert.IsTrue(jsonResponse == desiredObject);
            });
        }
        
        [TestMethod]
        public void FetchTextTest()
        {
            Request.Fetch("https://jsonplaceholder.typicode.com/todos/1").Then(response =>
            {
                var textResponse = response.Text();
                Assert.IsTrue(textResponse ==
                    "{\"userId\": 1,\n  \"id\": 1,\n  \"title\": \"delectus aut autem\",\n  \"completed\": false\n}");
            });
        }
    }
}