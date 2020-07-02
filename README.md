# Social Activities

> App for creating and managing managing social activities

**Stack**
- Clean Architecture
- CQRS and Mediator Pattern
- .NET Core
- Entity Framework
- AutoMapper
- MediatR
- SignalR
- React
- MobX
- etc.

## Running the source code

 1. At the root level run the command `dotnet restore`
 2. Change directory into the Client and run `npm install`
 3. Modify appsettings.json and add `"TokenKey": "secretkey",`
 4. If you want to see the photo upload working then you will also need a Cloudinary account and add the following in the appsettings.json (obviously replace the Cloudinary settings with your details).  The project will work without these but you will not be able to use the photo upload feature.
 ```javascript
"Cloudinary": {
	"CloundName": "YourCloudName",
	"ApiSecret": "YourApiSecret",
	"ApiKey": "YourApiKey"
}
```
 6. You should now be able to start the Api. This will also create a Sqlite DB file inside the Api project and seed some initial data into the database.
 7. Change directory into the Api and run the command `dotnet run`
 8. You should now see the API project running on Port 5000.
 9. In a separate terminal session, change the directory into the Client and run the command `npm start`
 10. This should start the React application on Port 3000. and you should now be able to browse to the application on the following URL: `http://localhost:3000`
 11. You can login to the application using one of the test user accounts that was seeded when you started the API. You can use the following test user: 

> email:  bob@test.com
> password:  Pa$$w0rd

## Facebook login

 To be able to use facebook login you need to register the application with facebook and then add the following in the appsettings.json with your AppId and AppSecret provided by facebook
  ```javascript
"Authentication": {
	"Facebook": {
		"AppId": "YourAppId",
		"AppSecret": "YourAppSecret"
	}
},
```
