# Settings

Settings from `appsettings.json` in root directory look like:

```json
{
  "App": {
    "DisplayName": "",  // Name of your software displayed in header of site. Default if empty: Documentor
    "ShowSequenceNumbers": true, // Show sequence number in navigation. Default: true
    "Download": { // Direct link to download latest version of your software
      "Url": "", // Url to download latest version of your software. Default: empty
      "Version": "" // Version tag of latest version of your software, for example v1.0.0. Default: empty
    },
    "ExternalLinks": { // External urls to download your software from any package managers 
      ...
    }
  },
  "Authorization": { // Authorizations settings
    "Emails": [] // Emails list
  },
  "Authentication": {
    "Google": {
      "ClientId": "",
      "ClientSecret": ""
    },
    "GitHub": {
      "ClientId": "",
      "ClientSecret": ""
    },
    "Facebook": {
      "ClientId": "",
      "ClientSecret": ""
    },
    "Yandex": {
      "ClientId": "",
      "ClientSecret": ""
    },
    "Vkontakte": {
      "ClientId": "",
      "ClientSecret": ""
    }
  },
  "IO": {
    "Pages": {
      "Path": "Pages"
    },
    "Cache": {
      "Path": "Cache",
      "Expire": 604800
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Error",
      "Microsoft": "Error"
    }
  }
}
```