NLog Azure Table Storage Target 
===============================

Azure Table Storage Target for NLog

For more info about NLog targets and how to use it, refer to <a href="https://github.com/nlog/NLog/wiki/How%20to%20write%20a%20Target">How To Write a Target</a>

Download
==========
Download package from <a href="https://www.nuget.org/packages/AzureTableStorageNLogTarget/">Nuget</a>

How to use
==========
- Download package from <a href="https://www.nuget.org/packages/AzureTableStorageNLogTarget/">Nuget</a>.
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
			PartitionKeyPrefix="[[partition-key-value]]"
			PartitionKeyPrefixKey="[[partition-key-configuration-key]]" />
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
A lot of ways you can access table storage, however using <a href="http://azurestorageexplorer.codeplex.com/">Azure Storage Explorer</a> is more than enough to view logs.

Notes
=====
- Before running tests on you local machine, make sure Azure Storage Emulator is running.
