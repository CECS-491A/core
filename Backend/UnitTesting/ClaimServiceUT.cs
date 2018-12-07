using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Services;

namespace UnitTesting
{
    [TestClass]
    public class ClaimServiceUT
    {
        ClaimService claimService;
        User user1;
        User user2;
        Service service1;
        Service service2;
        Claim claim1;

        public ClaimServiceUT()
        {
            var testUtils = new TestingUtils();

            claimService = new ClaimService();

            user1 = testUtils.CreateUser();
            user2 = testUtils.CreateUser();

            service1 = testUtils.CreateService(true);
            service2 = testUtils.CreateService(true);

            

            claim1 = testUtils.CreateClaim(user1, service1, user2);
        }

        [TestMethod]
        public void CreateClient()
        {
            Client newClient = new Client
            {
                Disabled = false,
                Name = "" + Guid.NewGuid() + "",
            };
            int response = claimService.CreateClient(newClient);

            Assert.IsTrue(response > 0);
        }

        [TestMethod]
        public void CreateClaim()
        {
            Claim newClaim = new Claim
            {
                UserId = user1.Id,
                ServiceId = service1.Id,
                UpdatedAt = DateTime.UtcNow
            };

            Client newClient = new Client
            {
                Disabled = false,
                Name = "" + Guid.NewGuid() + ""
            };
            claimService.CreateClient(newClient);

            // ACT
            int response = claimService.CreateClaim(user1.Id, service1.Id, newClient.Id);

            // Assert
            Assert.IsTrue(response > 0);
        }

        [TestMethod]
        public void getService()
        {
            Service received = claimService.getService(service1.ServiceName);

            StringAssert.Contains(received.ServiceName, service1.ServiceName);
        }

        [TestMethod]
        public void addServiceToUser()
        {
            claimService.addServiceToUser(user2, service2, user1);

            using (var _db = new DatabaseContext())
            {
                int count = _db.Claims
                    .Where(c => c.UserId == user2.Id && c.ServiceId == service2.Id && c.SubjectUserId == user1.Id)
                    .Count();
                
                Assert.IsTrue(count > 0);
            }
        }

        [TestMethod]
        public void userHasServiceAccess()
        {
            bool hasAccess = claimService.userHasServiceAccess(user1, service1, user2);

            Assert.IsTrue(hasAccess);
        }
    }
}
