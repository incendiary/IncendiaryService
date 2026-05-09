using Xunit;

namespace IncendiaryService
{
    public class IncendiaryUserServiceTests
    {
        [Fact]
        public void Start_CreatesUser_WhenUserDoesNotExist()
        {
            var fake = new FakeLocalAccountManager();
            var service = new IncendiaryUserService(fake);

            service.Start();

            Assert.Contains("Incendiary", fake.CreatedUsers);
        }

        [Fact]
        public void Start_SkipsUserCreation_WhenUserAlreadyExists()
        {
            var fake = new FakeLocalAccountManager();
            fake.SeedUser("Incendiary");
            var service = new IncendiaryUserService(fake);

            service.Start();

            Assert.Empty(fake.CreatedUsers);
        }

        [Fact]
        public void Start_AddsUserToGroup_WhenNotAlreadyMember()
        {
            var fake = new FakeLocalAccountManager();
            var service = new IncendiaryUserService(fake);

            service.Start();

            Assert.Contains(("Incendiary", "Administrators"), fake.AddedToGroups);
        }

        [Fact]
        public void Start_SkipsGroupAdd_WhenUserAlreadyMember()
        {
            var fake = new FakeLocalAccountManager();
            fake.SeedUser("Incendiary");
            fake.SeedGroupMembership("Incendiary", "Administrators");
            var service = new IncendiaryUserService(fake);

            service.Start();

            Assert.Empty(fake.AddedToGroups);
        }
    }
}
