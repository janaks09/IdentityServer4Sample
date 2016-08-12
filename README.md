# IdentityServer4Sample
Simple Auth server sample using Identity Server 4 with ASP.Net Identity Membership.

## Currently support for Resource Owner Password Flow only.

## Set up
- Do all download or clone stuff
- Restore and build solution
```
dotnet restore
dotnet build
```
- Migrate database

For user database we used ASP.Net Identity membership service. This solution aleardy contains migrations snapshot so you can update your database only by
```
dotnet ef database update
```
If you want to see how migartion work then delete migration folder and use following commands in dotnet CLI
```
dotnet ef migrations add InitialCreate (InitialCreate is current migartion snapshot name)
dotnet ef database update
```
Now you have migrated user database which will be used by Identity server for authentication process. Now you need user in database show that Idsrv can valid user auth request against users in database. Quick way to insert user is get MVCClient site with individual authentication, point same database as of IdentityServer, do database migration as like above and register few user.

Now create payload using postman and get your JWT token.

For payload (In postman)

- Prepare post request with url [base_url]/connect/token
- Payload body and response
![alt tag](https://zsrestawp.blob.core.windows.net/misc/idservpayload.png)

That's it. Now you can improve this solution as per your need.

# TODO
- Add support for other OAuth flow
- Custom client management
