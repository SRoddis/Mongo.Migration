nuget install NUnit.Runners -Version 3.10.0 -OutputDirectory tools
nuget install OpenCover -Version 4.67.882 -OutputDirectory tools
nuget install coveralls.net -Version 1.0.0 -OutputDirectory tools
 
.\tools\OpenCover.4.67.882\tools\OpenCover.Console.exe -target:.\tools\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe -targetargs:".\Mongo.Migration.Test\bin\Release\Mongo.Migration.Test.dll" -filter:"+[Mongo.Migration]* -[Mongo.Migration.Test]*" -register:user
 
.\tools\coveralls.net.1.0.0\tools\csmacnz.Coveralls.exe --opencover -i .\results.xml