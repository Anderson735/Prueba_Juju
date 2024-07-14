using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Interface
{
    internal interface ICustomer
    {
        bool CustomerExists(int customerId);
    }
}
