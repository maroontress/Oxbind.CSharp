#!/bin/sh
dotnet ~/.nuget/packages/reportgenerator/4.0.14/tools/netcoreapp2.0/ReportGenerator.dll \
  --reports:Oxbind.Test/coverage.opencover.xml --targetdir:Coverlet-html
