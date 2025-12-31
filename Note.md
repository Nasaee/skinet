# Skinet Project

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
