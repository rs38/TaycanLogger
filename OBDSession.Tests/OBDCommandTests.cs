using Xunit;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TaycanLogger.Tests
{
    public class OBDCommandTests
    {
        List<OBDCommand> cmds;
        OBDSession session;
        Mock<IOBDDevice> deviceMock = new Mock<IOBDDevice>();

        public OBDCommandTests()
        {
            string query = "22 0286 2a07 f17c";
            BuildCommands(query);
           var configContent = File.ReadAllText("obd2_Taycan.xml");
            session = new OBDSession(configContent, "fakedevice");
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
                Units = "v"
            };
            s.AddConversionFormula("B3+B4");
            c.Values.Add(s);
            s = new OBDValue(c)
            {
                Name = "v2",
                Units = "v"
            };
            s.AddConversionFormula("B8*B9/9.1");
            c.Values.Add(s);

            cmds.Add(c);
        }

        [Fact]
        public void LoadConfigShouldworkFine()
        {
            session.hasValidConfig().Should().BeTrue();
            var x = session.cmds.SelectMany(commands => commands.Values).ToList();
            x.Count.Should().Be(19);
        }
        [Fact]
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
            x.First().Should().Be(160);
            x.Skip(1).First().Should().BeApproximately(3283.95, 0.1);
           
        }
        
        [Fact]
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
            e.Current.Should().Be(160);
            
            e.MoveNext();
            e.Current.Should().BeApproximately(3283.95, 0.1);
            
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