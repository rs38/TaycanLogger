﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        Mock<IOBDDevice> deviceMock = new Mock<IOBDDevice>();


        public OBDCommandTests()
        {
            string query = "22 0286 2a07 f17c";
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
                name = "v1",
                ConversionFormula = "B3+B4",
                units = "v"
            };
            c.Values.Add(s);
            s = new OBDValue(c)
            {
                name = "v2",
                ConversionFormula = "B8*B9/9.1",
                units = "v"
            };
            c.Values.Add(s);

            cmds.Add(c);

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

            await cmds[0].DoExecAsync();
            var x =  cmds.SelectMany(values => values.Values).Select(v => v.Value);
            Assert.AreEqual(x.First(), 160);
            Assert.AreEqual(x.Skip(1).First(), 3283.95, 0.1);
        }

        [TestMethod()]
        public async Task processRawAnswerTest2()
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
        }
    }

}