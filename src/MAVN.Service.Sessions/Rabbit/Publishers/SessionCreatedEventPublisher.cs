using System;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.Sessions.Contracts;
using MAVN.Service.Sessions.Core.Publishers;

namespace MAVN.Service.Sessions.Rabbit.Publishers
{
    public class SessionCreatedEventPublisher : JsonRabbitPublisher<SessionCreatedEvent>, ISessionCreatedEventPublisher
    {
        private readonly SemaphoreSlim _sync = new SemaphoreSlim(1, 1);

        public SessionCreatedEventPublisher(ILogFactory logFactory, string connectionString, string exchangeName)
            : base(logFactory, connectionString, exchangeName)
        {
        }

        public async Task PublishAsync(string customerId, DateTime timestamp)
        {
            try
            {
                await _sync.WaitAsync();

                await PublishAsync(new SessionCreatedEvent
                {
                    CustomerId = customerId,
                    TimeStamp = timestamp
                });
            }
            finally
            {
                _sync.Release();
            }
        }
    }
}