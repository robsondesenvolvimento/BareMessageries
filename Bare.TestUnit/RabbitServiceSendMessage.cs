using Bare.Services.Messageries;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Bare.Domain;

namespace Bare.TestUnit
{
    public class RabbitServiceSendMessage
    {
        [Fact]
        public void CreateInstanceOfRabbitServiceAndSendMessage()
        {
            var moqLogger = new Mock<ILogger<RabbitService>>();
            var moqConfiguration = new Mock<IConfiguration>();

            moqConfiguration.Setup(m => m.GetSection("RabbitHost").Value).Returns("192.168.100.23");
            moqConfiguration.Setup(m => m.GetSection("RabbitQueue").Value).Returns("rabbit-bare");

            var rabbitService = new RabbitService(moqLogger.Object, moqConfiguration.Object);

            var  messageJson = rabbitService.SendMessage("Hello World!");

            var message = JsonSerializer.Deserialize<Message>(messageJson);

            moqConfiguration.Verify(v => v.GetSection(It.IsNotNull<string>()).Value, Times.Exactly(2));
            Assert.NotNull(message);
            Assert.IsType<Message>(message);
            Assert.Equal("Hello World!", message.MessageText);
        }
    }
}
