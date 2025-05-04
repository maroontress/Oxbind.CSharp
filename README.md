# Oxbind

Oxbind is a .NET library that deserializes an XML document. It depends on .NET
Standard 1.3.

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

First, creates a `Movie` class representing the `movie` element as follows:

```csharp
using Maroontress.Oxbind;

[ForElement("movie")]
public record class Movie(
    [ForAttribute("title")] string Title,
    [Mandatory] Director TheDirector,
    [Optional] Release? MaybeRelease,
    [Multiple] IEnumerable<Cast> Casts)
{
}
```

The `Movie` class has the `ForElement` attribute with the argument `"movie"`,
which means it is associated with the `movie` element.

And the constructor has parameters with some attributes, which are corresponding
to the schema of the root element.

- `[ForAttribute("title")] string Title` represents the instance property
  `Title`, which is associated with the XML attribute `title` of the `movie`
  elemtent. This means that the constructor's parameter with `[ForAttribute(…)]`
  is associated with the _XML attribute_ whose name is the argument of the _C#
  attribute_.

- `[Mandatory] Director TheDirector` represents the instance property
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
public record class Director([ForAttribute("name")] string Name)
{
}

[ForElement("release")]
public sealed class Release([ForAttribute("year")] string year)
{
    public string Year { get; } = year;
}

[ForElement("cast")]
public record class Cast([ForText] string Name)
{
}
```

All the classes have the `ForElement` attribute, which means each class is
associated with the element whose name is the argument of the attribute. For
example, the `Director` class is associated with the `director` element, and so
on.

The `Director` class has the constructor. The parameters of the constructor with
some attributes is associated with the schema. `[ForAttribute("name")] string
Name` represents the instance property `Name`, which is associated with the XML
attribute `name` of the `director` elemtent.


The `Release` class is similar to the `Director` class, except that it is not a
`record class`.

The `Cast` class is also similar to the `Director` class, but its constructor
has the parameter with the `ForText` attribute, which means the instance
property `Name` is associated with the inner text of the `cast` element.

Finally, uses the deserializer with an XML document and the associated classes,
to get a `Movie` instance from the XML document as follows:

```csharp
var reader = new StringReader(…);
var factory = new OxbinderFactory();
var binder = factory.Of<Movie>();
var movie = binder.NewInstance(reader);
```

> [See the result in .NET Fiddle](https://dotnetfiddle.net/Mu2FL2)

## How to build

### Requirements for build

- Visual Studio 2022 (Version 17.13) or [.NET 9.0 SDK (SDK 9.0.203)][dotnet-sdk]

### Get started

```plaintext
git clone URL
cd Oxbind.CSharp
dotnet build --configuration Release
```

### Get test coverage report with Coverlet

```plaintext
dotnet tool install -g dotnet-reportgenerator-globaltool
dotnet test --configuration Release --no-build \
  --logger "console;verbosity=detailed" \
  --collect:"XPlat Code Coverage" \
  --results-directory MsTestResults
reportgenerator -reports:MsTestResults/*/coverage.cobertura.xml \
  -targetdir:Coverlet-html
```

[dotnet-sdk]: https://dotnet.microsoft.com/en-us/download
