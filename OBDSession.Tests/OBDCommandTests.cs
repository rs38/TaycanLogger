using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaycanLogger.Tests
{
    [TestClass()]
    public class OBDCommandTests
    {

        OBDCommand c;

        Mock<IOBDDevice> deviceMock = new Mock<IOBDDevice>();


        public OBDCommandTests()
        {
            string query = "22 0286 2a07 f17c";
            c = new OBDCommand(deviceMock.Object)
            {

                send = query,
                skipCount = 0,
                header = "",
                name = "Test",
                HasSubCommands = true
            };

            c.Subcommands = new List<OBDBase>();
            var s = new OBDBase()
            {
                name = "v1",
                ConversionFormula = "B3+B4",
                units = "v"
            };
            c.Subcommands.Add(s);
        }



        [TestMethod()]
        public async Task processRawAnswerTest()
        {
            string query = "22 0286 2a07 f17c";
            string answer = @"021
0: 62 02 86 76 2A 07
1: 1F 5E F1 7C 53 4A 38
2: 2D 46 53 54 31 35 2E
3: 31 32 2E 31 34 31 30
4: 38 30 31 36 39 37 AA";
            deviceMock.Setup(d => d.WriteReadAsync(query))
                .ReturnsAsync(answer);

           await c.DoExecAsync();

            var res =  c.ResponseValue;
            Assert.IsTrue(res == 0.1);
        }
    }

}