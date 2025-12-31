# Skinet Project

## Setup project

1. create root folder

```bash
mkdir skinet
```

2.create API folder:

```bash
dotnet new webapi -o API -controllers
```

3.create Core folder:

```bash
dotnet new classlib -o Core
```

4.create Infrastructure folder:

```bash
dotnet new classlib -o Infrastructure
```

5.Add solution file:

```bash
dotnet sln add API
dotnet sln add Core
dotnet sln add Infrastructure
```

chek solution list:

```bash
dotnet sln list
```

6.Add references:

```bash
cd API
dotnet add reference ../Infrastructure/
```

```bash
cd ../Infrastructure
dotnet add reference ../Core/
```

## Database

create migration:

```bash
dotnet ef migrations add InitialCreate -s API -p Infrastructure
```

-s : spacify startup project

-p : where db context is located

update database:

```bash
dotnet ef database update -s API -p Infrastructure
```
