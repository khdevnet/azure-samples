## Run function v1 and v3 using azure function cli tools 
* Install 2 versions of cli tools using npm aliases
```
npm i -g azfunc3@npm:azure-functions-core-tools@3 azfunc1@npm:azure-functions-core-tools@1.0.19 --force --unsafe-perm true
```
* create azurefunc.ps1 script, run in parallel process
```powershell
$rootPath = "MyProject\backend"
$func1exe = $env:APPDATA + "\nvm\v12.16.1\node_modules\azfunc1\bin\func.exe"
$func3exe = $env:APPDATA + "\nvm\v12.16.1\node_modules\azfunc3\bin\func.exe"

Start-Process -NoNewWindow -FilePath $func3exe -ArgumentList 'start','--script-root',"$($rootPath)\FunctionVersion3\bin\Debug\netcoreapp3.1\"

Set-Location -Path $rootPath"\FunctionVersion1\"
Start-Process -NoNewWindow -FilePath $func1exe -ArgumentList "host", "start"
```