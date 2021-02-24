using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using System.Text.Json;

namespace EventHubsReceiver
{

    class Program
    {
        const string version = "0.0.1";

        class Config
        {
            public string eventHubName { get; set; }
            public string eventHubConnectionString { get; set; }

            public string consumergroup { get; set; }

            public string blobStorageConnectionString { get; set; }

            public string blobContainerName { get; set; }


            public void ReadConfigFile(string configPath = @"./receiver.config.json")
            {

                Console.WriteLine("Loading configration setting from {0}", configPath);

                try
                {
                    using (StreamReader st = new StreamReader(configPath, Encoding.GetEncoding("UTF-8")))
                    {
                        string configJson = st.ReadToEnd();
                        Config readConfig = JsonSerializer.Deserialize<Config>(configJson);

                        this.eventHubName = readConfig.eventHubName;
                        this.eventHubConnectionString = readConfig.eventHubConnectionString;
                        this.consumergroup = readConfig.consumergroup;
                        this.blobStorageConnectionString = readConfig.blobStorageConnectionString;
                        this.blobContainerName = readConfig.blobContainerName;

                        if (this.consumergroup == "")
                        {
                            this.consumergroup = EventHubConsumerClient.DefaultConsumerGroupName;
                        }

                        if (this.blobContainerName == "" ) {

                            if (readConfig.consumergroup == "") {
                                this.blobContainerName = "default";
                            }
                            else
                            {
                                this.blobContainerName = this.consumergroup;
                            }
                        }
                    }

                    Console.WriteLine("-----------------");
                    this.PrintValue();
                    Console.WriteLine("-----------------");
                    Console.WriteLine("done!");
                }
                catch (IOException e)
                {
                    Console.WriteLine("Failed to read config file.");
                    Console.WriteLine(e.Message);
                }
            }

            public void PrintValue()
            {
                Console.WriteLine("EventHub \t\t\t: {0}", eventHubName);
                Console.WriteLine("EventHubConnectionString \t: {0}", eventHubConnectionString);
                Console.WriteLine("consumergroup  \t\t\t: {0}", consumergroup);
                Console.WriteLine("StorageConnectionString  \t: {0}", blobStorageConnectionString);
                Console.WriteLine("BlobContainer  \t\t\t: {0}", blobContainerName);

            }
        }

        static async Task Main()
        {
            DisplayAbstract();

            //Load configration from ./receiver.config.json
            Config config = new Config();
            config.ReadConfigFile();

            // Create a blob container client that the event processor will use 
            BlobContainerClient storageClient = new BlobContainerClient(config.blobStorageConnectionString, config.blobContainerName);
            storageClient.CreateIfNotExists();

            // Create an event processor client to process events in the event hub
            EventProcessorClient processor = new EventProcessorClient(storageClient, config.consumergroup, config.eventHubConnectionString, config.eventHubName);

            // Register handlers for processing events and handling errors
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            // Start the processing
            Console.WriteLine("Starting to receive messages from Eventhub. Press any key to stop this application.");
            Console.WriteLine("");

            await processor.StartProcessingAsync();

            // Wait for 10 seconds for the events to be processed
            //await Task.Delay(TimeSpan.FromSeconds(60));
            Console.ReadLine();

            // Stop the processing
            Console.WriteLine("Stopping. please wait a minute.");
            await processor.StopProcessingAsync();
        }

        static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            // Write the body of the event to the console window
            Console.WriteLine("Received event on {0}", DateTime.Now.ToString());
            Console.WriteLine(" Event Body: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));
            Console.WriteLine(" EnqueuedTime: {0}", eventArgs.Data.EnqueuedTime.ToString());
            Console.WriteLine(" SequenceNumber: {0}", eventArgs.Data.SequenceNumber.ToString());
            Console.WriteLine(" Offset: {0}", eventArgs.Data.Offset.ToString());
            Console.WriteLine(" PartitionKey: {0}", eventArgs.Partition.PartitionId);
            Console.WriteLine("");



            // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            // Write details about the error to the console window
            Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.CompletedTask;
        }

        static void DisplayAbstract()
        {
            Console.WriteLine(Figgle.FiggleFonts.Standard.Render("EventHub Receiver!"));
            Console.WriteLine("EventHubReceiver v{0} - simple receive messages utility From EventHub", version);
            Console.WriteLine("Copyright (C) Yusuke.Yoneda");
            Console.WriteLine("");

        }
    }
}
