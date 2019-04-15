using DotNetCoreExample.Models;
using DotNetCoreExample.MovieRepository;
using DotNetCoreExample.Utils;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void checkLogin()
        {
            Assert.Pass();
        }

        [Test]
        public void keyWordParse()
        {
            string keyWord = "burak gur test";
            string tmp = keyWord.Replace(' ', '+');

            Assert.AreEqual(tmp, "burak+gur+test");
        }

        [Test]
        public void omdbApiTest()
        {
            OmdbRepo repo = new OmdbRepo();
            MovieClass result = repo.getMovie("john wick");

            Assert.IsNotNull(result);
        }
        
    }
}