using Mtf.Serial.SerialDevices;
using NUnit.Framework;

namespace Mtf.Serial.UnitTests.SerialDevicesTest
{
    [TestFixture]
    public class CarTftTests
    {
        [TestCase(103.9, "FF73A40573")]
        public void TestFrequencyToRdsCode(double frequency, string expectedResult)
        {
            var result = CarTft.FrequencyToRdsCode(frequency);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase("3F474E5320476D62483F", "GNS GmbH")]
        public void TestConvertHexToAscii(string response, string expectedResult)
        {
            var result = CarTft.ConvertHexToAscii(response);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}