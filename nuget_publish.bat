set version=1.0.0

dotnet nuget push Nuget\Bodoconsult.Database.%version%.nupkg --source https://api.nuget.org/v3/index.json
dotnet nuget push Nuget\Bodoconsult.Database.%version%.snupkg --source https://api.nuget.org/v3/index.json
dotnet nuget push Nuget\Bodoconsult.Database.SqlClient.%version%.nupkg --source https://api.nuget.org/v3/index.json
dotnet nuget push Nuget\Bodoconsult.Database.SqlClient.%version%.snupkg --source https://api.nuget.org/v3/index.json
dotnet nuget push Nuget\Bodoconsult.Database.Postgres.%version%.nupkg --source https://api.nuget.org/v3/index.json
dotnet nuget push Nuget\Bodoconsult.Database.Postgres.%version%.snupkg --source https://api.nuget.org/v3/index.json
dotnet nuget push Nuget\Bodoconsult.Database.Sqlite.%version%.nupkg --source https://api.nuget.org/v3/index.json
dotnet nuget push Nuget\Bodoconsult.Database.Sqlite.%version%.snupkg --source https://api.nuget.org/v3/index.json
pause