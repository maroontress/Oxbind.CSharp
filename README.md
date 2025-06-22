# Oxbind

Oxbind is a .NET library for deserializing XML documents to C# objects using
constructor injection and a declarative attribute-based mapping. It targets
.NET Standard 2.0.

## Why Oxbind?

- **Type-Safe Mapping**: Clear correspondence between XML schema and C# classes
- **Constructor-Driven**: Promotes immutable object design
- **Declarative Mapping**: Simple configuration through C# attributes
- **Detailed Error Reporting**: Error messages with XML line and column
  information

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

The `movie` element has the `director`, `release`, and `cast` elements. Here,
the `director` element occurs only once, the `release` element occurs zero or
one times, and the `cast` element occurs zero or more times. The schema of this
XML document can be described with _XML Schema_ as follows:

```xsd
  ...
  <xs:element name="movie">
    <xs:complexType>
      <xs:sequence>
        <xs:element ref="director" minOccurs="1" maxOccurs="1"/>
        <xs:element ref="release" minOccurs="0" maxOccurs="1"/>
        <xs:element ref="cast" minOccurs="0" maxOccurs="unbounded"/>
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

> [!NOTE]
> Oxbind does not use XML Schema and its validation, but the example of the XML
> Schema is given to show the occurrence order of the elements is important.

First, creates a `Movie` class representing the `movie` element as follows:

```csharp
using Maroontress.Oxbind;

[ForElement("movie")]
public record class Movie(
    [ForAttribute("title")] string? Title,
    [Required] Director TheDirector,
    [Optional] Release? MaybeRelease,
    [Multiple] IEnumerable<Cast> Casts);
```

The `Movie` class has the `ForElement` attribute with the argument `"movie"`,
which means it is associated with the `movie` element.

And the constructor has parameters with some attributes, which are
corresponding to the schema of the root element. In this example, since a
`record class` is used, the constructor parameters implicitly generate
instance properties. Each parameter is as follows:

- `[ForAttribute("title")] string? Title` represents the instance property
  `Title`, which is associated with the XML attribute `title` of the `movie`
  element. This means that the constructor's parameter with `[ForAttribute(…)]`
  is associated with the _XML attribute_ whose name is the argument of the _C#
  attribute_.

- `[Required] Director TheDirector` represents the instance property
  `TheDirector`, which is associated with the XML element `director` that occurs
  once. The type of `Director` is the class with the `ForElement` attribute with
  the argument `"director"`.

- `[Optional] Release? MaybeRelease` represents that the instance property
  `MaybeRelease`, which is associated with the XML element `release` that occurs
  zero or one times. The type of `Release` is the class with the `ForElement`
  attribute with the argument `"release"`.

- `[Multiple] IEnumerable<Cast> Casts` represents that the instance property
  `Casts`, which is associated with the XML element `cast` that occurs zero or
  more times. The type of `Cast` is the class with the `ForElement` attribute
  with the argument `"cast"`.

Therefore, the `Movie` class has four properties: `Title`, `TheDirector`,
`MaybeRelease`, and `Casts`.

Second, creates `Director`, `Release` and `Cast` classes representing
`director`, `release` and `cast` elements, respectively, as follows:

```csharp
[ForElement("director")]
public record class Director([ForAttribute("name")] string? Name);

[ForElement("release")]
public record class Release([ForAttribute("year")] string? Year);

[ForElement("cast")]
public record class Cast([ForText] string Name);
```

All the classes have the `ForElement` attribute, which means each class is
associated with the element whose name is the argument of the attribute. For
example, the `Director` class is associated with the `director` element, and so
on.

The `Director` class has the constructor. The parameters of the constructor with
some attributes is associated with the schema. `[ForAttribute("name")] string?
Name` represents the instance property `Name`, which is associated with the XML
attribute `name` of the `director` element.

The `Release` class is similar to the `Director` class, so a detailed
explanation is omitted here.

The `Cast` class is also similar to the `Director` class, but its constructor
has the parameter with the `ForText` attribute, which means the instance
property `Name` is associated with the inner text of the `cast` element.

Finally, to obtain a `Movie` instance from the XML document, use the
deserializer with the XML document and the associated classes as follows:

```csharp
var reader = new StringReader(…);
var factory = new OxbinderFactory();
var binder = factory.Of<Movie>();
var movie = binder.NewInstance(reader);
```

> [See the result in .NET Fiddle](https://dotnetfiddle.net/24Smmg)

The examples above use `record class` for simplicity, but you can also use
regular classes or primary constructors with Oxbind. Choose the style that best
fits your coding preferences or project requirements:

```csharp
[ForElement("movie")]
public sealed class Movie
{
    public Movie(
        [ForAttribute("id")] string? id,
        [ForAttribute("title")] string? title,
        [Required] Director theDirector,
        [Optional] Release? maybeRelease,
        [Multiple] IEnumerable<Cast> casts)
    {
        this.Id = id;
        this.Title = title;
        this.TheDirector = theDirector;
        this.MaybeRelease = maybeRelease;
        this.Casts = casts;
    }

    public string? Id { get; }
    public string? Title { get; }
    public Director TheDirector { get; }
    public Release? MaybeRelease { get; }
    public IEnumerable<Cast> Casts { get; }
}
```

```csharp
[ForElement("movie")]
public sealed class Movie(
    [ForAttribute("id")] string? id,
    [ForAttribute("title")] string? title,
    [Required] Director theDirector,
    [Optional] Release? maybeRelease,
    [Multiple] IEnumerable<Cast> casts)
{
    public string? Id { get; } = id;
    public string? Title { get; } = title;
    public Director TheDirector { get; } = theDirector;
    public Release? MaybeRelease { get; } = maybeRelease;
    public IEnumerable<Cast> Casts { get; } = casts;
}
```

> [!NOTE]
> The examples above use record class (available in C# 9.0 and later) and
> primary constructors (C# 12 and later) for simplicity, but you can also use
> regular classes with Oxbind. The library itself targets .NET Standard 2.0 and
> does not require these newer language features.

## Limitations

Oxbind is designed to map XML structures to C# constructor parameters
declaratively. This design principle leads to certain limitations on the XML
structures it can handle.

Specifically, Oxbind requires child elements to appear in a **fixed, sequential
order**, corresponding to the order of parameters in the C# constructor. This
is analogous to the `<xs:sequence>` compositor in an XML Schema.

Consequently, structures that require choice or non-sequential ordering are
**not supported**. This includes:

- **Choice of elements** (like `<xs:choice>`): Where only one element from a
  group of different elements can appear.
- **Interleaved repeating elements** (like `<xs:choice
  maxOccurs="unbounded">`): Where different types of child elements are mixed
  together, rather than being grouped by type.
- **Any order of elements** (like `<xs:all>`): Where elements can appear in any
  order.

For example, Oxbind cannot deserialize a document where different types of
elements are interleaved, as shown in the `<library-contents>` element below:

```xml
<!-- This structure is NOT supported -->
<library-contents>
  <book title="The Hobbit"/>
  <movie title="Avatar"/>
  <book title="Dune"/>
  <music-album artist="Queen" title="Greatest Hits"/>
  <movie title="The Lord of the Rings"/>
</library-contents>
```

This limitation is a direct consequence of mapping to a constructor's parameter
list, which has a single, defined signature.

## Getting started

Oxbind is available as [the ![NuGet-logo][nuget-logo] NuGet
package][nuget-oxbind].

### Install

```plaintext
dotnet add package Maroontress.Oxbind
```

### How to create a class representing an XML element

See [Attribute Specifications](GET_STARTED.md).

## How to build

### Requirements for build

- Visual Studio 2022 (Version 17.14) or [.NET 9.0 SDK (SDK 9.0.300)][dotnet-sdk]

### Build instructions

```plaintext
git clone <URL>
cd Oxbind.CSharp
dotnet build --configuration Release
```

### Get test coverage report with Coverlet

If the `dotnet-reportgenerator-globaltool` tool is not already installed:

```plaintext
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Run the following command to generate a test coverage report with Coverlet:

```plaintext
dotnet test --configuration Release --no-build \
  --logger "console;verbosity=detailed" \
  --collect:"XPlat Code Coverage" \
  --results-directory MsTestResults
reportgenerator -reports:MsTestResults/*/coverage.cobertura.xml \
  -targetdir:Coverlet-html
```

[dotnet-sdk]: https://dotnet.microsoft.com/en-us/download
[nuget-logo]: https://maroontress.github.io/images/NuGet-logo.png
[nuget-oxbind]: https://www.nuget.org/packages/Maroontress.Oxbind/
