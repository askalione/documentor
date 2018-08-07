
## Elibrary.Builder

### References

* .NET Framework 4.6.1

### Version

Текущая версия **v0.0.2**

### Description

.NET Framework библиотека позволяющая импортировать/экспортировать объекты в формат XML для обработки в Articulus, а также в другие форматы.

### Getting started

 * Подключите библиотеку **Elibrary.Builder** через **NuGet**.
 * Поддерживаемые форматы:
	 * [XML](https://ru.wikipedia.org/wiki/XML)
	 * [JATS](https://jats.nlm.nih.gov/)
 * Импортировать и экспортировать в разные форматы можно только только те объекты, которые были унаследованы от `BaseRootNode` (так называемые root-элементы). 
 Список root-элементов:
	 * Journal
	 * Book
	 * ConferenceBook

#### Import

**Elibrary.Builder** поддерживает версионирование, поэтому root-элемент в любом формате (xml, jats) должен содержать информацию о версии **Elibrary.Builder**,  для которой необходимо произвести обработку входного формата.

Для импорта необходимо знать, какой объект требуется получить. Например, для получения объекта журнала из XML необходимо вызвать метод:

~~~
Journal journal = ObjectBuilder.BuildFromXml<Journal>(@"/path/to/xml");
~~~

#### Export

Экспортировать можно только root-элемент. Для экспорта необходимо вначале создать необходимый root-элемент, используя иерархичные вызовы конструкторов, например:

~~~
	// Главы/Разделы
    List<Section> sections = new List<Section> {
        new Section(new List<SectionName>
        {
            new SectionName(ELanguage.Russian, "Локальные переменные")
        })
    };

	// Журнал
    Journal journal = new Journal(titleId: 1,
        issn: "1234-5678",
        eissn: "2144-453X",
        issue: new Issue(volume: "1",
            number: "1",
            altNumber: null,
            part: null,
            pages: null,
            yearPubl: 2010,
            names: new List<IssueName>
            {
                new IssueName(ELanguage.Russian, "Майское программирование")
            }
        ),
        sections: sections,
        names: new List<JournalName>
        {
            new JournalName(ELanguage.Russian, "Вестинк науки по программированию")
        },
        articles: new List<JournalArticle>
        {
                new JournalArticle(type: EJournalArticleType.ABS,
                    section: sections.First(),
                    pages: "1-200",
                    authors: new List<Author>
                    {
                        new Author(role: EAuthorRole.Author,
                            authorInfoList: new List<AuthorInfo>
                            {
                                new AuthorInfo(ELanguage.Russian, "Иванов", "И.И.")
                            },
                            id: 1
                        ),
                        new Author(role: EAuthorRole.Editor,
                            authorInfoList: new List<AuthorInfo>
                            {
                                new AuthorInfo(ELanguage.Russian, "Петвро", "П.П.")
                            },
                            id: 2,
                            spin: "12345XXX"
                        )
                    },
                    names: new List<ArticleName>
                    {
                        new ArticleName(ELanguage.Russian, "Наименование локальный переменных"),
                        new ArticleName(ELanguage.English, "The name of the local variables")
                    },
                    abstracts: new List<Abstract>
                    {
                        new Abstract(ELanguage.Russian, "Краткое описание наименования локальных переменных")
                    },
                    texts: new List<Text>
                    {
                        new Text(ELanguage.Russian, "Полное описание наименования локальных переменных")
                    },
                    codes: new List<Code>
                    {
                        new Code(ECodeType.DOI, "10.1016/j.urology.2017.04.039")
                    },
                    keywords: new List<Keyword>
                    {
                        new Keyword(ELanguage.Russian, "Переменная"),
                        new Keyword(ELanguage.Russian, "Программирование"),
                        new Keyword(ELanguage.English, "Programming")
                    },
                    references: new List<Reference>
                    {
                        new Reference("Jacoby, W. G. (1994). Public attitudes toward government spending. American Journal of Political Science, 38(2), 336-361.")
                    },
                    files: new List<File>
                    {
                        new File(EFileType.Url, "http://www.easybib.com/reference/guide/apa/journal"),
                        new File(EFileType.Contents, "content.pdf")
                    },
                    rubrics: new List<Rubric>
                    {
                        new Rubric("023101", "Общие вопросы"),
                        new Rubric("045155", "Социология языка")
                    },
                    fundings: new List<Funding>
                    {
                        new Funding(ELanguage.Russian, "Министерство Внутренних Дел")
                    }
                )
        }
    );
~~~

После чего для экспорта объекта, например в XML, можно использовать либо метод, который возвращает `XDocument`:

~~~
XDocument xml = ObjectBuilder.BuildToXml(journal);
~~~

Либо метод сразу сохраняющий объект в формат XML в файловой системе:

~~~
ObjectBuilder.SaveToXml(journal, @"/path/to/Journal.xml");
~~~

### Copyright

© Elibrary 2018