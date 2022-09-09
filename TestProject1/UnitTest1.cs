using WebApplication3.Controllers;
using WebApplication3.Data;

namespace TestProject1
{
    public class Tests
    {

        private readonly FileDBContext _context;
        [SetUp]
        public void Setup()
        {
            
        }

        public Tests(FileDBContext context)
        {
            _context = context;
        }

        [Test]
        public void Test1()
        {
           
            var apicontroller = new SpeechToTextController(_context);
            var result = apicontroller.Get();
            Assert.AreEqual("220 Exercises.docx"+"\n", result); //initially, only this document name should be returned
        }
    }
}