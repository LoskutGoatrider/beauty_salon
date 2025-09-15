using beauty_salon.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beauty_salon
{
        public static class ServiceExtensions
        {
                public static bool HasDiscount(this Service service)
                {
                        return service.Discount.HasValue && service.Discount > 0;
                }

                public static decimal CostWithDiscount(this Service service)
                {
                        return service.Discount.HasValue ?
                            service.Cost * (1 - (decimal)service.Discount.Value) : service.Cost;
                }

                public static string DurationFormatted(this Service service)
                {
                        TimeSpan duration = TimeSpan.FromSeconds(service.DurationInSeconds);
                        return $"{(int)duration.TotalHours:00}:{duration.Minutes:00}";
                }

                public static string BackgroundColor(this Service service)
                {
                        return service.Discount.HasValue && service.Discount > 0 ?
                            "LightGreen" : "Transparent";
                }
        }
}