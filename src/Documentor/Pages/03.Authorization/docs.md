# Authorization

To authenticate user, Documentor uses any OAuth 2.0 providers. There are no register page and store user data.

It's easy, no matter which provider user choose to authenticate. Configure OAuth 2.0 providers (Supported: [Google](https://developers.google.com/identity/protocols/OAuth2), [GitHub](https://developer.github.com/apps/building-oauth-apps/authorizing-oauth-apps/), [Facebook](https://developers.facebook.com/docs/facebook-login/), [Yandex](https://tech.yandex.ru/oauth/), [Vkontakte](https://vk.com/dev/authentication)). See more details in [Settings](/Getting-started/Settings) chapter.

But how do user authorize? All users can modify pages?! No. Documentor authorizes user by email address.

You can manage email address list by web interface.

![Documentor menu](/demo/pages_8.png)

Also you can configure list of email address by `appsettings.json`. See more details in [Settings](/Getting-started/Settings) chapter.