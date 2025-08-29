using Serilog;
using UnityEngine;
using ILogger = Serilog.ILogger;


namespace ECARules4All_DLL.Logger
{
    public static class LoggerFactory
    {
        public static ILogger Build(LoggingOptions opt)
        {
            var cfg = new LoggerConfiguration()
                .MinimumLevel.Is(opt.MinimumLevel)
                .Enrich.FromLogContext()
                .WriteTo.File(
                    opt.LogFilePath,
                    rollingInterval: RollingInterval.Infinite,
                    shared: true);

            // seq
            if (opt.SeqUrlToken != null && !string.IsNullOrWhiteSpace(opt.SeqUrlToken.seqUrl))
            {
                cfg = cfg.WriteTo.Seq(
                    serverUrl: opt.SeqUrlToken.seqUrl,
                    apiKey: opt.SeqUrlToken.seqApiKey
                );
            }
            
            // mongodb
            /*if (opt.MongoInfo != null)
            {
                cfg = cfg.WriteTo.MongoDBBson(
                    opt.MongoInfo.MongoDbConnectionString,
                    collectionName: opt.MongoInfo.MongoDbCollection
                );
            }*/

            return cfg.CreateLogger();
        }
    }
}