using Microsoft.AspNetCore.Components;
using ProductPricing.Helpers;
using ProductPricing.Models;

namespace ProductPricing.Components.Pages
{
    public class ProductsBase : ComponentBase
    {
        [Inject]
        protected IApiHelper Api { get; set; } = default!;

        protected List<Product>? _products;
        protected Product? _selectedHistory;
        protected string _message = string.Empty;
        protected bool _isError;

        protected readonly Dictionary<int, decimal> _discountInputs = new();
        protected readonly Dictionary<int, decimal> _priceInputs = new();
        protected const string DisplayAsCurrency = "C";
        protected const string DisplayAsDate = "g";

        protected override async Task OnInitializedAsync()
        {
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            try
            {
                _products = await Api.GetAsync<List<Product>>("api/products");
                if (_products is not null)
                {
                    foreach (var p in _products)
                    {
                        _discountInputs.TryAdd(p.Id, 0);
                        _priceInputs.TryAdd(p.Id, 0);
                    }
                }

                await RefreshHistory();
            }
            catch (Exception ex)
            {
                ShowMessage($"Failed to load products: {ex.Message}", true);
            }
        }

        private async Task RefreshHistory()
        {
            if (_selectedHistory is not null)
            {
                _selectedHistory = await Api.GetAsync<Product>($"api/products/{_selectedHistory.Id}");
            }
        }

        protected async Task ApplyDiscount(int id)
        {
            if (!_discountInputs.TryGetValue(id, out var discount) || discount < 0 || discount > 100)
            {
                ShowMessage("Discount must be between 0% and 100%.", true);
                return;
            }

            try
            {
                await Api.PostAsync<ApplyDiscountResponse>($"api/products/{id}/apply-discount", new { discountPercentage = discount });
                ShowMessage($"Discount of {discount}% applied successfully.", false);
                await LoadProducts();
            }
            catch (Exception ex)
            {
                ShowMessage($"Request failed: {ex.Message}", true);
            }
        }

        protected async Task ClearDiscount(int id)
        {
            try
            {
                await Api.PostAsync<ApplyDiscountResponse>($"api/products/{id}/apply-discount", new { discountPercentage = 0 });
                ShowMessage("Discount cleared.", false);
                await LoadProducts();
            }
            catch (Exception ex)
            {
                ShowMessage($"Request failed: {ex.Message}", true);
            }
        }

        protected async Task UpdatePrice(int id)
        {
            if (!_priceInputs.TryGetValue(id, out var newPrice) || newPrice <= 0)
            {
                ShowMessage("Price must be a positive number.", true);
                return;
            }

            try
            {
                await Api.PutAsync<UpdatePriceResponse>($"api/products/{id}/update-price", new { newPrice });
                ShowMessage($"Price updated to {newPrice:C} successfully.", false);
                await LoadProducts();
            }
            catch (Exception ex)
            {
                ShowMessage($"Request failed: {ex.Message}", true);
            }
        }

        protected async Task ViewHistory(int id)
        {
            try
            {
                _selectedHistory = await Api.GetAsync<Product>($"api/products/{id}");
            }
            catch (Exception ex)
            {
                ShowMessage($"Failed to load history: {ex.Message}", true);
            }
        }

        protected void ShowMessage(string message, bool isError)
        {
            _message = message;
            _isError = isError;
            StateHasChanged();
        }
    }
}
