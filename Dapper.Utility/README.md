# RS.Dapper.Utility

A lightweight utility library built on top of **Dapper** to simplify and standardize database operations across **MSSQL**, **PostgreSQL**, and **MySQL**. All are managed by appsettings.json and DbSchema.cs class

---

## ✅ Features

- `IDapperExecutor` for executing SQL queries and stored procedures easily.
- `IDapperRepository` for generic CRUD operations along with pagination and search.
- Support for:
  - `ExecuteAsync`
  - `ExecuteScalarAsync`
  - `QueryAsync`
  - `QueryFirstOrDefaultAsync`
  - `QueryMultipleAsync`
- Works with **DynamicParameters** and anonymous objects.
- Designed for **Dependency Injection** and **unit testing**.

---

## 📦 Installation

Install from **NuGet** (https://www.nuget.org/packages/RS.Dapper.Utility):

```bash
dotnet add package RS.Dapper.Utility
```

---

## ⚡ Quick Start
Add your database settings in appsettings.json.
``` javascript
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-FFBIFD1\\SQLEXPRESS;Database=AppFoundationDb;Trusted_Connection=True;TrustServerCertificate=True;"
   //"DefaultConnection": "server=000.000.00.000;User ID=rsinnov7_demo2025;password=xxxxxxxx;database=rsinnov7_demo2025;"//MYSQL
   //"DefaultConnection": "Host=db.lkjinmrmovukgciyfvml.supabase.co;Database=postgres;Username=postgres;Password=gHfFvMjK1PTnaF8q;SSL Mode=Require;Trust Server Certificate=true" //PostgreSql
  },
  "DatabaseSettings": {
    "Type": "SqlServer" //SqlServer/PostgreSql/MySql
  } 
}

```
### 1. Register Services

Add to your **Program.cs**:
- Download DbSchema.cs class from GitHub and add in your poroject [DbSchema.cs](https://github.com/ravinder25886/ProjectArchitecture/blob/main/ProjectName.DataAccess/Constants/DbSchema.cs)
- Setup DbSchema for RS_DapperUtility, it is part of Utility but it is open to manage by developer as they have to add table(s)and proc(s) names
```csharp
DbSchema.Initialize(builder.Configuration);
```

```csharp
//Inject RS_DapperUtilityDependencyInjections
builder.Services.RS_DapperUtilityDependencyInjections(builder.Configuration);
```
```csharp
//Start-****************Setup SqlBuilder IMemoryCache for RS_DapperUtility **************
var memoryCache = app.Services.GetRequiredService<IMemoryCache>();
SqlBuilder.Initialize(memoryCache);
//End-*****************Setup SqlBuilder IMemoryCache for RS_DapperUtility****************
```
- For more detail check [API Program.cs class on GitHub](https://github.com/ravinder25886/ProjectArchitecture/blob/main/ProjectName.API/Program.cs)
---
### How to use RS.Dapper Attributes 
Please use ToParametersForInsert and ToParametersForUpdate, it helps for auto column mapping, and use Attributes like following:-
- [IgnoreOnInsert]  // Exclude Id during insert, if it auto generated, else set your Id in Service class such as GUID
- [SqlParam("full_name")]//Just for example, we will use param full_name in PROC or it will also create select/insert prop mapping in SQL query.
```csharp
  [IgnoreOnInsert]  // Exclude Id during insert
  public virtual long Id { get; set; }
  [SqlParam("Icon")]// In this case we have Icon column name in database and prop name is Image 
  public string Image { get; set; }
```

### 2. Example: Execute Stored Procedure

```csharp
await _dapperExecutor.ExecuteAsync(
    DbSchema.UserInsertProc,//"User_Insert",
    user.ToParametersForInsert() 
    );
```

---

### 3. Example: Insert using Repository
💡 In today’s world, databases are already highly optimized and blazing fast. For common operations like Insert, Update, Delete, or Select, we don’t always need to write stored procedures anymore.

With RS.Dapper.Utility, we can achieve all of this dynamically and cleanly — without the extra overhead of manual SQL or stored procs.

🚀 Good news! RS.Dapper.Utility now supports a Paging SQL Query Builder 🎉. You can also define your own search columns for flexible filtering.

⚠️ Note: When working with data across multiple tables, it’s still best to use a stored procedure(chcek 4 point) or write a raw query — and then execute it using RS.Dapper.Utility’s DapperExecutor.

🚀 OMG one line code and we are done with CURD 
```csharp
 await _dapperRepository.InsertAsync(request, DbSchema.CategoryTable);
 await _dapperRepository.UpdateAsync(request, DbSchema.CategoryTable);
 await _dapperRepository.DeleteAsync(id, DbSchema.CategoryTable);
 await _dapperRepository.GetAllAsync<CategoryResponse>(DbSchema.CategoryTable);
 await _dapperRepository.GetByIdAsync<CategoryModel,int>(id, DbSchema.CategoryTable);
 await _dapperRepository.GetPagedDataAsync<CategoryResponse>(DbSchema.CategoryTable, pagedRequest)
```

---

### 4. Query Multiple Result Sets with PROC

```csharp
using var multi = await _dapperExecutor.QueryMultipleAsync(
    "User_GetWithRoles",
    new { Id = 1 },
    commandType: CommandType.StoredProcedure
);

var user = await multi.ReadFirstOrDefaultAsync<UserModel>();
var roles = await multi.ReadAsync<RoleModel>();
```

---

## 📂 Project Structure

```
Repositories/
├── Executor/
│   ├── IDapperExecutor.cs
│   └── DapperExecutor.cs
├── Repository/
│   ├── IDapperRepository.cs
│   └── DapperRepository.cs
```

---

## ✅ License

This project is licensed under the **MIT License**.

SPDX-License-Identifier: `MIT`

See the [LICENSE](LICENSE) file for details.

---

## 🔗 Links

- [Dapper on GitHub](https://github.com/ravinder25886/ProjectArchitecture/tree/main/Dapper.Utility)
- [SPDX License List](https://spdx.org/licenses/)
- 
## ✅ How I am using RS.Dapper.Utility
I use RS.Dapper.Utility in my projects, and it saves me a huge amount of development and testing time.

👉 For 95% of CRUD operations, I rely on IDapperRepository.
👉 For the remaining 5%, I use IDapperExecutor when I need to work with stored procedures.

I’m not against stored procedures—they’re still great for complex queries across multiple tables (insert/update/select). But with RS.Dapper.Utility, you have the freedom to choose:

Go fully with IDapperRepository for simplicity and speed

Or combine it with IDapperExecutor where it makes sense

⚡ One of the biggest advantages: Database switching.

If your app is 100% on IDapperRepository, moving from SQL Server to MySQL or PostgreSQL takes just 5 minutes.

Even if you’re 95% repository + 5% stored procedures, it still only takes about 1 hour.

📌 RS.Dapper.Utility is open-source and open for contributions.
You can download, customize, and enhance it:

🔗[RS.Dapper.Utility on GitHub](https://github.com/ravinder25886/ProjectArchitecture/tree/main/Dapper.Utility)

💡 Bonus: On the same GitHub repo, I also share ready-to-use project templates and working code samples—new content added every week to help developers save time.

🔗 [Full Project on GitHub](https://github.com/ravinder25886/ProjectArchitecture/tree/main)

🔥 If you’re a .NET developer looking for speed, flexibility, and clean database access with Dapper—RS.Dapper.Utility is built for you.

☕ [Buy Me A Coffee](https://buymeacoffee.com/ravinder25z)
Your support motivates me to keep adding more developer-friendly utilities 🚀