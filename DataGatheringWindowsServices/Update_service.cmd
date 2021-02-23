rem Update service binary
@echo off
sc stop BKV_DataGatheringWindowsServices
timeout 10
del C:\WindowsServices\DataGatheringWindowsServices\*.* /Q /S
xcopy C:\Source\WebPageParser\DataGatheringWindowsServices\bin\Release\netcoreapp3.1 C:\WindowsServices\DataGatheringWindowsServices\
sc start BKV_DataGatheringWindowsServices
timeout 5
pause

rem Add environment variables
rem setx WebParser_AppSecret__SQliteConnectionString "Filename=C:\WindowsServices\SQLite_Database.db" /M
rem setx WebParser_AppSecret__EmailLogin "abc@gmail.com" /M
rem setx WebParser_AppSecret__EmailPassword "123" /M

rem Add service
rem sc create DataGatheringWindowsServices binPath= C:\WindowsServices\DataGatheringWindowsServices\DataGatheringWindowsServices.exe

rem Remove service
rem sc delete DataGatheringWindowsServices