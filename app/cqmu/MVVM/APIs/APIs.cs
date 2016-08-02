using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cqmu.MVVM.APIs
{
    public class APIs
    {
        public static string APIGetCourse
        {
            get
            {
                return "http://localhost:17514/GetCourse.aspx?";
            }
        }

        public static string APICetQuery
        {
            get
            {
                return "http://localhost:17514/CetQuery.aspx?";
            }
        }
    }
}
