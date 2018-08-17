using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class HapiConfiguration
    {
        private HapiCatalog.Catalog _catalog;
        public HapiCatalog.Catalog Catalog
        {
            get
            {
                if (_catalog == null)
                {
                    _catalog = new HapiCatalog.Catalog();
                    _catalog.CreateCatalog();
                    return _catalog;
                }
                else
                {
                    return _catalog;
                }
            }
        }
    }
}
