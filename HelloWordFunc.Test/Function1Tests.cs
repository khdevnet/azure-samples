using HelloWordFunc.Services;
using HelloWordFunc.Test.TestFramework;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace HelloWordFunc.Test
{
    public class Function1Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_log_hello_word_anton()
        {
            var name = "Anton";
            var expectedName = name.ToUpper();
            var stringUppercaseServiceMock = Substitute.For<IStringUppercaseService>();
            var loggerMock = Substitute.For<ILogger<Function1>>();
            stringUppercaseServiceMock.Apply(Arg.Is<string>(v=>v == name)).Returns(expectedName);
            var func1 = new Function1(stringUppercaseServiceMock, loggerMock);
            func1.Run("Anton");

            loggerMock.Received().LogInformation($"Hello word: {expectedName}");
        }
    }
}