call powershell -executionpolicy bypass -file %~1\generateTemplate.ps1 %~1 Salesforce.SDK.Core Core
call powershell -executionpolicy bypass -file %~1\generateTemplate.ps1 %~1 Salesforce.SDK.Phone Phone
call powershell -executionpolicy bypass -file %~1\generateTemplate.ps1 %~1 Salesforce.SDK.Store Store
call powershell -executionpolicy bypass -file %~1\packageTemplate.ps1 %~1