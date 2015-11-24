NLog Azure Table Storage Target updated by Greenliff
====================================================

Azure Table Storage Target for NLog

For more info about NLog targets and how to use it, refer to <a href="https://github.com/nlog/NLog/wiki/How%20to%20write%20a%20Target">How To Write a Target</a>

Original source
===============
Based on the source available at <a href="https://github.com/harouny/NLog.Extensions.AzureTableStorage">Github</a>

Modifications
=============
- Support for ASP.NET 5 Beta8
- Support for NLog 4.2.1
- Support for WindowsAzure.Storage 6.1.1-preview
- Note that the latest available package Microsoft.WindowsAzure.ConfigurationManager 3.1.0 does not support DNX Core 5.0
- app.config has been replaced with appsettings.json, see Startup.cs in project NLog.Extensions.AzureTableStorage.EmptyApplication and AzureTableStorageTargetTests.cs in project NLog.Extensions.AzureTableStorage.Tests

How to use
==========
- There is currently no Nuget package for download available. Add the project NLog.Extensions.AzureTableStorage to your solution. Add the following dependency to your project.json file: "NLog.Extensions.AzureTableStorage": ""
- Configure your application for logging (appsettings.json, NLog.config, create logger). See sample code in Startup.cs of included project NLog.Extensions.AzureTableStorage.EmptyApplication 
- If you didn't setup Azure storage connection string yet, follow instructions in <a href="http://msdn.microsoft.com/en-us/library/azure/ee758697.aspx">Setup a storage connection string</a>.
- Open NLog configurations file ex: ```NLog.config``` and add the following lines:
- Add ```NLog.Extensions.AzureTableStorage``` assemply to ```extensions``` section:
`````xml
  <extensions>
    <add assembly="NLog.Extensions.AzureTableStorage"/>
  </extensions>
`````
- Add ```AzureTableStorage``` target to ```targets``` section:
`````xml
  <targets>
    <target xsi:type="AzureTableStorage" 
            name="[[target-name]]"
            ConnectionStringKey="[[connection-string-key]]" 
            TableName="[[table-name]]"
			PartitionKeyPrefix="[[partition-key-prefix-value]]"
			PartitionKeyPrefixKey="[[partition-key-prefix-configuration-key]]" />
  </targets>
`````
Where ```[[target-name]]``` is a name you give for the target, ```[[connection-string-key]]``` is the key of Azure Storage Account connection string setting in App Settings or Cloud Service configuration file, and ```[[table-name]]``` is a name you give to the log table that will be created.

If multiple applications need to share the same storage account it is possible to prefix the partition keys used with a custom string.
```[[PartitionKeyPrefix]]``` and ```[[PartitionKeyPrefixKey]]``` are optional and ```[[PartitionKeyPrefixKey]]``` has precedence over a hard coded value in ```[[PartitionKeyPrefix]]```. 
- Add a rule that uses the target in ```rules``` section.
`````xml
  <rules>
    <logger name="*" minlevel="Trace" writeTo="[[target-name]]" />
  </rules> 
`````

Note: You can also configure this by code refer to <a href="https://github.com/nlog/NLog/wiki/How%20to%20write%20a%20Target">How To Write a Target</a> for more information.

How to View Logs?
=================
A lot of ways you can access table storage.
- <a href="http://www.cloudportam.com/">Cloud Portam</a>
- <a href="http://azurestorageexplorer.codeplex.com/">Azure Storage Explorer</a>

Notes
=====
- Before running tests on you local machine, make sure Azure Storage Emulator is running.
