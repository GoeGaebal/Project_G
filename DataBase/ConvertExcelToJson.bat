@echo off

set current_folder=%cd%
for %%a in ("%current_folder%") do set folder_name=%%~nxa
echo Current folder name: %folder_name%
if "%folder_name%" neq "DataBase" cd DataBase

set "inputDirectory=%Data"
set "outputDirectory=%tempcsvs"
set "outputJsonDirectory=%.\\..\\Assets\\Resources\\Data"
set "outputCsDirectory=%.\\..\\Assets\\Scripts\\Data
set "converter=%ExcelToCsvConverter.vbs"
set "converter2=%CSVToJsonConverter.exe"

if not exist "%outputDirectory%" mkdir "%outputDirectory%"
if not exist "%outputJsonDirectory%" mkdir "%outputJsonDirectory%"
if not exist "%outputCsDirectory=%" mkdir "%outputCsDirectory%"

del /s /q "%outputDirectory%\*.*"

for %%F in ("%inputDirectory%\*.xlsx") do (
    cscript "%converter%" "%cd%\%%F" "%cd%\%outputDirectory%\%%~nF.csv"
)

echo CSV files created successfully!

%converter2% %outputDirectory% %outputCsDirectory% %outputJsonDirectory%

pause