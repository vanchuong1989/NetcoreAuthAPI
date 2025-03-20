# 1/API CRUD by using AutoMapper
# 2/Log monitor by using SeriLog and Serilog.Sinks.File
# 3/Enable CORS: this is security features => basically, it is blocking online domains not in allow domain list
# 4/Rate Limiting:
	-this is a strategy for limiing network traffic
	- can help us to stop kinds of malicious bot activity
	- can also reduce strain on web server (cũng có thể giảm bớt nặng nề cho máy chủ web)

# 5/Basic Authentication
		-Basic Auth sends usernames and passwords over the internet as text that is Base64 encoded, and the target server is not autheticated
		-This form of auth can expose usernames and passwords. If someone can intercept the tranmisssion, the username and password
		infor can easily be decoded

# 6/JWT Authentication
	6.1/Create AuthorizeController
	6.2/Create UserCredential Class in the Modal then Add this Object into the AuthorizeController
	6.3/Configure JwtSettings in "appseting.json"
	6.4/Create JwtSettings class in Modal folder
	6.5/Register JwtSettings in Program.cs
	6.6/Inject JwtSetting Class into "AuthorizeController"
	6.7/Configure in AuthorizeController class to generate Tokens
	6.8/Configure "tokenDescription"
	6.9/Enable JWT Authentication in Program.cs:
		-Install jwtBearer
		-Add JWT service
# 7/Configure Refesh Token
	7.1/Create TokenResponse.cs class in Modal => then creating IRefreshHandler.cs in Service folder=>Create RefreshHandler.cs in Container folder
	7.2/Inject IRefreshHandler.cs, RefreshHandler.cs  
	7.3/Config RefreshToken into AuthorizeController
# 8/Image Handling
	8.1/Upload
		-upload single img & store in server path
		-upload multiple images & store in server path
		-upload multiple iamges & store in DB
		-Enable middleware to read Images from Browser
		-Download Image File
	8.2/Retrieve(GET Image)
		-Get image from server path & DB
	8.3/Export Excel
		-Create excel using closedXML
		-Save Excel in Local Path
# 9/Minimal API
	-Minimal APIs are architected to create HTTP APIs with minimal dependencies.
	-They are ideals for micro services and apps that want to include only the mininum files, features, 
		and dependencies in Asp.netcore






