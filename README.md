NLog Azure Table Storage Target 
===============================

Azure Table Storage Target for NLog

For more info about NLog targets and how to use it, refer to <a href="https://github.com/nlog/NLog/wiki/How%20to%20write%20a%20Target">How To Write a Target</a>

How to use
==========
- Download package from <a href="https://www.nuget.org/packages/AzureTableStorageNLogTarget/">Nuget</a>.
- Open NLog configurations file ex: ```NLog.config``` and add the following sections:

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
            TableName="[[table-name]]" />
  </targets>
`````
Where ```[[target-name]]``` is a name you give for the target, ```[[connection-string-key]]``` is the key of Azure Storage Account connection string setting in App Settings or Cloud Service configuration file, and ```[[table-name]]``` is a name you give to the log table that will be created.
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
- Before running unit tests on you local machine, make sure Azure Storage Emulator is running.
