﻿using AutoMapper;
using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Helper;
using RMDesktopUI.Library.Models;
using RMDesktopUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RMDesktopUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private IProductEndpoint _productEndpoint;
        private ISaleEndpoint _saleEndPoint;
        private IConfigHelper _configHelper;
        private IMapper _mapper;
        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper, ISaleEndpoint saleEndPoint, IMapper mapper)
        {
            _productEndpoint = productEndpoint;
            _saleEndPoint = saleEndPoint;
            _configHelper = configHelper;
            _mapper = mapper;
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadProducts();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowsStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                var _status = IoC.Get<StatusInfoViewModel>();
                var _windows = IoC.Get<IWindowManager>();

                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unauthorized Access", "You do not have permission to interact with the Sales Form!");
                    _windows.ShowDialogAsync(_status, null, settings);
                }
                else
                {
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    _windows.ShowDialogAsync(_status, null, settings);
                }

                await TryCloseAsync();
            }

        }
        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAllAsync();
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);
            Products = new BindingList<ProductDisplayModel>(products);
        }
        private BindingList<ProductDisplayModel> _products;
        public BindingList<ProductDisplayModel> Products
        {
            get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);
            }
        }
        private ProductDisplayModel _selectedProduct;
        public ProductDisplayModel SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }
        private CartItemDisplayModel _selectedCartItem;
        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }
        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();
        public BindingList<CartItemDisplayModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
                NotifyOfPropertyChange(() => SubTotal);
            }
        }
        private string _itemQuantity = 1.ToString();
        public string ItemQuantity
        {
            get
            {
                return _itemQuantity;
            }
            set
            {
                _itemQuantity = value;
                if (ItemQuantity == null)
                {
                    quantity = 0;
                }
                else
                {
                    Int32.TryParse(ItemQuantity, out quantity);
                }
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }
        private int quantity  = 1;
        public string SubTotal
        {
            get
            {
                decimal subTotal = CalculateSubTotal();

                return subTotal.ToString("C");
            }
        }
        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;

            subTotal = Cart.Sum(x => x.Product.RetailPrice * x.QuantityInCart);

            return subTotal;
        }
        public string Tax
        {
            get
            {
                decimal taxAmount = CalculateTaxAmount();

                return taxAmount.ToString("C");
            }
        }
        private decimal CalculateTaxAmount()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxTate() / 100;

            taxAmount = Cart
                .Where(x => x.Product.IsTaxable)
                .Sum(x => x.Product.RetailPrice * x.QuantityInCart * taxRate);
            return taxAmount;
        }
        public string Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalculateTaxAmount();

                return total.ToString("C");
            }
        }
        public bool CanAddToCart
        {
            get
            {
                return SelectedProduct?.QuantityInStock >= quantity;
            }
        }
        public void AddToCart()
        {
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(x => x.Product == SelectedProduct);
            if (existingItem != null)
            {
                existingItem.QuantityInCart += quantity;
                Cart.Remove(existingItem);
                Cart.Add(existingItem);
            }
            else
            {
                var cartItem = new CartItemDisplayModel { Product = SelectedProduct, QuantityInCart = quantity };
                Cart.Add(cartItem);
            }

            SelectedProduct.QuantityInStock -= quantity;
            ItemQuantity = 1.ToString();
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }
        public bool CanRemoveFromCart
        {
            get
            {
                bool output = false;
                if (SelectedCartItem?.QuantityInCart > 0)
                {
                    output = true;
                }
                //return SelectedCartItem == null ? false : true;
                return output;
            }
        }
        public void RemoveFromCart()
        {
            SelectedCartItem.Product.QuantityInStock += 1;
            if (SelectedCartItem.QuantityInCart > 1)
            {
                SelectedCartItem.QuantityInCart -= 1;
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
        }
        public bool CanCheckOut
        {
            get
            {
                return Cart.Count > 0;
            }
        }
        public async Task CheckOut()
        {
            var saleModel = new SaleModel();
            foreach (var item in Cart)
            {
                var saleDetail = new SaleDetailsModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                };

                saleModel.SaleDetails.Add(saleDetail);
            }
            await _saleEndPoint.PostSaleAsync(saleModel);
            await ResetViewModel();

        }
        private async Task ResetViewModel()
        {
            Cart = new BindingList<CartItemDisplayModel>();
            await LoadProducts();

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
            NotifyOfPropertyChange(() => CanAddToCart);
        }
    }
}
