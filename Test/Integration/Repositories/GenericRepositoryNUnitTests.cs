using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using Test.BusinessObjects;
using Test.Fixture;

namespace Test.Integration.Repositories
{
    [TestFixture]
    [Category("Integration")]

    public class GenericRepositoryNUnitTests
    {
        private MongoOnlineFixture _fx;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            _fx = new MongoOnlineFixture();

            // Ensuring we can connect
            var cursor = await _fx.Client.ListDatabaseNamesAsync();
            _ = await cursor.ToListAsync();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _fx.DropDatabase();
        }

        private static ObjectId NewId() => ObjectId.GenerateNewId();

        // small helper (reuse your generator)
        private static Dummy MakeDummy(string name, int qty) => new Dummy
        {
            Id = ObjectId.GenerateNewId(),
            DummyName = name,
            DummyQuantity = qty
        };

        [Test]
        public async Task GetAll_And_Count_Works()
        {
            var repo = _fx.CreateRepo();
            var ct = CancellationToken.None;

            var a = MakeDummy("A", 1);
            var b = MakeDummy("B", 2);
            var c = MakeDummy("C", 10);

            await repo.InsertAsync(a, ct);
            await repo.InsertAsync(b, ct);
            await repo.InsertAsync(c, ct);

            var all = await repo.GetAllAsync(ct);
            Assert.That(all.Count, Is.GreaterThanOrEqualTo(3));

            var countGe2 = await repo.CountAsync(x => x.DummyQuantity >= 2, ct);
            Assert.That(countGe2, Is.GreaterThanOrEqualTo(2));

            var total = await repo.CountAsync(ct: ct);
            Assert.That(total, Is.GreaterThanOrEqualTo(3));
        }

        [Test]
        public async Task Get_With_Filter_Returns_Expected()
        {
            var repo = _fx.CreateRepo();
            var ct = CancellationToken.None;

            var high = MakeDummy("Nut", 10);
            var low = MakeDummy("Screw", 1);

            await repo.InsertAsync(high, ct);
            await repo.InsertAsync(low, ct);

            var hiQty = await repo.GetAsync(x => x.DummyQuantity >= 5, ct);
            Assert.That(hiQty.Select(x => x.Id), Does.Contain(high.Id));
            Assert.That(hiQty.Select(x => x.Id), Does.Not.Contain(low.Id));
        }

        [Test]
        public async Task Replace_UpsertFalse_Updates_When_Exists()
        {
            var repo = _fx.CreateRepo();
            var ct = CancellationToken.None;

            var original = MakeDummy("Old", 1);
            await repo.InsertAsync(original, ct);

            var updatedEntity = new Dummy
            {
                Id = original.Id,
                DummyName = "New",
                DummyQuantity = 99
            };

            var ok = await repo.ReplaceAsync(updatedEntity, upsert: false, ct);
            Assert.That(ok, Is.True);

            var fetched = await repo.GetByIdAsync(original.Id, ct);
            Assert.That(fetched, Is.Not.Null);
            Assert.That(fetched!.DummyName, Is.EqualTo("New"));
            Assert.That(fetched.DummyQuantity, Is.EqualTo(99));
        }

        [Test]
        public async Task Replace_UpsertTrue_Inserts_When_Missing()
        {
            var repo = _fx.CreateRepo();
            var ct = CancellationToken.None;

            var id = NewId();
            var entity = new Dummy { Id = id, DummyName = "Fresh", DummyQuantity = 3 };

            var ok = await repo.ReplaceAsync(entity, upsert: true, ct);
            Assert.That(ok, Is.True);

            var fetched = await repo.GetByIdAsync(id, ct);
            Assert.That(fetched, Is.Not.Null);
            Assert.That(fetched!.DummyName, Is.EqualTo("Fresh"));
            Assert.That(fetched.DummyQuantity, Is.EqualTo(3));
        }

        [Test]
        public async Task DeleteById_And_DeleteEntity_Works()
        {
            var repo = _fx.CreateRepo();
            var ct = CancellationToken.None;

            // DeleteById
            var e1 = MakeDummy("Trash", 0);
            await repo.InsertAsync(e1, ct);

            var del1 = await repo.DeleteByIdAsync(e1.Id, ct);
            Assert.That(del1, Is.True);
            Assert.That(await repo.GetByIdAsync(e1.Id, ct), Is.Null);

            // Delete(entity)
            var e2 = MakeDummy("Trash2", 0);
            await repo.InsertAsync(e2, ct);

            var del2 = await repo.DeleteAsync(e2, ct);
            Assert.That(del2, Is.True);
            Assert.That(await repo.GetByIdAsync(e2.Id, ct), Is.Null);
        }
    }
}
