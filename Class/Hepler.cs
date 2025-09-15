using beauty_salon.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beauty_salon
{
        public static class Helper
        {
                public static beauty_salonEntities GetContext()
                {
                        return new beauty_salonEntities( );
                }
        }
}
 