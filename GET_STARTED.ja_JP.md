# 属性の仕様

Oxbindでは、XMLの要素や属性をC#のクラスのコンストラクタパラメータに関連付けます。本ドキュメントでは、はじめて使う方向けに属性指定の流れやルールをわかりやすく解説します。

### 属性リスト表

| 属性 | 属性の対象 | 用途 |
|:--- |:--- | :--- |
| `ForElement`   | クラス                | クラスにXML要素名を紐付ける |
| `ForAttribute` | コンストラクタパラメータ | パラメータにXML属性を紐付ける |
| `Required`     | コンストラクタパラメータ | パラメータに必須出現の子要素を紐付ける |
| `Optional`     | コンストラクタパラメータ | パラメータに任意出現の子要素を紐付ける |
| `Multiple`     | コンストラクタパラメータ | パラメータに0回以上繰り返す子要素を紐付ける |
| `ForText`      | コンストラクタパラメータ | パラメータに要素テキスト内容を紐付ける |
| `Ignored`      | コンストラクタ         | Oxbindが値を注入しないコンストラクタ用 |

## 1. 基本的な手順

1. XML要素ごとにC#のクラス（またはレコードクラス）を作成する
2. クラスに`[ForElement("XML要素名")]`属性を付与する
3. クラスに複数のコンストラクタがある場合、Oxbindが値を注入しないコンストラクタに`[Ignored]`属性を付与する
4. 各コンストラクタパラメータに、XMLの属性や子要素を対応づける属性を付与する

## 2. XML要素に対応するクラス

XML要素に対応するクラスには`[ForElement]`属性を指定し、引数にXML要素名を記述します。

```csharp
[ForElement("movie")]
public record class Movie;
```

名前空間がある場合は、第2引数にURIを指定できます。

```xml
<movie xmlns="http://example.com/Oxbind/movie"/>
```

```csharp
[ForElement("movie", "http://example.com/Oxbind/movie")]
public record class Movie;
```

> [!TIP]
> 名前空間の指定がない要素（いわゆる「名前空間なし」または「無名名前空間」に属する要素）については、`[ForElement]`属性の第2引数に`""`（空文字列）を指定するか、省略してください。なお、名前空間が指定されていない要素はXML仕様上「無名名前空間（null namespace）」と見なされ、Oxbindでは `""`によってこれを明示的に表現できます。

## 3. コンストラクタの扱い

同一クラス内に複数のコンストラクタがある場合、Oxbindが値を注入しないコンストラクタそれぞれに`[Ignored]`属性を付与する必要があります。

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
> 「コンストラクタにOxbindが値を注入する」という表現は「XML要素から値を読み取り、対応するコンストラクタのパラメータに自動的に渡す」ことを意味しています。

`[Ignored]`属性が無い場合は実行時エラーになります。

## 4. コンストラクタパラメータの属性

コンストラクタパラメータには以下いずれかの属性を付与します:

- `ForAttribute`: XML属性に対応
- `Required`: 必須の子要素（1回だけ出現）
- `Optional`: 省略可能な子要素（0回または1回）
- `Multiple`: 複数回出現する子要素（0回以上）
- `ForText`: 要素のテキストに対応

> [!IMPORTANT]
> 1つのパラメータに指定できる属性は1種類のみです。

> [!NOTE]
> `[ForAttribute]`属性で対応するXML属性が存在しない場合、そのパラメータには`null`が渡されます。そのため、
ヌル許容参照型コンテキスト（nullable aware context）では、型はヌル許容参照型（`string?`または`BindResult<string>?`）である必要があります。

### 指定順序のルール

1. すべての`ForAttribute`属性のパラメータを先頭から連続して記述します（順序は任意）。
2. `ForAttribute`属性のパラメータの後には、テキスト内容か子要素の**いずれか一方のみ**をパラメータとして定義できます（同じコンストラクタ内で両方を指定することはできません）:
   - テキスト内容の場合: `ForText`パラメータを1つだけ使用します。このパラメータはコンストラクタパラメータの最後に配置する必要があります。
   - 子要素の場合: `Required`、`Optional`、`Multiple` パラメータを任意に組み合わせて使用できます<sup>&dagger;</sup>。

> &dagger; ただし、同じ要素名を持つパラメータの順序には追加の制約があります。詳細はセクション6を参照してください。

### 例: 映画データのXML

```xml
<movie id="1" title="Avatar">
  <director name="James Cameron"/>
  <release year="2009"/>
  <cast>Sam Worthington</cast>
  <cast>Zoe Saldana</cast>
</movie>
```

上記を表現するクラス例:

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

属性に名前空間を指定する場合は、`ForAttribute`属性の第2引数にURIを指定します。

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
> `ForAttribute`属性の第2引数は省略可能です。省略した場合、無名名前空間（null namespace）を指定したものとみなされます。

## 5. パラメータ指定のBNF

パラメータの順序や組み合わせは次のBNFで定義されます。

```plaintext
<parameters>           ::= <for-attribute> <content>?

<for-attribute>        ::= { [ForAttribute] <param> }*
<param>                ::= (string? | BindResult<string>?) <parameter-name>

<content>              ::= <for-text> | <child-elements>
<for-text>             ::= [ForText] (string | BindResult<string>) <parameter-name>
<child-elements>       ::= { (<required-element>
                            | <optional-element>
                            | <multiple-elements>) <parameter-name> }*

<required-element>     ::= [Required] (<element-type> | BindResult<<element-type>>)
<optional-element>     ::= [Optional] (<element-type>? | BindResult<<element-type>>?)
<multiple-elements>    ::= [Multiple] (IEnumerable<<element-type>>
                                     | IEnumerable<BindResult<<element-type>>>)

; <parameter-name> はC#の有効なパラメータ名です。
; <element-type> は[ForElement]属性が付与されたクラスです。
```

- `[ForAttribute]` は先頭から連続
- `[ForText]` は最後に1つだけ
- `[Required]` / `[Optional]` / `[Multiple]` は `ForText` とは併用不可
- 1パラメータに複数属性は不可

## 6. 実践的な注意点

### 同じXML要素名に対応するパラメータの並び順

同じXML要素名に対応するパラメータの並び順に注意してください。例:

```csharp

- `[Optional]`の後に同要素名の`[Required]`や`[Multiple]`はエラー
- `[Multiple]`の後に（属性に依らず）同じ要素名をもつパラメータはエラー

```csharp
// NG: Optional の後に同要素名の Required
[ForElement("book")]
public record class Book(
    [Optional] Author? MaybeExtraAuthor,
    [Required] Author MainAuthor);
```

```csharp
// NG: Multiple の後に同要素名の Optional
[ForElement("book")]
public record class Book(
    [Multiple] IEnumerable<Author> Authors,
    [Optional] Author? ExtraAuthor);
```

### 同一のXML要素名をもつ複数のクラス

Oxbindでは、同じXML要素名を持つ複数のクラスを定義することができます。たとえば、以下のように`<item>`要素に対して異なるクラスを定義できます。

```csharp
[ForElement("item")]
public record class ItemA([ForText] string Name);

[ForElement("item")]
public record class ItemB([ForText] string Description);
```

`[ForElement]`属性を持つ複数のクラスで同一のXML要素名（例: `<item>`）を指定している場合でも、Oxbindはコンストラクタのパラメータの順序に従ってXMLを評価するため、どのクラスが使われるかは文脈から決定的に判定されます。したがって、要素名が重複していても、パースに支障はありません。

## 7. `BindResult<T>`の活用

### 目的と概要

`BindResult<T>`は、XML のデシリアライズ時に、値そのものだけでなく、その値が元のXML文書のどの位置（行番号と列番号）に存在していたかの情報を保持するためのインタフェースです。これにより、エラー報告やデバッグ、ユーザーへのフィードバックにおいて、より具体的な情報を提供することが可能になります。

### 使用例

```csharp
[ForElement("book")]
public record class Book(
    [ForAttribute("id")] BindResult<string>? Id,
    [Required] BindResult<Author> MainAuthor);

[ForElement("author")]
public record class Author([ForText] string Name);
```

上記の例では、`id`属性と`author`要素の値だけでなく、それらがXML文書内のどの位置（行と桁）に記述されていたかの情報も取得できます。

### 利点

- **エラー報告の精度向上**: 不正な値や期待される形式と異なる場合に、具体的な位置情報を含めたエラーメッセージを生成できます。
- **デバッグの効率化**: デシリアライズ時の問題を迅速に特定し、修正する際の手がかりとなります。
- **ユーザーへのフィードバック**: 入力されたXMLに問題がある場合、具体的な位置を示すことで、ユーザーが修正しやすくなります。

### 注意点

- `BindResult<T>`は、`T`が参照型（クラス）である必要があります。
- `BindResult<T>`を使用する場合、対応するパラメータの型は`BindResult<T>`として明示的に指定する必要があります。

## 8. よくある質問

> **Q1:** `ForAttribute` と `Required` を同じパラメータに付けられますか？
>
> A1: いいえ。同一パラメータに複数の属性は指定できません。

> **Q2:** `ForText` はどこに書けばいいですか？
>
> A2: コンストラクタの最後のパラメータにのみ指定できます。

> **Q3:** `Required` 等の子要素属性と `ForText` は同時に使えますか？
>
> A3: いいえ。どちらか一方のみです。
