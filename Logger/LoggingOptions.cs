using Serilog.Events;


namespace ECARules4All_DLL.Logger
{
    public class SeqUrlToken
    {
        public string seqUrl { get; }
        public string seqApiKey { get; }

        public SeqUrlToken(string seqUrl, string seqApiKey)
        {
            this.seqUrl = seqUrl;
            this.seqApiKey = seqApiKey;
        }
    }
    
    public class MongodbLogOptions
    {
        public string MongoDbConnectionString { get; }
        public string MongoDbCollection  { get; }

        public MongodbLogOptions(string MongoDbConnectionString, string MongoDbCollection = "unity-logs")
        {
            this.MongoDbConnectionString = MongoDbConnectionString;
            this.MongoDbCollection = MongoDbCollection;
        }
    }
    
    public class LoggingOptions
    {
        public LogEventLevel MinimumLevel { get; set; } = LogEventLevel.Debug;
        //public SeqUrlToken SeqUrlToken { get; set; } = null;
        public MongodbLogOptions MongoInfo { get; set; } = null;
        public string LogFilePath { get; set; } = "logs/log.txt";
    }
}