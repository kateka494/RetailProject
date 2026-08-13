using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using RetailProject.Models;
using RetailProject.Options;

namespace RetailProject.Services
{
    public class TableStorageService
    {
        private readonly AzureStorageOptions _settings;
        private readonly TableClient _customerTable;
        private readonly TableClient _productTable;

        public TableStorageService(IOptions<AzureStorageOptions> settings)
        {
            _settings = settings.Value;

            _customerTable = new TableClient(_settings.ConnectionString, _settings.CustomersTable);
            _productTable = new TableClient(_settings.ConnectionString, _settings.ProductsTable);
        }

        public async Task InitializeAsync()
        {
            await _customerTable.CreateIfNotExistsAsync();
            await _productTable.CreateIfNotExistsAsync();
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            var customers = new List<Customer>();
            try
            {
               
                await foreach (var entity in _customerTable.QueryAsync<TableEntity>())
                {
                    customers.Add(new Customer
                    {
                        PartitionKey = entity.PartitionKey,
                        RowKey = entity.RowKey,
                        CustomerId = entity["CustomerId"]?.ToString() ?? "",
                        FirstName = entity["FirstName"]?.ToString() ?? "",
                        LastName = entity["LastName"]?.ToString() ?? "",
                        EmailAddress = entity["EmailAddress"]?.ToString() ?? "",
                        ContactNumber = entity["ContactNumber"]?.ToString() ?? "",
                        Location = entity["Location"]?.ToString() ?? "",
                        IsActive = entity["IsActive"] != null && (bool)entity["IsActive"],
                        DateRegistered = entity["DateRegistered"] != null ? DateTime.Parse(entity["DateRegistered"].ToString()) : DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR GETTING CUSTOMERS: {ex.Message}");
            }
            return customers;
        }

        public async Task SaveCustomerAsync(Customer customer)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customer.CustomerId))
                    customer.CustomerId = Guid.NewGuid().ToString();

                if (string.IsNullOrWhiteSpace(customer.RowKey))
                    customer.RowKey = customer.CustomerId;

                customer.PartitionKey = "Customer";
                customer.DateRegistered = DateTime.UtcNow;

                var entity = new TableEntity(customer.PartitionKey, customer.RowKey)
                {
                    { "CustomerId", customer.CustomerId },
                    { "FirstName", customer.FirstName },
                    { "LastName", customer.LastName },
                    { "EmailAddress", customer.EmailAddress },
                    { "ContactNumber", customer.ContactNumber },
                    { "Location", customer.Location },
                    { "PasswordHash", customer.PasswordHash },
                    { "PasswordSalt", customer.PasswordSalt },
                    { "IsActive", customer.IsActive },
                    { "LoginAttempts", customer.LoginAttempts },
                    { "DateRegistered", customer.DateRegistered }
                };

                
                await _customerTable.UpsertEntityAsync(entity, TableUpdateMode.Replace);
                System.Diagnostics.Debug.WriteLine($"CUSTOMER SAVED: {customer.RowKey}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CRITICAL ERROR: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteCustomerAsync(string customerId)
        {
            try
            {
                await _customerTable.DeleteEntityAsync("Customer", customerId);
            }
            catch (RequestFailedException ex) when (ex.Status == 404) { }
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = new List<Product>();
            await foreach (var p in _productTable.QueryAsync<Product>())
                products.Add(p);
            return products;
        }

        public async Task<Product?> GetProductAsync(string productId)
        {
            try
            {
                var result = await _productTable.GetEntityAsync<Product>("Product", productId);
                return result.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404) { return null; }
        }

        public async Task SaveProductAsync(Product product)
        {
            product.PartitionKey = "Product";
            if (string.IsNullOrWhiteSpace(product.RowKey))
                product.RowKey = product.ProductId;
            await _productTable.UpsertEntityAsync(product, TableUpdateMode.Replace);
        }

        public async Task DeleteProductAsync(string productId)
        {
            try { await _productTable.DeleteEntityAsync("Product", productId); }
            catch (RequestFailedException ex) when (ex.Status == 404) { }
        }
    }
}