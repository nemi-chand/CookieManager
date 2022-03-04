using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;

namespace CookieManager.Test
{
    [TestFixture]
    public class CookieManagerTest
    {
        private Mock<ICookie> mockICookie;

        [SetUp]
        public void Setup()
        {
            mockICookie = new Mock<ICookie>();
        }

        [Test]
        public void GetStringTests()
        {
            var cookieManager = GetCookieManager();
            mockICookie.Setup(ex => ex.Get("test")).Returns("\"it works\"");

            var actualResult = cookieManager.Get<string>("test");
            Assert.AreEqual("it works", actualResult);
        }

        [Test]
        public void GetString_NotExistsTests()
        {
            var cookieManager = GetCookieManager();
            mockICookie.Setup(ex => ex.Get("test"));

            var actualResult = cookieManager.Get<string>("test");
            Assert.IsNull(actualResult);
        }

        [Test]
        public void GetJsonTests()
        {
            var json = new { name = "CookieManager", data = "secret" };
            var cookieManager = GetCookieManager();
            mockICookie.Setup(ex => ex.Get("test")).Returns(JsonConvert.SerializeObject(json));

            var actualResult = cookieManager.Get<dynamic>("test");
            Assert.AreEqual(json.name, actualResult.name.ToString());
            Assert.AreEqual(json.data, actualResult.data.ToString());
        }

        [Test]
        public void SetStringTests()
        {
            var cookieManager = GetCookieManager();
            mockICookie.Setup(ex => ex.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));
            cookieManager.Set("test", "it works", 60);
            //Verify
            mockICookie.Verify(ex => ex.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void ContainsTests()
        {
            var cookieManager = GetCookieManager();
            mockICookie.Setup(ex => ex.Contains("test")).Returns(true);
            // key exists
            var actualResult = cookieManager.Contains("test");
            Assert.True(actualResult);
            // Key doesn't exists
            var actualResult1 = cookieManager.Contains("test1");
            Assert.False(actualResult1);
        }

        [Test]
        public void RemoveTests()
        {
            var cookieManager = GetCookieManager();
            mockICookie.Setup(ex => ex.Remove(It.IsAny<string>()));
            cookieManager.Remove("test");
            //Verfiy
            mockICookie.Verify(ex => ex.Remove(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GetOrSet_NotContains_StringTests()
        {
            var cookieManager = GetCookieManager();
            mockICookie.Setup(ex => ex.Contains("test")).Returns(false);

            var actualResult = cookieManager.GetOrSet<string>("test", () => 
            {
                return "it works";
            });

            Assert.AreEqual("it works", actualResult);
            //Verfiy
            mockICookie.Verify(ex => ex.Contains("test"), Times.Once);
            mockICookie.Verify(ex => ex.Get("test"), Times.Never);

        }

        [Test]
        public void GetOrSet_Contains_StringTests()
        {
            var cookieManager = GetCookieManager();
            mockICookie.Setup(ex => ex.Contains("test")).Returns(true);
            mockICookie.Setup(ex => ex.Get("test")).Returns("\"it works\"");

            var actualResult = cookieManager.GetOrSet<string>("test", () =>
            {
                return "it works";
            });

            Assert.AreEqual("it works", actualResult);
            //Verfiy
            mockICookie.Verify(ex => ex.Contains("test"), Times.Once);
            mockICookie.Verify(ex => ex.Get("test"), Times.Once);

        }


        private ICookieManager GetCookieManager()
        {
            return new DefaultCookieManager(mockICookie.Object);
        }
    }
}
