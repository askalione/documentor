
<img src="http://drive.google.com/uc?export=view&id=1fy3yCMD_z1eJqSI2Ta_Lp-0yQGIecGzf" alt="Documentor">

# Documentor

Documentor is extremly light and easy to use flat file CMS on [.NET Core 2.1](https://www.microsoft.com/net/download/dotnet-core/2.1) for build software documentation. 

Inspiration was found in flat file CMS on PHP - [Grav](https://github.com/getgrav/grav). Of course, Grav has more functions in comparison with Documentor, but at the same time Documentor holds all essential functions to quickly and simply create a platform for your software documentation using **.NET stack**.

It’s used [Markdown](https://www.markdownguide.org) for documentation description.

## Demo

[Demo page](http://documentor.creacode.ru).

## References

- It’s used [Markdig](https://github.com/lunet-io/markdig) for conversion from Markdown to Html. But if you prefer another converter, you can easily use your own implementation of `IMarkdownConverter`;
- For logging it’s used  [NLog](https://github.com/NLog/NLog);
- For generating breadcrumbs it's used [SmartBreadcrumbs](https://github.com/zHaytam/SmartBreadcrumbs);
- For building dump it's used [SharpZipLib](https://github.com/icsharpcode/SharpZipLib);
- [Bootstrap 4](https://getbootstrap.com/docs/4.0), [CSS3](https://developer.mozilla.org/en/docs/Web/CSS/CSS3), [HTML5](https://developer.mozilla.org/en-US/docs/Web/Guide/HTML/HTML5), [BEM](http://getbem.com/) used to build frontend.

It’s recommended to use [XDocumentor](https://github.com/askalione/xdocumentor) for automatic documents generation in Markdown format from Xml-comments in your  *.dll.

## Features

- Authorization by external resources;
- Documentation pages management includes wysiwyg editor for markdown;
- SEO management;
- Export dump of documentation;

## How to use

Download last release and open solution. Publish application to target site directory.

### Authorization

Documentor uses authorization on basis of [OAuth 2.0](https://oauth.net/2/) and available emails list. By default next resources are available:
- Google;
- GitHub;
- Facebook;
- Yandex;
- Vkontakte.

In file `appsettings.json` in `Authorization.Emails` section it's necessary to type emails, which can be authorized in application. It's necessary to type at least one email to start work. You can open access for other users via application interface. Authorized users can use all just listed features. If you create documentation structure and text by hand, authorization is optional.

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
	"Title": "Start page",
	"Description": "Let's start this documentation"
}
```
Fields description in `metadata.json` file:

| Name | Description | 
| --- | --- | 
| Title | Page title, which displays in nav and in title meta tag | 
| Description| Page description, which displays in description meta tag | 

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
