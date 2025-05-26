namespace Maroontress.Oxbind.Test;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class ExampleTest
{
    [TestMethod]
    public void RequiredOptionalOneMultiple()
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <movie id="1" title="Avatar">
              <director name="James Cameron"/>
              <release year="2009"/>
              <cast>Sam Worthington</cast>
              <cast>Zoe Saldana</cast>
            </movie>
            """;
        var factory = new OxbinderFactory();
        var binder = factory.Of<Movie>();
        var reader = new StringReader(xml);
        var movie = binder.NewInstance(reader);

        Assert.AreEqual("1", movie.Id);
        Assert.AreEqual("Avatar", movie.Title);
        Assert.AreEqual("James Cameron", movie.TheDirector.Name);
        var maybeRelease = movie.MaybeRelease;
        Assert.IsNotNull(maybeRelease);
        Assert.AreEqual("2009", maybeRelease.Year);
        var castList = movie.Casts
            .ToList();
        Assert.AreEqual(2, castList.Count);
        Assert.AreEqual("Sam Worthington", castList[0].Name);
        Assert.AreEqual("Zoe Saldana", castList[1].Name);
    }

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
}
