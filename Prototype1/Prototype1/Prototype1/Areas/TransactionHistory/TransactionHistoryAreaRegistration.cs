using System.Web.Mvc;

namespace Prototype1.Areas.TransactionHistory
{
    public class TransactionHistoryAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TransactionHistory";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "TransactionHistory_default",
                "TransactionHistory/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}