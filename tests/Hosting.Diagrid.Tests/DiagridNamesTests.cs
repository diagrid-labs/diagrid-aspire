using Diagrid.Aspire.Hosting;

namespace Hosting.Diagrid.Tests;

public class DiagridNamesTests
{
    [Theory]
    [InlineData("Test_Api")]
    [InlineData("TestApi")]
    [InlineData("-leading")]
    [InlineData("trailing-")]
    [InlineData("trailing\n")]
    [InlineData("")]
    [InlineData("café")]
    public void Rejects_an_aspire_name_diagrid_will_not_accept(string name)
    {
        var ex = Assert.Throws<ArgumentException>(() => DiagridNames.EnsureValidResourceName(name));
        Assert.Contains("lowercase", ex.Message, StringComparison.Ordinal);
        Assert.Contains($"'{name}'", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Rejects_a_name_longer_than_63_characters()
    {
        var name = new string('a', 64);
        var ex = Assert.Throws<ArgumentException>(() => DiagridNames.EnsureValidResourceName(name));
        Assert.Contains(name, ex.Message, StringComparison.Ordinal);
        Assert.Contains("lowercase", ex.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("test-api")]
    [InlineData("api2")]
    public void Accepts_valid_names(string name) => DiagridNames.EnsureValidResourceName(name);

    [Fact]
    public void Accepts_a_name_of_exactly_63_characters()
        => DiagridNames.EnsureValidResourceName(new string('a', 63));
}
