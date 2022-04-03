using Xunit;
using FluentAssertions;

namespace KioskBrowser.Tests;

public class KioskBrowserComponentTests
{
    [Theory]
    [InlineData("dev")]
    [InlineData("beta")]
    [InlineData("canary")]
    [InlineData("webview")]
    public void IsInstalled_ShouldReturnTrue(string installType)
    {
        var sut = new WebViewComponentFake(installType);

        sut.IsInstalled
            .Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void IsInstalled_ShouldReturnFalse(string installType)
    {
        var sut = new WebViewComponentFake(installType);

        sut.IsInstalled
            .Should().BeFalse();
    }
}