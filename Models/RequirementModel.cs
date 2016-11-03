using Nop.Web.Framework;

namespace Nop.Plugin.DiscountRules.IsSpendingAmount.Models
{
    public class RequirementModel
    {
        [NopResourceDisplayName("Plugins.DiscountRules.IsSpendingAmount.Fields.Amount")]
        public decimal SpentAmount { get; set; }

        public int DiscountId { get; set; }

        public int RequirementId { get; set; }
    }
}