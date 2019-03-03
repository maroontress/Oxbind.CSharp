# Oxbind

Oxbind is an XML deserializer using APIs of .NET Standard 1.3.

## Example

Deserialize the following XML document:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<movie title="Avatar">
  <director name="James Cameron"/>
  <release year="2009"/>
  <cast>Sam Worthington</cast>
  <cast>Zoe Saldana</cast>
</movie>
```

The `movie` element has the `director`, `release` and `cast` elements.
Here, the `director` element occurs only once,
the `release` element occurs zero or one times,
and the `cast` element occurs zero or more times.
The schema of this XML document can be described with _XML Schema_ as follows:

```xsd
  ...
  <xs:element name="movie">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="director" minOccurs="1" maxOccurs="1"/>
        <xs:element ref="release" minOccurs="0" maxOccurs="1"/>
        <xs:element ref="cast" minOccurs="0"/>
      </xs:sequence>
      <xs:attribute name="title"/>
    </xs:complexType>
  </xs:element>

  <xs:element name="director">
    <xs:complexType>
      <xs:sequence/>
      <xs:attribute name="name"/>
    </xs:complexType>
  </xs:element>
  ...
```

Note that Oxbind does not use XML Schema and its validation, but the example
of the XML Schema is given to show the occurrence order of the elements is
important.

First, creates `Movie` class representing `movie` element as follows:

```csharp
using Maroontress.Oxbind;

[ForElement("movie")]
public sealed class Root
{
    [ElementSchema]
    private static readonly Schema TheSchema = Schema.Of(
        Mandatory.Of<Director>(),
        Optional.Of<Release>(),
        Multiple.Of<Cast>());

    [ForChild]
    public Director director;

    [ForChild]
    public Release release;

    [ForChild]
    public IEnumerable<Cast> casts;
}
```

The `Movie` class has the `ForElement` attribute with the argument `"movie"`,
which means it is associated with the `movie` element.

And there is the `static` and `readonly` field whose type is `Schema`,
with the `ElementSchema` attribute, in the `Movie` class.
The value of this field represents the schema of the `root` element.
The value can be created with the `Schema.Of(params SchemaType[])` method,
and the arguments are as follows:

- `Mandatory.Of<Director>()` represents that the element associated with
  `Director` class occurs once. The `Movie` class must have the instance
  field with the `ForChild` attribute, whose type is `Director`.

- `Optional.Of<Release>()` represents that the element associated with
  `Release` class occurs zero or one times. The `Movie` class must have the
  instance field with the `ForChild` attribute, whose type is `Release`.

- `Multiple.Of<Cast>()` represents that the element associated with `Cast`
  class occurs zero or more times. The `Movie` class must have the instance
  field with the `ForChild` attribute, whose type is `IEnumerable<Cast>`.

Therefore, the `Movie` class has 3 fields of `director`, `release` and
`casts`.
Each field has the `ForChild` attribute, which means it occurs in the
`movie` element.

Second, creates `Director`, `Release` and `Cast` classes
representing `director`, `release` and `cast` elements, respectively,
as follows:

```csharp
[ForElement("director")]
public sealed class Director
{
    [ForAttribute("name")]
    private string name;

    public string Name => name;
}

[ForElement("release")]
public sealed class Release
{
    [field: ForAttribute("year")]
    public string Year { get; }
}

[ForElement("cast")]
public sealed class Cast
{
    [field: ForText]
    public string Name { get; }
}
```

All the classes have the `ForElement` attribute,
which means each class is associated with the element
whose name is the argument of the attribute (for example,
the `Director` class is associated with the `director` element, and so on).

The `Director` class has the instance field `name`, whose type is `string`,
with the `ForAttribute` attribute.
This means that the `name` instance field is
associated with the _XML attribute_
whose name is the argument of the _C# attribute_
(for example, the instance field `name` is associated with the XML attribute
 `"name"`).

The `Release` class is similar to the `Director` class, except that
it has the auto property but its _backing field_ has the `ForAttribute`
attribute.

The `Cast` class has the auto property `Name` representing
the text content of the `Cast` element,
so that its backing field has the `ForText` attribute.

Finally, uses the deserializer with XML document and the associated classes,
to get a `Movie` instance from the XML document as follows:

```csharp
var reader = new StringReader("...");
var factory = new OxbinderFactory();
var binder = factory.Of<Movie>();
var movie = binder.NewInstance(reader);
```

## Requirements for build

- Visual Studio 2017 Version 15.9
  or [.NET Core 2.2 SDK (SDK 2.2.104)][dotnet-core-sdk]

## How to get started

```bash
git clone URL
cd Oxbind.CSharp
dotnet restore
dotnet build
```

## How to get test coverage report with Coverlet

```bash
dotnet test -p:CollectCoverage=true -p:CoverletOutputFormat=opencover --no-build Oxbind.Test
sh coverage-report.sh
```

[dotnet-core-sdk]:
  https://www.microsoft.com/net/download/thank-you/dotnet-sdk-2.2.104-windows-x64-installer
