using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyIoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyIoC.Tests
{
    [TestClass]
    public class ContainerTests
    {
        public Container Container { get; set; }
        [TestInitialize]
        public void CreateContainer()
        {
            Container = new Container();
            Container.AddAssembly(Assembly.GetAssembly(typeof(Container)));
        }

        [TestMethod]
        public void AdditionOfResolvetype()
        {
            Container.AddType(typeof(CustomerBLL));
            Container.AddType(typeof(Logger));
            Container.AddType(typeof(CustomerDAL), typeof(ICustomerDAL));
            Assert.IsTrue(Container.ContainerTypes[typeof(CustomerBLL)] != null && Container.ContainerTypes[typeof(CustomerBLL)] == typeof(CustomerBLL));
            Assert.IsTrue(Container.ContainerTypes[typeof(Logger)] != null && Container.ContainerTypes[typeof(Logger)] == typeof(Logger));
            Assert.IsTrue(Container.ContainerTypes[typeof(ICustomerDAL)] != null && Container.ContainerTypes[typeof(ICustomerDAL)] == typeof(CustomerDAL));

        }

        [TestMethod]
        public void ConstructorInjection()
        {
            var customerBLL = (CustomerBLL)Container.CreateInstance(typeof(CustomerBLL));

            Assert.IsTrue(customerBLL != null);
            Assert.AreEqual(typeof(CustomerDAL), customerBLL.Dal.GetType());
            Assert.AreEqual(typeof(Logger), customerBLL.Logger.GetType());
        }

        [TestMethod]
        public void PropertiesInjection()
        {
            var customerBLL2 = Container.CreateInstance<CustomerBLL2>();
 
            Assert.IsTrue(customerBLL2 != null);
            Assert.AreEqual(typeof(CustomerDAL), customerBLL2.CustomerDAL.GetType());
            Assert.AreEqual(typeof (Logger), customerBLL2.logger.GetType());

        }

        [TestMethod]
        [ExpectedException(typeof(ResolvedTypeNotFoundException))]
        public void ExceptionIsArrised()
        {
            var s = Container.CreateInstance(typeof(OutOfAssembly));
        }
    }
    public class OutOfAssembly { }
}
