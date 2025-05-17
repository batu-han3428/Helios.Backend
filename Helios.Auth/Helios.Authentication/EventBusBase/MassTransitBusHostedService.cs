using MassTransit;

namespace Helios.Authentication.EventBusBase
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
                var result = await _busControl.StartAsync(cancellationToken);
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
