using beauty_salon.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beauty_salon.Class
{
        public partial class ClientService
        {
                public DateTime EndTime
                {
                        get
                        {
                                using (var context = Helper.GetContext())
                                {
                                        var service = context.Service.Find(ServiceID);
                                        return StartTime.AddSeconds(service.DurationInSeconds);
                                }
                        }
                }
        }
}
