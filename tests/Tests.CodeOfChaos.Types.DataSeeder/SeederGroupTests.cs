// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Types;

namespace Tests.CodeOfChaos.Types.DataSeeder;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class SeederGroupTests {
    [Test]
    public async Task Constructor_ShouldInitializeEmptySeederGroup() {
        // Arrange

        // Act
        var seederGroup = new SeederGroup();

        // Assert
        await Assert.That(seederGroup.SeederTypes)
            .IsEmpty()
            .And.HasCount().EqualToZero();
    }

    [Test]
    public async Task AddSeeder_ShouldAddSeederType() {
        // Arrange
        var seederGroup = new SeederGroup();

        // Act
        seederGroup.AddSeeder<TestSeeder>();

        // Assert
        await Assert.That(seederGroup.SeederTypes)
            .HasCount().EqualTo(1)
            .And.Contains(typeof(TestSeeder));
    }


    public class TestSeeder : Seeder {
        public override Task SeedAsync(CancellationToken ct = default) => Task.CompletedTask;
    }
}
