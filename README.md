# C#: usando Skip e Take com Entity Framework Core e SQL Server 2005 ou 2008

- [Requisitos](#requisitos)
- [Simulando o problema](#simulando-o-problema)
- [Ajustando o DbContext](#ajustando-o-dbcontext)
- [Observação](#observação)
- [Blog](#blog)

## Requisitos

- .NET Core Runtime 2.2.8 e SDK 2.2.207
- SQL Server 2008 Express

## Simulando o problema

- Abra o **Visual Studio Code**,
- Crie e abra a pasta **C:\GitHub\tiagopariz\CSharpEFSkipTakeSql2005e2008**,
- Execute os comandos a seguir:

```powershell
dotnet new globaljson --sdk-version 2.2.207
dotnet new sln
mkdir src
dotnet new console --output .\src\CSharpEFSkipTakeSql2005e2008.Prompt
dotnet sln add .\src\CSharpEFSkipTakeSql2005e2008.Prompt\
dotnet add ".\src\CSharpEFSkipTakeSql2005e2008.Prompt\CSharpEFSkipTakeSql2005e2008.Prompt.csproj" package "Microsoft.EntityFrameworkCore.SqlServer" --version 2.2.6
dotnet add ".\src\CSharpEFSkipTakeSql2005e2008.Prompt\CSharpEFSkipTakeSql2005e2008.Prompt.csproj" package "Microsoft.EntityFrameworkCore.Design" --version 2.2.6
dotnet add ".\src\CSharpEFSkipTakeSql2005e2008.Prompt\CSharpEFSkipTakeSql2005e2008.Prompt.csproj" package "Microsoft.EntityFrameworkCore.Tools" --version 2.2.6
dotnet add ".\src\CSharpEFSkipTakeSql2005e2008.Prompt\CSharpEFSkipTakeSql2005e2008.Prompt.csproj" package "Microsoft.Extensions.Configuration.FileExtensions" --version 2.2.6
dotnet add ".\src\CSharpEFSkipTakeSql2005e2008.Prompt\CSharpEFSkipTakeSql2005e2008.Prompt.csproj" package "Microsoft.Extensions.Configuration.Json" --version 2.2.6
```

- Na pasta **C:\GitHub\tiagopariz\CSharpEFSkipTakeSql2005e2008\src\CSharpEFSkipTakeSql2005e2008.Prompt** adicione o arquivo **appsettings.json**,

```javascript
{
    "Logging": {
        "LogLevel": {
            "Default": "Debug",
            "System": "Information",
            "Microsoft": "Information"
        }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
        "SkipTakeConnection": "Server=.\\SQLExpress;Integrated Security=true;Initial Catalog=SkipTakeDb"
    }
}
```

- Edite o arquivo de projeto **CSharpEFSkipTakeSql2005e2008.Prompt.csproj**, incluindo a referência ao arquivo **appsettings.json**,

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp3.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="3.1.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="3.1.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="3.1.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.Extensions.Configuration.FileExtensions" Version="3.1.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="3.1.0" />
  </ItemGroup>

  <ItemGroup>
    <EmbeddedResource Include="appsettings.json">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </EmbeddedResource>
  </ItemGroup>

</Project>
```

- Na pasta **C:\GitHub\tiagopariz\CSharpEFSkipTakeSql2005e2008\src\CSharpEFSkipTakeSql2005e2008.Prompt** adicione o arquivo **Person.cs**,

```CSharp
using System;

namespace CSharpEFSkipTakeSql2005e2008.Prompt
{
    public class Person
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }
    }    
}
```

- Na pasta **C:\GitHub\tiagopariz\CSharpEFSkipTakeSql2005e2008\src\CSharpEFSkipTakeSql2005e2008.Prompt** adicione o arquivo **SkipTakeContext.cs**,

```CSharp
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
 
namespace CSharpEFSkipTakeSql2005e2008.Prompt
{ 
    public class SkipTakeContext : DbContext
    {
        private readonly string _connectionString;
 
        public SkipTakeContext()
        {
            var configurationFile = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile($"appsettings.json")
                .Build();
 
            _connectionString = configurationFile
                                    .GetConnectionString("SkipTakeConnection");
 
        }
 
        public DbSet<Person> People { get; set; }
 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
```

- Abra o terminal e digite os comandos a seguir para criar o banco:

```powershell
dotnet ef migrations add InitialCreate --project .\src\CSharpEFSkipTakeSql2005e2008.Prompt
dotnet ef database update --project .\src\CSharpEFSkipTakeSql2005e2008.Prompt --verbose
```

```sql
USE [SkipTakeDb]
GO

INSERT INTO [dbo].[People]
           ([Id]
           ,[Name]
           ,[Age])
     VALUES 
		('6bb58044-f817-4729-b03d-b71b7f9001c8','João', 18),
		('bcb719f4-6374-4352-8f1e-4a3699eac4fe','Maria', 32),
		('bfa3cc30-acab-4570-ba47-3dc1d2027309','José', 45),
		('85a8fdd2-6211-4308-8a36-6323a8330114','Rodrigo', 8),
		('04ae1a1f-6fc7-41f7-aac5-37cad718156b','André', 20),
		('cc8efb11-5d0a-4f23-83c5-66a28feebd50','Letícia', 50),
		('5f9c8f34-815e-44e2-974c-4746a5b14583','Antônia', 22),
		('814d5bf3-a520-4e16-8103-67e226274344','Mia', 16),
		('437121bb-131c-4967-a365-998878a1c4d8','John', 39),
		('0c75b32d-a9f6-4856-ab9e-1ccd98cd50a0','Snake', 45),
		('94f7f221-f974-4538-a213-87c69c63fa43','Clarissa', 61),
		('627d15d1-8d0e-4dfb-a18c-bef33ae95542','Ziraldo', 28),
		('1a5772ca-6e27-4d0f-8d1d-043129466ba8','Cebolinha', 6),
		('76b5a2f6-b23a-4a02-9617-73086db2dbb2','Bianca', 18),
		('10f9c766-cf05-4902-9e0f-dc4fbbff1aec','Sindy', 32),
		('3e2a0bd2-211f-4e83-83d5-8f5f75361b62','Andrei', 18),
		('eb8ab1a6-2513-438d-b7cb-a106d233f533','Fabiano', 18),
		('7a42c0e9-49d2-46b8-ad71-0ce7124c94d7','Daniel', 18),
		('508e095c-7437-471c-8b67-198e43bd38b6','Falcão', 18),
		('d439c17f-f708-44a9-9c0e-8bce5ff7e0c0','Claudia', 18)
GO
```

- Altere a classe **Program.cs**, ordenando por nome, listando os primeiros 10 registros (**Take(10)**) e pulando os dois primeiros (**Skip(2)**),

```CSharp
using System;
using System.Linq;

namespace CSharpEFSkipTakeSql2005e2008.Prompt
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new SkipTakeContext();
            var people = dbContext.People
                                    .OrderBy(x => x.Name)
                                    .Take(10)
                                    .Skip(2);

            foreach (var person in people)
            {
                Console.WriteLine($"Name: {person.Name}\t | Age: {person.Age}\t | Id: {person.Id}");
            }            
        }
    }
}
```

- Execute o projeto com o comando a seguir:

```powershell
dotnet run --project .\src\CSharpEFSkipTakeSql2005e2008.Prompt\
```

- A seguinte mensagem de erro será exibida:

```
Unhandled exception. Microsoft.Data.SqlClient.SqlException (0x80131904): Incorrect syntax near 'OFFSET'.
   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.SqlDataReader.TryConsumeMetaData()
   at Microsoft.Data.SqlClient.SqlDataReader.get_MetaData()
   at Microsoft.Data.SqlClient.SqlCommand.FinishExecuteReader(SqlDataReader ds, RunBehavior runBehavior, String resetOptionsString, Boolean isInternal, Boolean forDescribeParameterEncryption, Boolean shouldCacheForAlwaysEncrypted)
   at Microsoft.Data.SqlClient.SqlCommand.RunExecuteReaderTds(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, Boolean isAsync, Int32 timeout, Task& task, Boolean asyncWrite, Boolean inRetry, SqlDataReader ds, Boolean describeParameterEncryptionRequest)
   at Microsoft.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, TaskCompletionSource`1 completion, Int32 timeout, Task& task, Boolean& usedCache, Boolean asyncWrite, Boolean inRetry, String method)
   at Microsoft.Data.SqlClient.SqlCommand.RunExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream, String method)
   at Microsoft.Data.SqlClient.SqlCommand.ExecuteReader(CommandBehavior behavior)
   at Microsoft.Data.SqlClient.SqlCommand.ExecuteDbDataReader(CommandBehavior behavior)
   at System.Data.Common.DbCommand.ExecuteReader()
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReader(RelationalCommandParameterObject parameterObject)
   at Microsoft.EntityFrameworkCore.Query.Internal.QueryingEnumerable`1.Enumerator.InitializeReader(DbContext _, Boolean result)
   at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.Execute[TState,TResult](TState state, Func`3 operation, Func`3 verifySucceeded)
   at Microsoft.EntityFrameworkCore.Query.Internal.QueryingEnumerable`1.Enumerator.MoveNext()
   at CSharpEFSkipTakeSql2005e2008.Prompt.Program.Main(String[] args) in C:\GitHub\tiagopariz\CSharpEFSkipTakeSql2005e2008\src\CSharpEFSkipTakeSql2005e2008.Prompt\Program.cs:line 16
ClientConnectionId:a199f978-2f56-458e-83fb-71502c001368
Error Number:102,State:1,Class:15
```

- O erro ocorre quando o Entity Framework Core tenta executar a seguinte consulta:

```sql
exec sp_executesql N'SELECT [t].*
FROM (
    SELECT TOP(@__p_0) [x].[Id], [x].[Age], [x].[Name]
    FROM [People] AS [x]
    ORDER BY [x].[Name]
) AS [t]
ORDER BY [t].[Name]
OFFSET @__p_1 ROWS',N'@__p_0 int,@__p_1 int',@__p_0=10,@__p_1=2
```

A clausula **OFFSET** é um recurso do **SQL Server 2012** e que é a forma padrão do .NET Core criar consultas usando **Take** e **Skip**.

No **SQL Server 2008** temos a função **Row_Number()** que atende o **Take** e **Skip**. Então, para que o EF Core use esta função, precisamos fazer um ajuste mínimo na classe de contexto.

## Ajustando o DbContext

- Edite o arquivo **SkipTakeContext.cs**, e no método **OnConfiguring()** altere a linha **optionsBuilder.UseSqlServer(_connectionString)** para que o Entity Framework Core use a função **Row_Number()** no lugar da claúsula **OFFSET**:

```CSharp
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
 
namespace CSharpEFSkipTakeSql2005e2008.Prompt
{ 
    public class SkipTakeContext : DbContext
    {
        private readonly string _connectionString;
 
        public SkipTakeContext()
        {
            var configurationFile = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile($"appsettings.json")
                .Build();
 
            _connectionString = configurationFile
                                    .GetConnectionString("SkipTakeConnection");
 
        }
 
        public DbSet<Person> People { get; set; }
 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString, builder => builder.UseRowNumberForPaging());            
            base.OnConfiguring(optionsBuilder);
        }
    }
}
```

- Execute novamente a aplicação:

```powershell
dotnet run --project .\src\CSharpEFSkipTakeSql2005e2008.Prompt\
```

- Agora os resultados serão exibidos corretamente,

```
Name: Antônia    | Age: 22       | Id: 5f9c8f34-815e-44e2-974c-4746a5b14583
Name: Bianca     | Age: 18       | Id: 76b5a2f6-b23a-4a02-9617-73086db2dbb2
Name: Cebolinha  | Age: 6        | Id: 1a5772ca-6e27-4d0f-8d1d-043129466ba8
Name: Clarissa   | Age: 61       | Id: 94f7f221-f974-4538-a213-87c69c63fa43
Name: Claudia    | Age: 18       | Id: d439c17f-f708-44a9-9c0e-8bce5ff7e0c0
Name: Daniel     | Age: 18       | Id: 7a42c0e9-49d2-46b8-ad71-0ce7124c94d7
Name: Fabiano    | Age: 18       | Id: eb8ab1a6-2513-438d-b7cb-a106d233f533
Name: Falcão     | Age: 18       | Id: 508e095c-7437-471c-8b67-198e43bd38b6
```

## Observação

Não há suporte para SQL Server 2005 e 2008 a partir do .NET Core 3.0.

## Veja no blog

 - <https://blog.tiagopariz.com/c-usando-skip-e-take-com-entity-framework-core-e-sql-server-2005-ou-2008>