using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaycanLogger.Tests
{
    [TestClass()]
    public class OBDCommandTests
    {

        List<OBDCommand> cmds;

        OBDSession session;

        Mock<IOBDDevice> deviceMock = new Mock<IOBDDevice>();


        public OBDCommandTests()
        {
            string query = "22 0286 2a07 f17c";
            BuildCommands(query);
            session = new OBDSession("obd2_gw.xml", "fakedevice");


        }

        private void BuildCommands(string query)
        {
            cmds = new List<OBDCommand>();

            var c = new OBDCommand(deviceMock.Object)
            {
                send = query,
                skipCount = 0,
                header = ""
            };

            c.Values = new List<OBDValue>();
            var s = new OBDValue(c)
            {
                Name = "v1",
                ConversionFormula = "B3+B4",
                units = "v"
            };
            c.Values.Add(s);
            s = new OBDValue(c)
            {
                Name = "v2",
                ConversionFormula = "B8*B9/9.1",
                units = "v"
            };
            c.Values.Add(s);

            cmds.Add(c);
        }

        [TestMethod()]

        public void LoadConfigShouldworkFine()
        {
            Assert.IsTrue( session.hasValidConfig());
            var x = session.cmds.SelectMany(commands => commands.Values).ToList();

        }
        [TestMethod()]
        public async Task processRawAnswerTestMultiframe5()
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

            await cmds[0].DoExecAsync();
            var x =  cmds.SelectMany(values => values.Values).Select(v => v.Value);
            Assert.AreEqual(x.First(), 160);
            Assert.AreEqual(x.Skip(1).First(), 3283.95, 0.1);
        }

        [TestMethod()]
        public async Task processRawAnswerTest4Lines()
        {
            string query = "22 0286 2a07 f17c";
            string answer = @"021
0: 62 02 86 76 2A 07
1: 1F 5E F1 7C 53 4A 38
2: 2D 46 53 54 31 35 2E
3: 31 32 2E 31 34 31 30";

            deviceMock.Setup(d => d.WriteReadAsync(query))
                .ReturnsAsync(answer);
            
            await cmds[0].DoExecAsync();
            var x = cmds.SelectMany(values => values.Values).Select(v => v.Value);
            var e = x.GetEnumerator();
            e.MoveNext();
            Assert.AreEqual(e.Current, 160);
            e.MoveNext();
            Assert.AreEqual(e.Current, 3283.95, 0.1);

            answer = @"00D
0: 6218020249A4
1: 18011F46F40D00
>";
            //gateway
            answer = @">2202f9
008
0: 62 02 F9 01 4D A0
1: A8 2D AA AA AA AA AA
";
        }
    }

}