nuget install NUnit.Runners -Version 3.12.0 -OutputDirectory tools
nuget install OpenCover -Version 4.7.1221 -OutputDirectory tools
nuget install coveralls.net -Version 3.0.0 -OutputDirectory tools
 
.\tools\OpenCover.4.7.1221\tools\OpenCover.Console.exe -target:.\tools\NUnit.ConsoleRunner.3.12.0\tools\nunit3-console.exe -targetargs:".\Mongo.Migration.Test\bin\Release\Mongo.Migration.Test.dll" -filter:"+[Mongo.Migration]* -[Mongo.Migration.Test]*" -register:user
 
.\tools\coveralls.net.3.0.0\tools\csmacnz.Coveralls.exe --opencover -i .\results.xml