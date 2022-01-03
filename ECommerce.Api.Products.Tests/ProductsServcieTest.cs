using AutoMapper;
using ECommerce.Api.Products.Db;
using ECommerce.Api.Products.Profiles;
using ECommerce.Api.Products.Providers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

// ���� ������ ������ �� ������ xUnit .Net Core
// ��������� ������ �� ������ Products, �.�. ����� ��� �����������

// ����� ���������������� ������ ������, �������� ������ �� https://azure.microsoft.com/en-us/services/devops/
// https://www.linkedin.com/learning/azure-microservices-with-dot-net-core-for-developers/creating-an-azure-devops-project?autoAdvance=true&autoSkip=true&autoplay=true&resume=false&u=2113185

namespace ECommerce.Api.Products.Tests
{
    // ��� ������� ������ ��� �� �������� ������ -> Run Tests
    public class ProductsServcieTest
    {
        [Fact] // ����� ���������� ������������ xUnit, ���������� ���� ����
        public async Task GetProductsReturnsAllProducts()
        {
            // ��� ������ ���������� InMemoryDatabase, ����� �� ������ �� �������� ������
            var options = new DbContextOptionsBuilder<ProductsDbContext>().UseInMemoryDatabase(nameof(GetProductsReturnsAllProducts)).Options;
            var dbContext = new ProductsDbContext(options );
            CreateProducts(dbContext);

            var productProfile = new ProductProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(productProfile));
            var mapper = new Mapper(configuration);

            var productsProvider = new ProductsProvider(dbContext, null, mapper);
            var products = await productsProvider.GetProductsAsync();
            Assert.True(products.IsSuccess);
            Assert.True(products.Products.Any());
            Assert.Null(products.ErrorMessage);
        }

        [Fact]
        public async Task GetProductsReturnsProductUsingValidId()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>().UseInMemoryDatabase(nameof(GetProductsReturnsProductUsingValidId)).Options;
            var dbContext = new ProductsDbContext(options);
            CreateProducts(dbContext);

            var productProfile = new ProductProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(productProfile));
            var mapper = new Mapper(configuration);

            int productId = 1;
            var productsProvider = new ProductsProvider(dbContext, null, mapper);
            var product = await productsProvider.GetProductAsync(productId);
            Assert.True(product.IsSuccess);
            Assert.NotNull(product.Product);
            Assert.True(product.Product.Id == productId);
            Assert.Null(product.ErrorMessage);
        }

        [Fact]
        public async Task GetProductsReturnsProductUsingInvalidId()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>().UseInMemoryDatabase(nameof(GetProductsReturnsProductUsingInvalidId)).Options;
            var dbContext = new ProductsDbContext(options);
            CreateProducts(dbContext);

            var productProfile = new ProductProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(productProfile));
            var mapper = new Mapper(configuration);

            int productId = -1;
            var productsProvider = new ProductsProvider(dbContext, null, mapper);
            var product = await productsProvider.GetProductAsync(productId);
            Assert.False(product.IsSuccess);
            Assert.Null(product.Product);
            Assert.NotNull(product.ErrorMessage);
        }

        private void CreateProducts(ProductsDbContext dbContext)
        {
            for (int i = 1; i <= 10; i++)
            {
                dbContext.Products.Add(new Product()
                {
                    Id = i,
                    Name = Guid.NewGuid().ToString(),
                    Inventory = i + 10,
                    Price = (decimal)(i * 3.14)
                });
            }
            dbContext.SaveChanges();
        }
    }
}