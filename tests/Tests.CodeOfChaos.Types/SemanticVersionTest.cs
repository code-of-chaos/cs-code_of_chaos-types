// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Types;
using JetBrains.Annotations;

namespace Tests.CodeOfChaos.Types;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[TestSubject(typeof(SemanticVersion))]
public class SemanticVersionTest {
    // Test for constructor with integer parameters and ToString method
    [Test]
    [Arguments(1, 2, 3, "1.2.3")]
    [Arguments(10, 20, 30, "10.20.30")]
    public async Task TestSemanticVersionWithIntParams(int major, int minor, int patch, string expected) {
        // Arrange

        // Act
        var semanticVersion = new SemanticVersion(major, minor, patch);

        // Assert
        await Assert.That(semanticVersion.ToString()).IsEqualTo(expected);
        await Assert.That(semanticVersion.Major).IsEqualTo(major);
        await Assert.That(semanticVersion.Minor).IsEqualTo(minor);
        await Assert.That(semanticVersion.Patch).IsEqualTo(patch);
    }

    // Test for constructor with string parameter
    [Test]
    [Arguments("1.2.3", 1, 2, 3)]
    [Arguments("10.20.30", 10, 20, 30)]
    public async Task TestSemanticVersionConstructorWithStringParam_ValidString(string version, int major, int minor, int patch) {
        // Arrange

        // Act
        var semanticVersion = new SemanticVersion(version);

        // Assert
        await Assert.That(semanticVersion.ToString()).IsEqualTo(version);
        await Assert.That(semanticVersion.Major).IsEqualTo(major);
        await Assert.That(semanticVersion.Minor).IsEqualTo(minor);
        await Assert.That(semanticVersion.Patch).IsEqualTo(patch);
    }

    // Test for constructor with string parameter & addendum
    [Test]
    [Arguments("1.2.3-alpha", 1, 2, 3, "alpha")]
    [Arguments("10.20.30-beta", 10, 20, 30, "beta")]
    [Arguments("10.20.30-beta_gamma", 10, 20, 30, "beta_gamma")]
    public async Task TestSemanticVersionConstructorWithStringParamAddendum_ValidString(string version, int major, int minor, int patch, string addendum) {
        // Arrange

        // Act
        var semanticVersion = new SemanticVersion(version);

        // Assert
        await Assert.That(semanticVersion.ToString()).IsEqualTo(version);
        await Assert.That(semanticVersion.Major).IsEqualTo(major);
        await Assert.That(semanticVersion.Minor).IsEqualTo(minor);
        await Assert.That(semanticVersion.Patch).IsEqualTo(patch);
        await Assert.That(semanticVersion.Addendum).IsEqualTo(addendum);
    }

    // Test for constructor with string parameters (leading zeroes)
    [Test]
    [Arguments("01.002.0003", 1, 2, 3)]
    public async Task TestSemanticVersionConstructorWithStringLeadingZeroes_ValidString(string version, int major, int minor, int patch) {
        // Arrange

        // Act
        var semanticVersion = new SemanticVersion(version);

        // Assert
        await Assert.That(semanticVersion.Major).IsEqualTo(major);
        await Assert.That(semanticVersion.Minor).IsEqualTo(minor);
        await Assert.That(semanticVersion.Patch).IsEqualTo(patch);
    }

    // Test for comparison of versions with and without addendum
    [Test]
    [Arguments("1.2.3-alpha", "1.2.3")]
    public async Task TestSemanticVersionComparisonWithWithoutAddendum(string versionWithAddendum, string versionWithoutAddendum) {
        // Arrange

        // Act
        var semanticVersionWithAddendum = new SemanticVersion(versionWithAddendum);
        var semanticVersionWithoutAddendum = new SemanticVersion(versionWithoutAddendum);

        // Assert

        // Depending upon your requirements, the version with Addendum might be considered lesser, greater or equal
        await Assert.That(semanticVersionWithAddendum).IsGreaterThan(semanticVersionWithoutAddendum);
    }

    // Test for constructor with invalid string parameter
    [Test]
    [Arguments("1.2.3.4")]
    [Arguments("1.2")]
    [Arguments("abcd")]
    public async Task TestSemanticVersionConstructorWithStringParam_InvalidString(string version) {
        // Arrange

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => Task.FromResult(new SemanticVersion(version)));
    }

    // Test for constructor with string parameter & addendum
    [Test]
    [Arguments("1.2.3-alpha-")]
    [Arguments("10.20.30-beta-a")]
    public async Task TestSemanticVersionConstructorWithStringParamAddendum_InvalidString(string version) {
        // Arrange

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() => Task.FromResult(new SemanticVersion(version)));
    }

    // Test for equality (==)
    [Test]
    [Arguments(1, 1, 1, 1, 1, 1)]
    public async Task TestEqualityOperator(int majorA, int minorA, int patchA, int majorB, int minorB, int patchB) {
        // Arrange

        // Act
        SemanticVersion semanticVersionA = new(majorA, minorA, patchA);
        SemanticVersion semanticVersionB = new(majorB, minorB, patchB);

        // Assert
        await Assert.That(semanticVersionA).IsEqualTo(semanticVersionB);
    }

    // Test for equality (!=)
    [Test]
    [Arguments(1, 1, 1, 0, 0, 0)]
    [Arguments(1, 1, 1, 0, 1, 0)]
    [Arguments(1, 1, 1, 0, 0, 1)]
    public async Task TestInEqualityOperator(int majorA, int minorA, int patchA, int majorB, int minorB, int patchB) {
        // Arrange

        // Act
        SemanticVersion semanticVersionA = new(majorA, minorA, patchA);
        SemanticVersion semanticVersionB = new(majorB, minorB, patchB);

        // Assert
        await Assert.That(semanticVersionA).IsNotEqualTo(semanticVersionB);
    }

    // Test for less than (<)
    [Test]
    [Arguments(1, 0, 0, 2, 0, 0)]
    [Arguments(2, 1, 0, 2, 2, 0)]
    [Arguments(2, 2, 1, 2, 2, 2)]
    public async Task TestLessThanOperator(int majorA, int minorA, int patchA, int majorB, int minorB, int patchB) {
        // Arrange

        // Act
        SemanticVersion semanticVersionA = new(majorA, minorA, patchA);
        SemanticVersion semanticVersionB = new(majorB, minorB, patchB);

        // Assert
        await Assert.That(semanticVersionA).IsLessThan(semanticVersionB);
    }

    // Test for greater than (>)
    [Test]
    [Arguments(2, 0, 0, 1, 0, 0)]
    [Arguments(2, 2, 0, 2, 1, 0)]
    [Arguments(2, 2, 2, 2, 2, 1)]
    public async Task TestGreaterThanOperator(int majorA, int minorA, int patchA, int majorB, int minorB, int patchB) {
        // Arrange

        // Act
        SemanticVersion semanticVersionA = new(majorA, minorA, patchA);
        SemanticVersion semanticVersionB = new(majorB, minorB, patchB);

        // Assert
        await Assert.That(semanticVersionA).IsGreaterThan(semanticVersionB);
    }
    // Test for CompareTo
    [Test]
    [Arguments(1, 2, 3, 1, 2, 4, -1)]
    [Arguments(1, 2, 3, 1, 2, 2, 1)]
    [Arguments(1, 2, 3, 1, 2, 3, 0)]
    public async Task TestCompareTo(int majorA, int minorA, int patchA, int majorB, int minorB, int patchB, int expected) {
        // Arrange

        // Act
        SemanticVersion semanticVersionA = new(majorA, minorA, patchA);
        SemanticVersion semanticVersionB = new(majorB, minorB, patchB);
        int comparison = semanticVersionA.CompareTo(semanticVersionB);

        // Assert
        await Assert.That(expected).IsEqualTo(comparison);
    }

    // Test for CompareTo method with Addendum comparison
    [Test]
    [Arguments(1, 2, 3, "alpha", 1, 2, 3, null, 1)]
    [Arguments(1, 2, 3, null, 1, 2, 3, "alpha", -1)]
    [Arguments(1, 2, 3, "alpha", 1, 2, 3, "alpha", 0)]
    public async Task TestCompareToWithAddendum(int majorA, int minorA, int patchA, string? addendumA, int majorB, int minorB, int patchB, string? addendumB, int expected) {

        // Arrange

        // Act
        SemanticVersion semanticVersionA = new(majorA, minorA, patchA, addendumA);
        SemanticVersion semanticVersionB = new(majorB, minorB, patchB, addendumB);
        int comparison = semanticVersionA.CompareTo(semanticVersionB);

        // Assert
        await Assert.That(expected).IsEqualTo(comparison);
    }


    // Test for implicit string conversion
    [Test]
    [Arguments("1.2.3", 1, 2, 3)]
    public async Task TestImplicitStringConversion(string versionString, int major, int minor, int patch) {
        // Arrange

        // Act
        SemanticVersion version = versionString;

        // Assert
        await Assert.That(version.Major).IsEqualTo(major);
        await Assert.That(version.Minor).IsEqualTo(minor);
        await Assert.That(version.Patch).IsEqualTo(patch);
    }

    // Test for implicit SemanticVersion conversion
    [Test]
    [Arguments(1, 2, 3, "1.2.3")]
    public async Task TestImplicitSemanticVersionConversion(int major, int minor, int patch, string expected) {
        // Arrange
        var version = new SemanticVersion(major, minor, patch);

        // Act
        string versionString = version;

        // Assert
        await Assert.That(versionString).IsEqualTo(expected);
    }

    // Test for TryParse
    [Test]
    [Arguments("1.2.3", 1, 2, 3)]
    public async Task TestTryParse_Valid(string input, int expectedMajor, int expectedMinor, int expectedPatch) {
        // Arrange

        // Act
        bool result = SemanticVersion.TryParse(input, out SemanticVersion version);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(version.Major).IsEqualTo(expectedMajor);
        await Assert.That(version.Minor).IsEqualTo(expectedMinor);
        await Assert.That(version.Patch).IsEqualTo(expectedPatch);
    }

    [Test]
    [Arguments("invalid")]
    public async Task TestTryParse_Invalid(string input) {
        // Arrange

        // Act
        bool result = SemanticVersion.TryParse(input, out SemanticVersion version);

        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(version.Major).IsEqualTo(0);
        await Assert.That(version.Minor).IsEqualTo(0);
        await Assert.That(version.Patch).IsEqualTo(0);
        await Assert.That(version).IsEqualTo(SemanticVersion.Zero);
    }

    // Test for Equals
    [Test]
    [Arguments(1, 1, 1, true)]
    [Arguments(1, 2, 3, false)]
    public async Task TestEquals(int major, int minor, int patch, bool expected) {
        // Arrange

        // Act
        var versionA = new SemanticVersion(1, 1, 1);
        var versionB = new SemanticVersion(major, minor, patch);

        // Assert
        await Assert.That(versionA.Equals(versionB)).IsEqualTo(expected);
    }

    // Test for GetHashCode
    [Test]
    public async Task TestGetHashCode() {
        // Arrange
        var versionA = new SemanticVersion(1, 2, 3);
        var versionB = new SemanticVersion(1, 2, 3);

        // Act
        int hash1 = versionA.GetHashCode();
        int hash2 = versionB.GetHashCode();

        // Assert
        await Assert.That(hash1).IsEqualTo(hash2);
    }

    // Test for Default Constructor
    [Test]
    public async Task TestDefaultConstructor() {
        // Arrange

        // Act
        var version = new SemanticVersion();

        // Assert
        await Assert.That(version.Major).IsEqualTo(0);
        await Assert.That(version.Minor).IsEqualTo(0);
        await Assert.That(version.Patch).IsEqualTo(0);
        await Assert.That(version.Addendum).IsNullOrEmpty();
    }

    // Test for Equals(object)
    [Test]
    [Arguments(1, 2, 3, true)]
    [Arguments(1, 2, 4, false)]
    public async Task TestObjectEquals(int major, int minor, int patch, bool expected) {
        // Arrange

        // Act
        var versionA = new SemanticVersion(1, 2, 3);
        object versionB = new SemanticVersion(major, minor, patch);

        // Assert
        await Assert.That(versionA.Equals(versionB)).IsEqualTo(expected);
    }

    // Test for less than or equal operator (<=)
    [Test]
    [Arguments(1, 2, 3, 1, 2, 3, true)]
    [Arguments(1, 2, 3, 1, 2, 4, true)]
    [Arguments(1, 2, 4, 1, 2, 3, false)]
    public async Task TestLessThanOrEqualOperator(int majorA, int minorA, int patchA, int majorB, int minorB, int patchB, bool expected) {
        // Arrange
        var versionA = new SemanticVersion(majorA, minorA, patchA);
        var versionB = new SemanticVersion(majorB, minorB, patchB);

        // Act
        bool lessThanOrEqual = versionA <= versionB;

        // Assert
        await Assert.That(lessThanOrEqual).IsEqualTo(expected);
    }

    // Test for greater than or equal operator (>=)
    [Test]
    [Arguments(1, 2, 3, 1, 2, 3, true)]
    [Arguments(1, 2, 4, 1, 2, 3, true)]
    [Arguments(1, 2, 3, 1, 2, 4, false)]
    public async Task TestGreaterThanOrEqualOperator(int majorA, int minorA, int patchA, int majorB, int minorB, int patchB, bool expected) {
        // Arrange
        var versionA = new SemanticVersion(majorA, minorA, patchA);
        var versionB = new SemanticVersion(majorB, minorB, patchB);

        // Act
        bool greaterThanOrEqual = versionA >= versionB;

        // Assert
        await Assert.That(greaterThanOrEqual).IsEqualTo(expected);
    }

    [Test]
    public async Task TestMax() {
        // Arrange
        SemanticVersion max = SemanticVersion.Max;

        // Act

        // Assert
        await Assert.That(max).IsEqualTo(SemanticVersion.Max);
        await Assert.That(max.Major).IsEqualTo(int.MaxValue);
        await Assert.That(max.Minor).IsEqualTo(int.MaxValue);
        await Assert.That(max.Patch).IsEqualTo(int.MaxValue);
        await Assert.That(max.Addendum).IsNullOrEmpty();
    }

    [Test]
    public async Task TestZero() {
        // Arrange
        SemanticVersion max = SemanticVersion.Zero;

        // Act

        // Assert
        await Assert.That(max).IsEqualTo(SemanticVersion.Zero);
        await Assert.That(max.Major).IsEqualTo(0);
        await Assert.That(max.Minor).IsEqualTo(0);
        await Assert.That(max.Patch).IsEqualTo(0);
        await Assert.That(max.Addendum).IsNullOrEmpty();
    }
}
