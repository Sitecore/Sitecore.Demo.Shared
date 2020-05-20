FROM mcr.microsoft.com/powershell:nanoserver-1809 AS build
ARG NODEJS_VERSION
ARG BAGET_TAG

# RUN single command to 1) Download required version of Node.js (LTS) binaries as zip, 2) extract zip file, 3) move contents to standardised folder name for later copying, 4) download required version of BaGet and 5) extract zip file
RUN pwsh -Command Invoke-WebRequest -UseBasicParsing https://nodejs.org/dist/%NODEJS_VERSION%/node-%NODEJS_VERSION%-win-x64.zip -OutFile node.zip && \
pwsh -Command Expand-Archive node.zip -DestinationPath C:\Expanded && \
move C:\\Expanded\\node-%NODEJS_VERSION%-win-x64 C:\\nodejs && \
pwsh -Command Invoke-WebRequest -UseBasicParsing https://github.com/loic-sharma/BaGet/releases/download/%BAGET_TAG%/BaGet.zip -OutFile BaGet.zip && \
pwsh -Command Expand-Archive BaGet.zip -DestinationPath C:\BaGet

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-1809
# Node.js: copy binaries and re-set path (using standard default path value found within image + Node.js path)
COPY --from=build ["/nodejs", "C:/Program Files/nodejs"]
ENV PATH "C:\Windows\system32;C:\Windows;C:\Program Files\dotnet;C:\Program Files\nodejs;"
# BaGet
WORKDIR /app
COPY --from=build /BaGet .
EXPOSE 80
ENTRYPOINT ["dotnet", "BaGet.dll"]