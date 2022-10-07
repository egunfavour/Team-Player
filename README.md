# Player selection

## Build the solution
This solution requires .NET 6 installed.

To build the solution run from the project folder
```shell
dotnet build
```

## Run solution locally
To run the solution locally just run this command from the project folder (WebApi folder)

```shell
dotnet run
```

## Run tests
To run the tests just run this command 

```shell
dotnet test
```

## Migration
Note that dotnet-ef tools is required to run the migration, run the following to install the tool
```shell
dotnet tool install --global dotnet-ef
``` 

### Add migration 
To add the migration, run this command from the project folder (WebApi folder)
```shell
dotnet ef migrations add InitialCreate
```
where InitialCreate is the  name of the migration

### Update database
```shell
dotnet ef database update
```

