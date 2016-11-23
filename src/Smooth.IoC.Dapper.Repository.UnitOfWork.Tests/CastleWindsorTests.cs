﻿using System;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Dapper.FastCrud;
using NUnit.Framework;
using Smooth.IoC.Dapper.FastCRUD.Repository.UnitOfWork.Tests.RepositoryTests;
using Smooth.IoC.Dapper.FastCRUD.Repository.UnitOfWork.Tests.TestClasses;
using Smooth.IoC.Dapper.Repository.UnitOfWork.Data;
using Castle.Core.Internal;
using Smooth.IoC.Dapper.Repository.UnitOfWork.Castle;

namespace Smooth.IoC.Dapper.FastCRUD.Repository.UnitOfWork.Tests
{
    [TestFixture]
    public class CastleWindsorTests
    {
        private static IWindsorContainer _container;

        [SetUp]
        public void TestSetup()
        {
            if (_container == null)
            {
                _container = new WindsorContainer();
                Assert.DoesNotThrow(() =>
                {
                    _container.Install(new SmoothIoCDapperRepositoryUnitOfWorkInstaller());
                    _container.Register(Classes.FromThisAssembly()
                        .Where(t => t.GetInterfaces().Length > 0 && 
                            t.GetInterfaces().Any(x => x != typeof(IDisposable)) 
                            && !t.HasAttribute<NoIoC>())
                        .Unless(t => t.IsAbstract)
                        .Configure(c =>
                        {
                            c.IsFallback();
                        })
                        .LifestyleTransient()
                        .WithServiceAllInterfaces());
                });
            }
        }

        [Test, Category("Integration")]
        public static void Install_1_Resolves_ISession()
        {
            var dbFactory = _container.Resolve<IDbFactory>();
            ITestSession session = null;
            Assert.DoesNotThrow(() => session = dbFactory.CreateSession<ITestSession>());
            Assert.That(session, Is.Not.Null);
        }


        [Test, Category("Integration")]
        public static void Install_2_Resolves_IUnitOfWork()
        {
            var dbFactory = _container.Resolve<IDbFactory>();
            using (var session = dbFactory.CreateSession<ITestSession>())
            {
                IUnitOfWork uow = null;
                Assert.DoesNotThrow(()=> uow = session.UnitOfWork());
                Assert.That(uow, Is.Not.Null);
            }
        }

        [Test, Category("Integration")]
        public static void Install_3_Resolves_SqlDialectCorrectly()
        {
            var dbFactory = _container.Resolve<IDbFactory>();
            using (var session = dbFactory.CreateSession<ITestSession>())
            {
                Assert.That(session.SqlDialect== SqlDialect.SqLite);
                var uow = session.UnitOfWork();
                Assert.That(uow.SqlDialect == SqlDialect.SqLite);
            }
        }

        [Test, Category("Integration")]
        public static void Install_4_Resolves_IBravoRepository()
        {
            IBraveRepository repo = null;
            Assert.DoesNotThrow(() => repo = _container.Resolve<IBraveRepository>());
            Assert.That(repo, Is.Not.Null);
        }
    }
}