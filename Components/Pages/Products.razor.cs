using Microsoft.AspNetCore.Components;
using static System.Net.WebRequestMethods;

namespace ProductPricing.Components.Pages
{
    public class ProductsBase : ComponentBase
    {
        private List<ProductDto>? _products;
        private ProductHistoryDto? _selectedHistory;
        private string _message = string.Empty;
        private bool _isError;

        private readonly Dictionary<int, decimal> _discountInputs = new();
        private readonly Dictionary<int, decimal> _priceInputs = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            try
            {
                _products = await Http.GetFromJsonAsync<List<ProductDto>>("api/products");
                if (_products is not null)
                {
                    foreach (var p in _products)
                    {
                        _discountInputs.TryAdd(p.Id, 0);
                        _priceInputs.TryAdd(p.Id, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Failed to load products: {ex.Message}", true);
            }
        }

        private async Task ApplyDiscount(int id)
        {
            if (!_discountInputs.TryGetValue(id, out var discount) || discount < 0 || discount > 100)
            {
                ShowMessage("Discount must be between 0% and 100%.", true);
                return;
            }

            try
            {
                var response = await Http.PostAsJsonAsync($"api/products/{id}/apply-discount",
                    new { discountPercentage = discount });

                if (response.IsSuccessStatusCode)
                {
                    ShowMessage($"Discount of {discount}% applied successfully.", false);
                    await LoadProducts();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ShowMessage($"Error: {error}", true);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Request failed: {ex.Message}", true);
            }
        }

        private async Task UpdatePrice(int id)
        {
            if (!_priceInputs.TryGetValue(id, out var newPrice) || newPrice <= 0)
            {
                ShowMessage("Price must be a positive number.", true);
                return;
            }

            try
            {
                var response = await Http.PutAsJsonAsync($"api/products/{id}/update-price",
                    new { newPrice });

                if (response.IsSuccessStatusCode)
                {
                    ShowMessage($"Price updated to {newPrice:C} successfully.", false);
                    await LoadProducts();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ShowMessage($"Error: {error}", true);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Request failed: {ex.Message}", true);
            }
        }

        private async Task ViewHistory(int id)
        {
            try
            {
                _selectedHistory = await Http.GetFromJsonAsync<ProductHistoryDto>($"api/products/{id}");
            }
            catch (Exception ex)
            {
                ShowMessage($"Failed to load history: {ex.Message}", true);
            }
        }

        private void ShowMessage(string message, bool isError)
        {
            _message = message;
            _isError = isError;
            StateHasChanged();
        }

        private class ProductDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public DateTime LastUpdated { get; set; }
        }

        private class ProductHistoryDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public List<PriceHistoryEntryDto> PriceHistory { get; set; } = [];
        }

        private class PriceHistoryEntryDto
        {
            public decimal Price { get; set; }
            public string Date { get; set; } = string.Empty;
        }
    }
}
