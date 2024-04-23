#this path contains an image rep....net 6 framework used to compile, build, publish .net applications
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
# creating a folder inside the image that will host my website files
WORKDIR /App

# Copy everything
COPY . ./
# restore means it will go inside the .csproj, read the references, download the libraries and install them in the image file
RUN dotnet restore
# publish > it compiles and output in the out folder a dll/exe file
RUN dotnet publish -c Release -o out

# so.... in the image it will contain the microsoft .net 6 runtime files (this in size is much smaller than the sdk)
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
# we copy the compiled files into the out folder again inside the image
COPY --from=build-env /App/out .

#exposing an internal port 80 in the image inside a container
EXPOSE 80

#entrypoint is set to run the command dotnet <name of the application>
ENTRYPOINT ["dotnet", "WebApplication1.dll"]
 