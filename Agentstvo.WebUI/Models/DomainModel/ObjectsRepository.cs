using System.Collections.Generic;
using System.Web.Mvc;

namespace Agentstvo.WebUI.Models.DomainModel
{
    public static class ObjectsRepository
    {
        private static IList<ObjectForSale> _objectsForSale;

        public static IList<ObjectForSale> ObjectForSales
        {
            get
            {
                _objectsForSale = _objectsForSale ?? new List<ObjectForSale>()
                {
                    new ObjectForSale
                    {
                        Id = 1,
                        Description = "Объект для продажи",
                        ImageLocation = "~/Content/images/Home1.jpg"
                    },
                    new ObjectForSale
                    {
                        Id = 2,
                        Description = "Объект для продажи",
                        ImageLocation = "~/Content/images/Home2.jpg"
                    },
                    new ObjectForSale
                    {
                        Id = 3,
                        Description = "Объект для продажи",
                        ImageLocation = "~/Content/images/Home3.jpg"
                    },
                };
                return _objectsForSale;

            }
        }
    }
}