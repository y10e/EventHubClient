using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text.Json;

namespace EventHubsSender
{
    class Program
    {
        const string version = "0.0.1";

        class Config
        {
            public int count { get; set; }
            public string msgprefix { get; set; }
            public string eventHubName { get; set; }
            public string connectionString { get; set; }


            public Config() {
                this.count = 10;
                this.msgprefix = "";
                this.eventHubName = "";
                this.connectionString = "";
            }

            public void ReadConfigFile(string configPath = @"./sender.config.json")
            {
                try
                {
                    using (StreamReader st = new StreamReader(configPath, Encoding.GetEncoding("UTF-8")))
                    {
                        string configJson = st.ReadToEnd();
                        Config readConfig = JsonSerializer.Deserialize<Config>(configJson);
                        this.count = readConfig.count;
                        this.msgprefix = readConfig.msgprefix;
                        this.eventHubName = readConfig.eventHubName;
                        this.connectionString = readConfig.connectionString;

                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Failed to read config file.");
                    Console.WriteLine(e.Message);
                }
            }

            public void ReadCmdArgs(string[] cmdArgs)
            {
                for (int i = 1; i < cmdArgs.Length; i++)
                {
                    //Console.WriteLine("command : {0:d} : {1}\r\n", i, Commands[i]);
                    switch (cmdArgs[i])
                    {
                        // the number of messages sent to EventHub
                        case "-c":
                            this.count = int.Parse(cmdArgs[i + 1]);
                            break;
                        case "--count":
                            this.count = int.Parse(cmdArgs[i + 1]);
                            break;

                        // message prefix
                        case "-p":
                            this.msgprefix = cmdArgs[i + 1];
                            break;
                        case "--prefix":
                            this.msgprefix = cmdArgs[i + 1];
                            break;

                        // EventHub name for the destination
                        case "-n":
                            this.eventHubName = cmdArgs[i + 1];
                            break;
                        case "--name":
                            this.eventHubName = cmdArgs[i + 1];
                            break;

                        // connection string of EventHub
                        case "-s":
                            this.connectionString = cmdArgs[i + 1];
                            break;
                        case "--connectionstring":
                            this.connectionString = cmdArgs[i + 1];
                            break;
                    }
                }
            }

            public void PrintValue() {
                Console.WriteLine("count \t\t\t: {0}", count);
                Console.WriteLine("msgprefix \t\t: {0}", msgprefix);
                Console.WriteLine("eventHubName \t\t: {0}", eventHubName);
                Console.WriteLine("connectionString \t: {0}", connectionString);
            }
        }

        static async Task Main(string[] args)
        {
            string[] Commands = System.Environment.GetCommandLineArgs();

            // Configration initialize
            Config config = new Config();

            // Read configration value from ./config.json
            config.ReadConfigFile();

            if (Commands.Length < 2)
            {
                DisplayAbstract();
                return;
            }
            else
            {
                // Read configration values from ./sender.config.json
                config.ReadCmdArgs(Commands);
            }

            if (config.connectionString != "") {
                Console.WriteLine("-----------------");
                config.PrintValue();
                Console.WriteLine("-----------------");
                await using (var producerClient = new EventHubProducerClient(config.connectionString, config.eventHubName))
                {
                    // Create a batch of events 
                    using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                    // Add events to the batch. An event is a represented by a collection of bytes and metadata. 
                    Console.WriteLine("A batch of events started to be published.");
                    for (int i = 0; i < config.count; i++)
                    {
                        String eventmsg = config.msgprefix + " event " + (i+1) +"/"+ config.count + " " + DateTime.Now.ToString();
                        Console.WriteLine(eventmsg);
                        eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(eventmsg)));
                    }

                    // Use the producer client to send the batch of events to the event hub
                    await producerClient.SendAsync(eventBatch);
                    Console.WriteLine("A batch of events has been published.");
                }
            }

        }

        static void DisplayAbstract()
        {

            Console.WriteLine(Figgle.FiggleFonts.Standard.Render("EventHub Sender!"));
            Console.WriteLine("EventHubSender v{0} - simple messages sending utility to EventHub", version);
            Console.WriteLine("Copyright (C) Yusuke.Yoneda");
            Console.WriteLine("");
            Console.WriteLine("EventHubSender usage: ehsend [-c/--count MessageCount] [-p/--prefix MessagePrefix] [-n/--name EventHubName] [-s/--connectionstring ConnectionString]");
            Console.WriteLine(" -c\tthe number of messages sent to EventHub. default value is 10.");
            Console.WriteLine(" -p\tmessage prefix. If Prefix is \"hoge\", the mssage is \"hoge event #{num} yyyy/mm/dd hh:mm:ss");
            Console.WriteLine(" -n\teventhub name for the destination");
            Console.WriteLine(" -s\tconnection string of EventHub");

        }
    }
}
