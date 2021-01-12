using Bare.Domain;
using Bare.Services.Messageries;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bare.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private RabbitService _rabbitService;

        public Worker(ILogger<Worker> logger, RabbitService rabbitService)
        {
            _logger = logger;
            _rabbitService = rabbitService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _rabbitService.SendMessage("Hello World!");
                _rabbitService.ReceiveMessage();               
                
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
