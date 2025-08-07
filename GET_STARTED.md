# Attribute Specifications

Oxbind maps XML elements and attributes to C# class constructor parameters.
This document provides a clear, step-by-step explanation of the attribute
specification flow and rules for first-time users.

### Attribute Reference Table

| Attribute         | Target                | Purpose                                                           |
| :---              | :---                  | :---                                                              |
| `ForElement`      | Class                 | Associates a class with an XML element name                       |
| `ForAttribute`    | Constructor parameter | Binds a constructor parameter to an XML attribute                 |
| `Required`        | Constructor parameter | Binds a required child element (must appear once)                 |
| `Optional`        | Constructor parameter | Binds an optional child element (0 or 1 occurrence)               |
| `Multiple`        | Constructor parameter | Binds a repeatedly occurring child element (0 or more times)      |
| `ForChildElement` | Constructor parameter | Binds a child element that only contains text content             |
| `ForText`         | Constructor parameter | Binds the element’s text content to a parameter                   |
| `Ignored`         | Constructor           | Marks a constructor that Oxbind should ignore for deserialization |

## 1. Basic Steps

1. Create a C# class (or record class) for each XML element.
2. Annotate the class with the `[ForElement("XML_ELEMENT_NAME")]` attribute.
3. If the class has multiple constructors, mark each constructor that Oxbind
   should ignore with `[Ignored]`.
4. Annotate each constructor parameter with the appropriate attribute to bind
   to an XML attribute or child element.

## 2. Classes Corresponding to XML Elements

Specify `[ForElement]` on the class and pass the XML element name as the first
argument:

```csharp
[ForElement("movie")]
public record class Movie;
```

If the element belongs to a namespace, supply its URI as the second argument:

```xml
<movie xmlns="http://example.com/Oxbind/movie"/>
```

```csharp
[ForElement("movie", "http://example.com/Oxbind/movie")]
public record class Movie;
```

> [!TIP]
> For elements without a namespace (so-called “no namespace” or “null
> namespace”), either pass an empty string `""` as the second argument of
> `[ForElement]` or omit the second argument entirely. According to the XML
> specification, elements in no namespace are considered part of the null
> namespace, which Oxbind explicitly represents with `""`.

## 3. Handling Constructors

If a class defines multiple constructors, you must mark each constructor that
Oxbind should not inject with `[Ignored]`:

```csharp
[ForElement("movie")]
public record class Movie(
    [ForAttribute("id")] string? Id,
    [ForAttribute("title")] string? Title)
{
    [Ignored]
    public Movie() : this("default", "")
    {
    }
}
```

> [!NOTE]
> The phrase “Oxbind injects values into the constructor” means that Oxbind
> reads values from the XML and automatically passes them to the corresponding
> constructor parameters.

If you omit the `[Ignored]` attribute, a runtime error will occur.

## 4. Attributes on Constructor Parameters

Annotate each constructor parameter with attributes to define its binding. The
core binding attributes are mutually exclusive.

### Core Binding Attributes

Choose **exactly one** of the following for each parameter:

- `ForAttribute`: Binds to an XML attribute
- `Required`: Binds to a required child element (must appear exactly once)
- `Optional`: Binds to an optional child element (0 or 1 occurrence)
- `Multiple`: Binds to a child element that may repeat (0 or more times)
- `ForText`: Binds to the text content of the element

> [!IMPORTANT]
> You may specify only one of the core binding attributes listed above per
> parameter.

> [!NOTE]
> If no matching XML attribute exists for a `ForAttribute` binding, the
> parameter receives `null`. Therefore, in a nullable-aware context, the
> parameter type must be a nullable reference type (e.g. `string?` or
> `BindResult<string>?`).

The `ForChildElement` attribute can be used in combination with `Required`,
`Optional`, or `Multiple` to bind simple text-only child elements, which is
explained in a later section.

### Ordering Rules

1. List all `ForAttribute` parameters first (order among them does not matter).
2. After the `ForAttribute` parameters, you can define parameters for _either_
   text content _or_ child elements, but not both in the same constructor:
   - For text content: Use a single `ForText` parameter. This parameter must be
     the last one in the constructor parameters.
   - For child elements: Use any combination of `Required`, `Optional`, and/or
     `Multiple` parameters<sup>&dagger;</sup>. These can be combined with
     `ForChildElement` for simple text-only elements.

> &dagger; However, there are additional restrictions on the order for
> parameters with the same element name. See Section 6.

### Example: Movie XML

```xml
<movie id="1" title="Avatar">
  <director name="James Cameron"/>
  <release year="2009"/>
  <cast>Sam Worthington</cast>
  <cast>Zoe Saldana</cast>
</movie>
```

Represented as C# record classes:

```csharp
[ForElement("movie")]
public record class Movie(
    [ForAttribute("id")] string? Id,
    [ForAttribute("title")] string? Title,
    [Required] Director TheDirector,
    [Optional] Release? MaybeRelease,
    [Multiple] IEnumerable<Cast> Casts);

[ForElement("director")]
public record class Director([ForAttribute("name")] string? Name);

[ForElement("release")]
public record class Release([ForAttribute("year")] string? Year);

[ForElement("cast")]
public record class Cast([ForText] string Name);
```

To bind attributes in a namespace, supply the namespace URI as the second
argument of `ForAttribute`:

```xml
<actor xmlns:m="http://example.com/Oxbind/movie" m:id="1" m:name="Sam Worthington"/>
```

```csharp
[ForElement("actor", "http://example.com/Oxbind/movie")]
public record class Actor(
    [ForAttribute("id", "http://example.com/Oxbind/movie")] string? Id,
    [ForAttribute("name", "http://example.com/Oxbind/movie")] string? Name);
```

> [!NOTE]
> The second argument of `ForAttribute` is optional. If omitted, the attribute
> is assumed to be in the null namespace.

### Optimizing Simple Child Elements with `[ForChildElement]`

For child elements that have no attributes and only contain text (like the
`<cast>` element in our example), you can bind them directly to a `string`
parameter by using the `[ForChildElement]` attribute. This avoids the need to
create a separate class for a simple element.

`[ForChildElement]` must be used in combination with one of the quantity
attributes: `[Required]`, `[Optional]`, or `[Multiple]`. It takes the name of
the child element as an argument.

Let's simplify the `Movie` class by using `[ForChildElement]` for the `cast`
elements:

```csharp
[ForElement("movie")]
public record class Movie(
    [ForAttribute("id")] string? Id,
    [ForAttribute("title")] string? Title,
    [Required] Director TheDirector,
    [Optional] Release? MaybeRelease,
    // Bind the text of <cast> elements directly to a string collection
    [Multiple][ForChildElement("cast")] IEnumerable<string> Casts);

[ForElement("director")]
public record class Director([ForAttribute("name")] string? Name);

[ForElement("release")]
public record class Release([ForAttribute("year")] string? Year);

// The Cast class is no longer needed.
// [ForElement("cast")]
// public record class Cast([ForText] string Name);
```

As you can see, the `Casts` parameter is now of type `IEnumerable<string>`, and
the `[Multiple][ForChildElement("cast")]` attributes instruct Oxbind to map the
text content of each `<cast>` element to a string in the collection. This makes
the separate `Cast` class unnecessary, simplifying your code.

> [!TIP]
> This approach is ideal for simple data elements. If a child element has its
> own attributes or a more complex structure (i.e., nested child elements), you
> should continue to use a dedicated class annotated with `[ForElement]`.

## 5. BNF Grammar for Parameters

The order and combination of parameters follow this BNF:

```plaintext
<parameters>           ::= <for-attribute> <content>?

<for-attribute>        ::= { [ForAttribute(<attribute-name>)] <param> }*
<param>                ::= (string? | BindResult<string>?) <parameter-name>

<content>              ::= <for-text> | <child-elements>
<for-text>             ::= [ForText] (string | BindResult<string>) <parameter-name>
<child-elements>       ::= { (<required-element>
                            | <optional-element>
                            | <multiple-elements>) <parameter-name> }*

<required-element>     ::= [Required]
                           (<element-type>
                          | BindResult<<element-type>>
                          | [ForChildElement(<child-element-name>)] string
                          | [ForChildElement(<child-element-name>)] BindResult<string>)
<optional-element>     ::= [Optional]
                           (<element-type>?
                          | BindResult<<element-type>>?
                          | [ForChildElement(<child-element-name>)] string?
                          | [ForChildElement(<child-element-name>)] BindResult<string>?)
<multiple-elements>    ::= [Multiple]
                           (IEnumerable<<element-type>>
                          | IEnumerable<BindResult<<element-type>>>
                          | [ForChildElement(<child-element-name>)] IEnumerable<string>
                          | [ForChildElement(<child-element-name>)] IEnumerable<BindResult<string>>)

; <parameter-name> is a valid C# parameter name.
; <element-type> refers to a class marked with [ForElement].
; <attribute-name> is a string literal specifying the name of the XML attribute.
; <child-element-name> is a string literal specifying the name of the child XML element.
```

- `[ForAttribute]` parameters appear first.
- `[ForText]` appears last and only once.
- `Required`/`Optional`/`Multiple` cannot be combined with `ForText` in the
  same constructor.

## 6. Practical Considerations

### Order of Parameters for the Same XML Element Name

Pay attention to the order of parameters for the same XML element name. For
example:

- An `Optional` parameter followed by a `Required` or `Multiple` parameter with
  the same element name causes an error.
- A `Multiple` parameter followed by any parameter with the same element name
  causes an error.

```csharp
// Invalid: Optional followed by Required with the same element name
[ForElement("book")]
public record class Book(
    [Optional][ForChildElement("author")] Author? MaybeExtraAuthor,
    [Required][ForChildElement("author")] Author MainAuthor);

// Invalid: Multiple followed by Optional with the same element name
[ForElement("book")]
public record class Book(
    [Multiple][ForChildElement("author")] IEnumerable<Author> Authors,
    [Optional][ForChildElement("author")] Author? ExtraAuthor);
```

### Handling Multiple Classes with the Same XML Element Name

Oxbind allows you to define multiple classes with the same XML element name.
For example, you can define different classes for the `<item>` element as shown
below:

```csharp
[ForElement("item")]
public record class ItemA([ForText] string Name);

[ForElement("item")]
public record class ItemB([ForText] string Description);
```

If multiple classes are annotated with the same XML element name (e.g.
`<item>`), Oxbind determines which class to use unambiguously by evaluating the
XML according to the order of constructor parameters. Therefore, duplicate
element names do not cause parsing conflicts.

## 7. Using `BindResult<T>`

### Purpose and Overview

`BindResult<T>` is an interface that, during XML deserialization, captures not
only the value but also its location (line and column) in the original XML
document. This information aids in detailed error reporting, debugging, and
providing user feedback.

### Example Usage

```csharp
[ForElement("book")]
public record class Book(
    [ForAttribute("id")] BindResult<string>? Id,
    [Required] BindResult<Author> MainAuthor,
    [Multiple][ForChildElement("genre")] IEnumerable<BindResult<string>> Genres);

[ForElement("author")]
public record class Author([ForText] string Name);
```

In this example, the `id` attribute, the `author` element, and the `genre`
child elements will all include their original XML document locations.

### Benefits

- **Improved Error Reporting**: Generate error messages with precise location
  data if values are invalid.
- **Easier Debugging**: Quickly identify where deserialization issues
  originate.
- **Enhanced User Feedback**: Show users exactly where in their XML input a
  problem occurred.

### Notes

- `T` in `BindResult<T>` must be a reference type (class).
- You must explicitly declare constructor parameters as `BindResult<T>` to use
  this feature.

## 8. Frequently Asked Questions

> **Q1:** Can I apply both `ForAttribute` and `Required` to the same parameter?
>
> **A1:** No. The main binding attributes (`ForAttribute`, `Required`,
> `Optional`, `Multiple`, and `ForText`) are mutually exclusive. You can only
> apply one of these to each parameter.

> **Q2:** Where should `ForText` be placed?
>
> **A2:** It must be the last constructor parameter and only one `ForText`
> parameter is allowed.

> **Q3:** Can I combine `Required` (or `Optional`/`Multiple`) with `ForText` in
> the same constructor?
>
> **A3:** No. A constructor can either have parameters for child elements
> (using `Required`, `Optional`, `Multiple`) or a parameter for its own text
> content (using `ForText`), but not both.

> **Q4:** What's the difference between `[ForChildElement]` and `[ForText]`?
>
> **A4:**
> - `[ForText]` binds the text content of the **current element** (the one
>   whose class is annotated with `[ForElement]`).
> - `[ForChildElement]` binds the text content of a **child element**. It must
>   be combined with `[Required]`, `[Optional]`, or `[Multiple]` and it
>   specifies the name of that child element.
