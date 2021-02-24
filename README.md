# EventHubsClient
Very simple sender and receiver client for Azure Event Hubs. This source code is available to everyone under the standard [MIT license](./LICENSE).


# EventHubs Sender

```
  _____                 _   _   _       _       ____                 _           _
 | ____|_   _____ _ __ | |_| | | |_   _| |__   / ___|  ___ _ __   __| | ___ _ __| |
 |  _| \ \ / / _ \ '_ \| __| |_| | | | | '_ \  \___ \ / _ \ '_ \ / _` |/ _ \ '__| |
 | |___ \ V /  __/ | | | |_|  _  | |_| | |_) |  ___) |  __/ | | | (_| |  __/ |  |_|
 |_____| \_/ \___|_| |_|\__|_| |_|\__,_|_.__/  |____/ \___|_| |_|\__,_|\___|_|  (_)


EventHubSender v0.0.1 - simple send messages utility to EventHub
Copyright (C) Yusuke.Yoneda
```


## Usage
you can dowload ***ehsend.exe*** and a sample file of ***sender.config.json*** from release page. you just run ehsend.exe from powershell or command prompt.
you have to specify a few parameters to send message. When you set up the parameters, a config file(./sender.config.json) or cmdline args is available.
The sample of sender.config.json is the following. please mind that sender.config.json must be stored under the current directory.

```json
{
  "count": 5,
  "msgprefix": "<MessagePrefix>"
  "eventHubName": "<YourEventHubsName>",
  "connectionString": "Endpoint=sb://xxxx.servicebus.windows.net/;SharedAccessKeyName=xxxx;SharedAccessKey=xxxxx;EntityPath=xxx"
}
```

If ./sender.config.json exists, the values are loaded firstly from sender.config.json. If you specify the parameter on cmdline args, the value will be overwrited.

```
EventHubSender usage: ehsend [-c/--count MessageCount] [-p/--prefix MessagePrefix] [-n/--name EventHubName] [-s/--connectionstring ConnectionString]
 -c     the number of messages sent to EventHub. default value is 10.
 -p     message prefix. If Prefix is "hoge", the mssage is "hoge yyyy/mm/dd hh:mm:ss
 -n     eventhub name for the destination
 -s     connection string of EventHub
```

example
```sh
# send 3 messages, other parameters are used from ./sender.config.json
ehsend -c 3

# all parameters are specified on cmdline args
ehsend -c 5 -p hoge -n functest -s "Endpoint=sb://xxxx.servicebus.windows.net/;SharedAccessKeyName=xxxx;SharedAccessKey=xxxx;EntityPath=xxx"
```

# EventHubs Receiver

```
 _____                 _   _   _       _       ____               _                _
 | ____|_   _____ _ __ | |_| | | |_   _| |__   |  _ \ ___  ___ ___(_)_   _____ _ __| |
 |  _| \ \ / / _ \ '_ \| __| |_| | | | | '_ \  | |_) / _ \/ __/ _ \ \ \ / / _ \ '__| |
 | |___ \ V /  __/ | | | |_|  _  | |_| | |_) | |  _ <  __/ (_|  __/ |\ V /  __/ |  |_|
 |_____| \_/ \___|_| |_|\__|_| |_|\__,_|_.__/  |_| \_\___|\___\___|_| \_/ \___|_|  (_)


EventHubReceiver v0.0.1 - simple receive messages utility From EventHub
Copyright (C) Yusuke.Yoneda
```

## Usage
you can dowload ***ehreceive.exe*** and a sample of ***receiver.config.json*** from release. you just run ehsend.exe from powershell or command prompt.   
You must set up the config file(./receiver.config.json). The sample of receiver.config.json is the following. please mind that receiver.config.json must be stored under the current directory.

```json
{
  "eventHubName": "<YourEventHubsName>",
  "eventHubConnectionString": "Endpoint=sb://xxxx.servicebus.windows.net/;SharedAccessKeyName=xxxx;SharedAccessKey=xxxxx;EntityPath=xxx",
  "consumergroup": "", //Eventhubs Consumer Group. Default value is $default.
  "blobStorageConnectionString": "DefaultEndpointsProtocol=https;AccountName=xxx;AccountKey=xxx;EndpointSuffix=core.windows.net",
  "blobContainerName": ""
}
```

