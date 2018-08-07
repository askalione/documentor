<img src="http://drive.google.com/uc?export=view&id=1fy3yCMD_z1eJqSI2Ta_Lp-0yQGIecGzf" alt="Documentor">

# Documentor

Documentor is extremly light and easy to use flat file CMS on .NET Core 2.1 for build software documentation. 

Вдохновением для создания послужил отличный flat file CMS on PHP - [Grav](https://github.com/getgrav/grav). Конечно Grav обладает намного большими функциями по сравнению с Documentor, но при этом Documentor обладает всеми необходимыми функциями позволяющими быстро и просто создать платформу для документации вашего software используя **.NET stack**.

Для описания документов используется облегчённый язык разметки [Markdown](https://www.markdownguide.org).

## References

Для конвертации Markdown в Html используется [Markdig](https://github.com/lunet-io/markdig). Но если вы предпочитаете другой конвертер, то вы легко можете использовать свою реализацию `IMarkdownConverter`.

В качестве логгера используется [NLog](https://github.com/NLog/NLog).

Для автоматической генерации документов в формате Markdown из Xml-комментариев в ваших *.dll сборках рекомендуется использовать [XDocumentor](https://github.com/askalione/xdocumentor).

## How to use

Скачайте и откройте solution. Publish application to target site directory.

### Pages
 В корне сайта создайте директорию `/Pages`, внутри которой необходимо создать дерево директорий с файлами `docs.md`(required) и `metadata.json`(optional), которое будет отражать иерархию страниц вашей документации.

Пример:

<img src="http://drive.google.com/uc?export=view&id=1YwV2Svd_4NE8isBAT6n_uGcykCOC_NMX" alt="Documentor pages">

Директории должны быть обязательно пронумерованы, как в примере, для корректного отображения в навигации в нужном порядке.

 - `docs.md`(required) - файл в формате Markdown, содержащий текст документации;
 - `metadata.json`(optional) - файл в формате JSON, содержащий метаинформацию для 
   страниц документации.

Пример файла `metadata.json`:
```json
{
	"title": "Start page",
	"description": "Let's start this documentation"
}
```
Описание полей в файле`metadata.json`:

| Name | Description | 
| --- | --- | 
| title | Title of page which display in nav and title meta tag | 
| description| Desription of page which display in description meta tag | 

### Configuration

Для конфигурации используется файл `appsettings.json`.
Описание основных полей из секции **App** в файле `appsettings.json`:

| Name | Description | 
| --- | --- | 
| DisplayName| Название вашего software, которое будет отображаться в шапке сайта. (default: 'Documentor') | 
| ShowSequenceNumbers| Отображать или не отображать порядковые номера в навигации. (default: true) |
| Download.Url| Прямая ссылка на скачивание последней версии вашего software |
| Download.Version| Номер последней версии вашего software |
| ExternalLinks| Ссылки на внешние package managers |

Описание основных полей из секции **IO** в файле `appsettings.json`:

| Name | Description | 
| --- | --- | 
| Pages.Path| Путь относительно корня сайта, где расположена структура документации (default: 'Pages') | 
| Cache.Path| Путь относительно корня сайта, где расположен кеш для страниц документации и навигации. (default: 'Cache') |
| Cache.Expire| Время в секундах, через которое будет обновлен кеш для страниц документации (default: 604800s = 7 days) |

## License

Documentor is Copyright © 2018 [Alexey Drapash](https://github.com/askalione), [Creacode](http://creacode.ru/) under the [MIT license](https://github.com/askalione/documentor/blob/master/LICENSE).
