using Xunit;
using FluentAssertions;

namespace LMSA.Tests;

/// <summary>
/// Tests to verify the basic solution structure is set up correctly.
/// </summary>
public class SolutionStructureTests
{
    [Fact]
    public void Solution_Should_Build_Successfully()
    {
        // This test passes if the solution builds, which it does!
        true.Should().BeTrue("Solution structure is created correctly");
    }

    [Fact]
    public void Test_Framework_Has_Moq_Available()
    {
        // Verify Moq is available
        var mockType = typeof(Moq.Mock);
        mockType.Should().NotBeNull("Moq package should be installed");
    }

    [Fact]
    public void Test_Framework_Has_FluentAssertions_Available()
    {
        // Verify FluentAssertions is available
        var assertionsType = typeof(FluentAssertions.AssertionExtensions);
        assertionsType.Should().NotBeNull("FluentAssertions package should be installed");
    }
}
