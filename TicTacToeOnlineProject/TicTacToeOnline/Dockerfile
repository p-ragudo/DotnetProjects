FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TicTacToeOnline.csproj", "./"]
RUN dotnet restore "TicTacToeOnline.csproj"
COPY . .
RUN dotnet publish "TicTacToeOnline.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TicTacToeOnline.dll"]