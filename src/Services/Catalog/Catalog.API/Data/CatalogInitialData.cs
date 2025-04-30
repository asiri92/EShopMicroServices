using Marten.Schema;

namespace Catalog.API.Data
{
    public class CatalogInitialData : IInitialData
    {
        public async Task Populate(IDocumentStore store, CancellationToken cancellation)
        {
            using var session = store.LightweightSession();

            if(await session.Query<Product>().AnyAsync(cancellation))
                return;

            // marten UPSERT with cater for existing records
            session.Store<Product>(GetPreConfiguredProducts());
            await session.SaveChangesAsync();
        }

        private static IEnumerable<Product> GetPreConfiguredProducts() => new List<Product>
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product 1",
                Category = new List<string> { "Category 1" },
                Description = "Description 1",
                ImageFile = "ImageFile 1",
                Price = 2000.0m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product 2",
                Category = new List<string> { "Category 2" },
                Description = "Description 2",
                ImageFile = "ImageFile 2",
                Price = 1500.0m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product 3",
                Category = new List<string> { "Category 3" },
                Description = "Description 1",
                ImageFile = "ImageFile 1",
                Price = 850.0m
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product 4",
                Category = new List<string> { "Category 4" },
                Description = "Description 2",
                ImageFile = "ImageFile 2",
                Price = 199.46m
            }
        };
       
    }
}
