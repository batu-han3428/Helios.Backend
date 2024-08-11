using StackExchange.Redis;

namespace Helios.Shared.Helpers
{
    public class ConnectionHelper
    {
        private static readonly Lazy<ConnectionMultiplexer> lazyConnection;

        static ConnectionHelper()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var redisConnectionString = configuration.GetSection("Redis")["ConnectionString"];
            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                var options = ConfigurationOptions.Parse(redisConnectionString);
                options.AbortOnConnectFail = false;
                options.ConnectRetry = 5;
                options.ConnectTimeout = 10000; // Increase timeout as needed
                options.KeepAlive = 180;
                return ConnectionMultiplexer.Connect(options);
            });
        }

        public static ConnectionMultiplexer Connection => lazyConnection.Value;
    }
}
