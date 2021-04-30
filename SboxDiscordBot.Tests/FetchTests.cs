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
                Assert.IsTrue(jsonResponse.Count == 4);
            });
        }
    }
}