using Bare.Domain;
using Bare.Services.Messageries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Bare.TestUnit
{
    public class RabbitServiceReceiveMessage
    {
        [Fact]
        public void CreateInstanceOfRabbitServiceAndSendMessage()
        {
            var moqLogger = new Mock<ILogger<RabbitService>>();
            var moqConfiguration = new Mock<IConfiguration>();

            moqConfiguration.Setup(m => m.GetSection("RabbitHost").Value).Returns("192.168.100.23");
            moqConfiguration.Setup(m => m.GetSection("RabbitQueue").Value).Returns("rabbit-bare");

            var rabbitService = new RabbitService(moqLogger.Object, moqConfiguration.Object);

            rabbitService.SendMessage("Hello World!");
            rabbitService.ReceiveMessage();
            var messageJson = rabbitService.MsgConsume;

            var message = JsonSerializer.Deserialize<Message>(messageJson);

            moqConfiguration.Verify(v => v.GetSection(It.IsNotNull<string>()).Value, Times.Exactly(2));
            Assert.NotNull(message);
            Assert.IsType<Message>(message);
            Assert.Equal("Hello World!", message.MessageText);
        }
    }
}
