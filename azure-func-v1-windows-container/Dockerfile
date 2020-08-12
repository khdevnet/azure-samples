FROM stefanscherer/node-windows:12.18.3-windowsservercore-2019
SHELL ["powershell", "-Command"]

ADD http://download.microsoft.com/download/0/5/6/056dcda9-d667-4e27-8001-8a0c6971d6b1/vcredist_x86.exe C:\\vcredist_x86.exe

RUN .\vcredist_x86.exe /q ; Remove-Item vcredist_x86.exe

RUN npm i -g azure-functions-core-tools@1.0.19

EXPOSE 80/tcp

COPY ./bin/Debug/net461/. .

ENTRYPOINT func host start -p 80
