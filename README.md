# SqlCE-Wmi-Test

## 1. Efficiency of SQL CE compared with WMI (currently used)

### [Test Data]
### *WMI*    
```
class PluginExecuteMetrics
{
	[key] string PluginId;
	string PluginName;
	uint32 ExecuteTotalCount;
	uint32 ExecuteExceptionCount;
	uint64 ExecuteMinDuration;
	uint64 ExecuteAvgDuration;
	uint64 ExecuteMaxDuration;
};
```

### *SQL CE*
```
class PluginExecuteMetrics
{
	[key] string PluginId;
	string PluginName;
	INT ExecuteTotalCount;
	INT ExecuteExceptionCount;
	BIGINT ExecuteMinDuration;
	BIGINT ExecuteAvgDuration;
	BIGINT ExecuteMaxDuration;
}
```


**Notice:**
* Pluginid is generated randomly using Guid.NewGuid()
* PluginName and int are default value.
* using System.Data.SqlServerCe and System.Management 

### [EXAMPLE]

insert 5000 plugin metrics execute result into SQL CE table (with random PluginId and default count)
result:
![image](https://user-images.githubusercontent.com/67184811/186099220-ac9975de-88e6-4878-9b0d-dc823cd93fde.png)

insert 5000 plugin metrics execute result into WMI table(with random PluginId and default count)
result:
![image](https://user-images.githubusercontent.com/67184811/186099240-c413f7fb-82b8-42c1-bd42-73ba757b5090.png)

	
	
### [INSERT]
![image](https://user-images.githubusercontent.com/67184811/186100609-9467394b-427a-4360-acc6-7fecdc98f962.png)

### [DELETE]
![image](https://user-images.githubusercontent.com/67184811/186100695-d42c725f-067a-4949-9aa3-28754fdb8ada.png)

### [UPDATE]
![image](https://user-images.githubusercontent.com/67184811/186100726-dc9d0670-f450-49b7-8e9b-a479f6bab64c.png)

### [SEARCH]
![image](https://user-images.githubusercontent.com/67184811/186100799-38a1e58a-54f5-4684-9b6f-b4fd35e41106.png)


## 2.availability

### [schema changed]

*SQL CE*  
* still available when add/drop/alter a column 
* using sql "ALTER TABLE ADD/DROP/ALTER COLUMN" **( no data in this column)**

*WMI* 
* needs to compile the MOF file **(no instance in this class)**


## 3.Install on Client's PC

* SQL CE needs an extra .exe installation package on client's PC.
* 2.5MB SSCERuntime-ENU.msi 
* msi package With WiX : 34KB(original test code) -> 2.49MB

## 4.Feasibility
	
* to what extend support SQL
[SQL Server Compact 与 SQL Server 之间的差异](https://docs.microsoft.com/zh-cn/previous-versions/sql/compact/sql-server-compact-4.0/bb896140(v=sql.110))

* NOT Support: ORDER BY/Stored Procedure/...

* any other use of WMI ? (get process id / os version /)

## 5.Other choices
- Firebird Embedded
- SQLite
- TurboSQL

## 6.CPU Usage

**Notice**
* both use `Resource Monitor` to check the avg CPU usage
* the whole .exe file includes create database and insert/update/search/delete operation(5000 times each) and a Console output

**SQLCE** : 12%

**WMI** :6.5%

## 7.Memory Usage

**Notice**
* both use `System.Diagnostics.Process.PeakWorkingSet`
* the whole .exe file includes create database and insert/update/search/delete operation(5000 times each) and a Console output

**SQLCE** : 69MB

**WMI** :30.2MB

conclusion: The CPU/Memory Usage of Wmi is obviously better than SQLCE.


