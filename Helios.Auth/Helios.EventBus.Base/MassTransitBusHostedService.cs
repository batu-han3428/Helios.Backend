using MassTransit;
using MassTransit.Util;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.EventBus.Base
{
    public class MassTransitBusHostedService : IHostedService
    {
        private readonly IBusControl _busControl;

        public MassTransitBusHostedService(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                TaskUtil.Await(() => _busControl.StartAsync(), cancellationToken);
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _busControl.StopAsync(cancellationToken);
        }
    }
}
