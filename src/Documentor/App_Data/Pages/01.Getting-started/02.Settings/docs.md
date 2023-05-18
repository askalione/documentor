# Settings

Settings from `appsettings.json` in root directory looks like:

```json
{
  "App": { // Base application settings
    "DisplayName": "",  // Name of your software displayed in header of site. Default if empty: Documentor.
    "ShowSequenceNumbers": true, // Show sequence number in navigation. Default: true.
    "Download": { // Direct link to download latest version of your software.
      "Url": "", // Url to download latest version of your software. Default: empty.
      "Version": "" // Version tag of latest version of your software, for example v1.0.0. Default: empty.
    },
    "ExternalLinks": { // External urls to download your software from any package managers .
      ...
    }
  },
  "Authorization": { // Authorizations settings.
    "Emails": [] // List containing email addresses available to authorize. Required to configure.
  },
  "Authentication": { // List of OAuth 2.0 providers. Required to configure at least one provider.
    ...
  },
  "IO": { // Application directories settings.
    "Pages": { // Pages content directory settings.
      "Path": "App_Data/Pages" // Path relative to the root directory. Will be created if it does not exist. Default: Pages.
    },
    "Cache": { // Cache directory settings.
      "Path": "App_Data/Cache", // Path relative to the root directory. Will be created if it does not exist. Default: Cache.
      "Expire": 604800 // Cache expire time in seconds. Default: 604800 (7 days).
    }
  },
  "Logging": { // Logging settings
    "LogLevel": {
      "Default": "Error", // Min log level for exception by Application. Default: Error
      "Microsoft": "Error" // Min log level for exception by Microsoft.*. Default: Error
    }
  }
}
```

## Required
1. Configure at least one [OAuth 2.0](https://oauth.net/2/) provider (Supported: [Google](https://developers.google.com/identity/protocols/OAuth2), [GitHub](https://developer.github.com/apps/building-oauth-apps/authorizing-oauth-apps/), [Yandex](https://tech.yandex.ru/oauth/), [Vkontakte](https://vk.com/dev/authentication)) in section `Authentication`.

	For example:
	```json
	"GitHub": {
		"ClientId": "ac09asd850f98e5fc24",
		"ClientSecret": "a2d06f2b09keaf79f4f32cec1dek574e000l41e"
	}
	```

2. Add at least one email address to configure `Authorization` section. With that email address you should authorize by configured OAuth provider.

	For example:
	```json
	Authorization": {
		"Emails": ["email@example.com"] 
	}
	```

Without configured `Authentication` and `Authorization` sections you can't manage content by web interface, but you can still manage them by file system. See more details in [Content management](/Content-management) chapter.