@echo off
dotnet tool restore
dotnet fake -v run build.fsx