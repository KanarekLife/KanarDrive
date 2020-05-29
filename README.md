# KanarDrive
My own custom self-hosted google drive alternative. Still work in progress...
## Try it out yourself!
https://kanar-drive.kanareklife.me/
Account: sample@mail.com - S@mp1eP@ssword 
Database is temporary!
## Functions:
- Managing directories
- Uploading files
- Creating sharable link to file
- Moving files around the directories
## TODO
- [ ] Adding registration
- [ ] Accounts settings
## How to run?
1. Clone repo
2. Create appsettings.json file in KanarDrive.App folder
3. Run `npm install` and `npm run build`
4. Run `dotnet run`!
## Config
Sample config (appsettings.json)
```json
{
  "ConnectionStrings" : {
    "DefaultConnection" : "database.db"
  },
  "DefaultAdmin" : {
    "Username" : "KanarekLife",
    "Email" : "kanareklife@gmail.com",
    "Password" : "S@mp1eP@ssword",
    "Firstname" : "Stanisław",
    "Lastname" : "Nieradko",
    "AvailableCloudSpaceInGbs" : 10
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

Sample docker config (.env)
```dotenv
ConnectionStrings__DefaultConnection=database.db
DefaultAdmin__Username=KanarekLife
DefaultAdmin__Email=kanareklife@gmail.com
DefaultAdmin__Password=S@mp1eP@ssword
DefaultAdmin__Firstname=Stanisław
DefaultAdmin__Lastname=Nieradko
DefaultAdmin__AvailableCloudSpaceInGbs=10
Logging__LogLevel__Default=Information
Logging__LogLevel__Mirosoft=Warning
Logging__LogLevel__Microsoft.Hosting.Lifetime=Information
AllowedHosts=*
ASPNETCORE_URLS=http://localhost:80
```
## Sample Screens
![](docs/screen%20(1).png)
![](docs/screen%20(2).png)
![](docs/screen%20(3).png)
![](docs/screen%20(4).png)
![](docs/screen%20(5).png)
![](docs/screen%20(6).png)
