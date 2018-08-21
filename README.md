
<img src="http://drive.google.com/uc?export=view&id=1fy3yCMD_z1eJqSI2Ta_Lp-0yQGIecGzf" alt="Documentor">

# Documentor

Documentor is extremly light and easy to use flat file CMS on .NET Core 2.1 for build software documentation. 

Inspiration was found in flat file CMS on PHP - [Grav](https://github.com/getgrav/grav). Of course, Grav has more functions in comparison with Documentor, but at the same time Documentor holds all essential functions to quickly and simply create a platform for your software documentation using **.NET stack**.

It’s used [Markdown](https://www.markdownguide.org) for documentation description.

## References

It’s used [Markdig](https://github.com/lunet-io/markdig) for conversion from Markdown to Html. But if you prefer another converter, you can easily use your own implementation of `IMarkdownConverter`.

As logger it’s used  [NLog](https://github.com/NLog/NLog).

It’s recommended to use [XDocumentor](https://github.com/askalione/xdocumentor) for automatic documents generation in Markdown format from Xml-comments in your  *.dll.

## Features

- Authorization by external resources;
- Documentation pages management, include wysiwyg editor for markdown;
- SEO management;
- Export dump of documentation;

## How to use

Download and open solution. Publish application to target site directory.

### Authorization

Documentor осуществляет авторизацию на основе OAuth 2.0. По умолчанию для подключения доступны следующие ресурсы:
- Google;
- GitHub;
- Facebook;
- Yandex;
- Vkontakte.

В файле `appsettings.json` в разделе `Authorization.Emails` необходимо указать emails, которые могут авторизоваться в системе. Для начала работы нужно добавить хотя бы один email. Открыть доступ другим пользователям далее можно будет через интерфейс системы. Авторизованные пользователи могут использовать все вышеперечисленные features. Если вы создаете структуру и текст документации вручную, то авторизация является optional.

### Pages

In site root directory create a subdirectory `/Pages`, in which one it’s necessary to create a tree of directory with `docs.md`(required) и `metadata.json`(optional) files, which will show hierarchy of pages in your documentation.

Example:

<img src="http://drive.google.com/uc?export=view&id=1YwV2Svd_4NE8isBAT6n_uGcykCOC_NMX" alt="Documentor pages">

Directories must be sequence numbered, as in the example, for correct order display in navigation.

 - `docs.md`(required) - file in Markdown format with documentation text;
 - `metadata.json`(optional) - file in JSON format with metadata for documentation pages.

Example fo `metadata.json`:
```json
{
	"title": "Start page",
	"description": "Let's start this documentation"
}
```
Fields description in `metadata.json` file:

| Name | Description | 
| --- | --- | 
| title | Page title, which displays in nav and in title meta tag | 
| description| Page description, which displays in description meta tag | 

### Configuration

For configuration it’s used `appsettings.json` file. Description of main fields from App section is in `appsettings.json` file:

| Name | Description | 
| --- | --- | 
| DisplayName| Your software name, which will be displayed in site header. (default: 'Documentor') | 
| ShowSequenceNumbers| To display or not to display sequence numbers in navigation. (default: true) |
| Download.Url| Direct reference to download your software last version |
| Download.Version| Your software last version number |
| ExternalLinks| Links to package managers |

Description of main fields from IO section in `appsettings.json` file:

| Name | Description | 
| --- | --- | 
| Pages.Path| Path relative to site root, where documentation structure is placed (default: 'Pages') | 
| Cache.Path| Path relative to site root, where cache for documentation and navigation pages is placed. (default: 'Cache') |
| Cache.Expire| Time in seconds, over which cache for documentation pages and navigation will be updated (default: 604800s = 7 days) |

## License

Documentor is Copyright © 2018 [Alexey Drapash](https://github.com/askalione), [Creacode](http://creacode.ru/) under the [MIT license](https://github.com/askalione/documentor/blob/master/LICENSE).
