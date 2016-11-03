using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;

namespace Nop.Plugin.DiscountRules.IsSpendingAmount
{
    public partial class IsSpendingAmountDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IPriceCalculationService _priceCalculationService;

        public IsSpendingAmountDiscountRequirementRule(ISettingService settingService,
            IStoreContext storeContext, IWorkContext workContext, IPriceCalculationService priceCalculationService)
        {
            _settingService = settingService;
            _workContext = workContext;
            _storeContext = storeContext;
            _priceCalculationService = priceCalculationService;
        }

        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>true - requirement is met; otherwise, false</returns>
        public bool CheckRequirement(CheckDiscountRequirementRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.DiscountRequirement == null)
                throw new NopException("Discount requirement is not set");

            var spentAmountRequirement = _settingService.GetSettingByKey<decimal>(string.Format("DiscountRequirement.IsSpendingAmount-{0}", request.DiscountRequirement.Id));

            if (spentAmountRequirement == decimal.Zero)
                return true;

            if (request.Customer == null || request.Customer.IsGuest())
                return false;

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .Where(sci => sci.StoreId == _storeContext.CurrentStore.Id)
                .ToList();

            var subTotal = cart.Sum(shoppingCartItem => _priceCalculationService.GetSubTotal(shoppingCartItem, true));

            var spentAmount = subTotal;
            
            return spentAmount > spentAmountRequirement;
        }

        /// <summary>
        /// Get URL for rule configuration
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
        /// <returns>URL</returns>
        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            //configured in RouteProvider.cs
            string result = "Plugins/DiscountRulesIsSpendingAmount/Configure/?discountId=" + discountId;
            if (discountRequirementId.HasValue)
                result += string.Format("&discountRequirementId={0}", discountRequirementId.Value);
            return result;
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.IsSpendingAmount.Fields.Amount", "Required spending amount");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.IsSpendingAmount.Fields.Amount.Hint", "Discount will be applied if customer is spending/ordering x.xx amount.");
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.DiscountRules.IsSpendingAmount.Fields.Amount");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.IsSpendingAmount.Fields.Amount.Hint");
            base.Uninstall();
        }
    }
}