# SdHub
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/mixa3607/SdHub/build-container?style=flat-square)
![GitHub](https://img.shields.io/github/license/mixa3607/SdHub?style=flat-square)

## Как поднять dev среду
Требования:
- postgresql
- .net 6
- nodejs

Настройка:
- В `SdHub/appsettings.Development.json` подредактировать connection string'и для `Database` и `Hangfire`
- БД для `Hangfire` нужно создать руками, основная БД создасться и засидиться сама если есть права
- Серты для подписи жвт для dev окружения есть в репо, если нужно то новые можно сгенерировать скриптом в `/scripts`

Запуск:
- Экспортировать `ASPNETCORE_ENVIRONMENT` с значением `Development` или указать в профиле для студии
- Запустить `SdHub` из солюшена в `/Server` 
- Запустить `npm run start` из `/Client`
- Открыть http://localhost:5790 и войти с кредсами `Admin:strong_password`


## Настройки дсотупные в appsettings
|name|default|summary|
|-|-|-|
|**AppInfo**||Application info|
&nbsp;&nbsp;&nbsp;&nbsp;BaseUrl*||Base url
&nbsp;&nbsp;&nbsp;&nbsp;GitRefName*|"not_set"|Git ref. Fill from env if docker container
&nbsp;&nbsp;&nbsp;&nbsp;GitCommitSha*|"deadbeef"|Git sha. Fill from env if docker container
&nbsp;&nbsp;&nbsp;&nbsp;FrontDevServer||Use angular dev server instead compiled blobs. For development
&nbsp;&nbsp;&nbsp;&nbsp;DisableUsersRegistration|true|Disable user registration
|**Database**||Main db options|
&nbsp;&nbsp;&nbsp;&nbsp;ConnectionString*|""|Connection string
|**FileProcessor**||Files processings options|
&nbsp;&nbsp;&nbsp;&nbsp;CacheDir*|"./cache/upload"|Directory for temp files
&nbsp;&nbsp;&nbsp;&nbsp;PreserveCache|false|Don't delete cache
|**Hangfire**||Hangfire scheduler options. GUI on /hgf|
&nbsp;&nbsp;&nbsp;&nbsp;DatabaseConnectionString*||Database connection string
&nbsp;&nbsp;&nbsp;&nbsp;DatabaseSchema*|"public"|Database schema
&nbsp;&nbsp;&nbsp;&nbsp;RunServer|true|Run hangfire worker on backend
&nbsp;&nbsp;&nbsp;&nbsp;ServerName*|"bakend"|Worker name
|**Mailing**||Mailing options|
&nbsp;&nbsp;&nbsp;&nbsp;From*||Sender email
&nbsp;&nbsp;&nbsp;&nbsp;Host*||Mail server host
&nbsp;&nbsp;&nbsp;&nbsp;Port|587|Mail server port
&nbsp;&nbsp;&nbsp;&nbsp;Username*||Login
&nbsp;&nbsp;&nbsp;&nbsp;Password*||Password
&nbsp;&nbsp;&nbsp;&nbsp;EnableSsl|true|Enable ssl
&nbsp;&nbsp;&nbsp;&nbsp;UseMaildir|true|Использовать Maildir вместо отправки сообщений по сети
&nbsp;&nbsp;&nbsp;&nbsp;PathToMaildir*|"./maildir/"|Путь до корневой папки Maildir
&nbsp;&nbsp;&nbsp;&nbsp;MailTrustLevel|"Allow"|Есть установить то email'ы не будут проверяться на однодневки/подозрительные
&nbsp;&nbsp;&nbsp;&nbsp;TemplatesDir*|"./files/mailing/templates"|Templates directory
|**Recaptcha**||Google recaptcha options|
&nbsp;&nbsp;&nbsp;&nbsp;Bypass|true|Disable captcha
&nbsp;&nbsp;&nbsp;&nbsp;SecretKey*||Secret key
&nbsp;&nbsp;&nbsp;&nbsp;SiteKey*||Public site key
|**SdHubSeeder**||Database seeder options|
&nbsp;&nbsp;&nbsp;&nbsp;AdminPassword*||Password for Admin account
|**Swagger**||Swagger options|
&nbsp;&nbsp;&nbsp;&nbsp;Enable|false|Enable /swagger endpoint
|**WebSecurity**||Web security options|
&nbsp;&nbsp;&nbsp;&nbsp;EnableHttpsRedirections|true|Enable https redirection
&nbsp;&nbsp;&nbsp;&nbsp;EnableForwardedHeaders|true|Enable forwarded headers like X-Forwarded-For
&nbsp;&nbsp;&nbsp;&nbsp;Jwt|{}|Jwt auth options
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Issuers*|["SdHub"]|Issuers
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Issuer*|"SdHub"|Issuer
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Audiences*|["SdHub"]|Audiences
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ValidateLifetime|true|Validate lifetime
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;JwtLifetime|"00:05:00"|Lifetime
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RefreshTokenLifetime|"5.00:00:00"|Refresh token lifetime
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PfxPassword||Password for pfx cert. Can be null if not required
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PfxFile*||Path to pfx cert file
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;LogPii|false|Log more info about authorization
&nbsp;&nbsp;&nbsp;&nbsp;Cors|{}|CORS options
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AllowedHosts*|[]|Allowed hosts
|**Serilog**||Options for logging to ES. Used serilogs-sink-{assemblyName}-{0:yyyy.MM.dd} index|
&nbsp;&nbsp;&nbsp;&nbsp;DisableElastic|true|Disable logging to elastic search
&nbsp;&nbsp;&nbsp;&nbsp;ElasticUris*|[]|Urls to ES cluster
&nbsp;&nbsp;&nbsp;&nbsp;ElasticIndexPrefix||Append {prefix}- to index
&nbsp;&nbsp;&nbsp;&nbsp;LevelPreset|"Prod"|Predefined levels for logging AddCustomSerilog
&nbsp;&nbsp;&nbsp;&nbsp;EnableRequestLogging|true|Enable request logging
&nbsp;&nbsp;&nbsp;&nbsp;RequestLogMessageLevel|"Information"|Request message log level
