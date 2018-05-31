using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyIoC
{
	[ImportConstructor]
	public class CustomerBLL
	{
        public Logger Logger { get; set;}
        public ICustomerDAL Dal { get; set;}
        public CustomerBLL(ICustomerDAL dal, Logger logger)
        {
            Dal = dal;
            Logger = logger;
        }
	}

	public class CustomerBLL2
	{
		[Import]
		public ICustomerDAL CustomerDAL { get; set; }
		[Import]
		public Logger logger { get; set; }
	}
}
