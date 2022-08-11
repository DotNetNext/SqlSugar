using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface IRazorService
    {
        List<KeyValuePair<string, string>> GetClassStringList(string razorTemplate, List<RazorTableInfo> model);
    }
}
