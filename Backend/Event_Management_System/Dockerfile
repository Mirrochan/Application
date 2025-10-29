FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src


COPY ./Event_Management_System/*.csproj ./Event_Management_System/
COPY ./Infrastructure/*.csproj ./Infrastructure/
COPY ./Application/*.csproj ./Application/
COPY ./Domain/*.csproj ./Domain/
RUN dotnet restore ./Event_Management_System/Event_Management_System.csproj


COPY . .
RUN dotnet publish ./Event_Management_System/Event_Management_System.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5107
EXPOSE 5107

ENTRYPOINT ["dotnet", "Event_Management_System.dll"]
