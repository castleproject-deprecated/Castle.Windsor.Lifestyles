using System;
using System.Threading;
using Castle.MicroKernel.Lifestyle.Contextual;
using Castle.MicroKernel.Registration;
using NUnit.Framework;

namespace Castle.MicroKernel.Lifestyle.Tests.Contextual
{
	[TestFixture]
	public class ContainerContextFixture
	{
		private IKernel kernel;

		[SetUp]
		public void SetUp()
		{
			kernel = new DefaultKernel();
		}

		[Test]
		public void Should_register_IContainerContextStore_when_first_context_is_created()
		{
			kernel.Register(Component.For<ComponentA>().LifeStyle.Custom(typeof(ContextualLifestyle)));
			new ContainerContext(kernel);
			Assert.That(kernel.HasComponent(typeof(IContainerContextStore)));
		}

		[Test]
		public void Should_resolve_the_same_instances_when_inside_the_same_context()
		{
			kernel.Register(Component.For<ComponentA>().LifeStyle.Custom(typeof(ContextualLifestyle)));
			using (new ContainerContext(kernel))
			{
				var c1 = kernel.Resolve<ComponentA>();
				var c2 = kernel.Resolve<ComponentA>();
				Assert.That(c1, Is.SameAs(c2));
			}
		}

		[Test]
		public void Should_resolve_different_instances_when_inside_different_contexts()
		{
			kernel.Register(Component.For<ComponentA>().LifeStyle.Custom(typeof(ContextualLifestyle)));

			ComponentA c1, c2;
			using (new ContainerContext(kernel))
			{
				c1 = kernel.Resolve<ComponentA>();
			}
			using (new ContainerContext(kernel))
			{
				c2 = kernel.Resolve<ComponentA>();
			}

			Assert.That(c1, Is.Not.SameAs(c2));
		}

		[Test]
		public void Should_resolve_in_a_new_context_when_nested_contexts_are_used()
		{
			kernel.Register(Component.For<ComponentA>().LifeStyle.Custom(typeof(ContextualLifestyle)));

			ComponentA c1, c2, c3, c4;
			using (new ContainerContext(kernel))
			{
				c1 = kernel.Resolve<ComponentA>();
				using (new ContainerContext(kernel))
				{
					c3 = kernel.Resolve<ComponentA>();
					c4 = kernel.Resolve<ComponentA>();
				}
				c2 = kernel.Resolve<ComponentA>();
			}

			Assert.That(c1, Is.SameAs(c2));
			Assert.That(c3, Is.SameAs(c4));
			Assert.That(c1, Is.Not.SameAs(c3));
		}

		[Test]
		public void Should_implicitly_initialize_a_new_context_when_there_is_none_created()
		{
			kernel.Register(Component.For<ComponentA>().LifeStyle.Custom(typeof(ContextualLifestyle)));

			var c1 = kernel.Resolve<ComponentA>();
			var c2 = kernel.Resolve<ComponentA>();
			Assert.That(c1, Is.Not.SameAs(c2));
		}

		[Test]
		public void Should_use_same_instance_within_resolution_chain()
		{
			kernel.Register(
				Component.For<ComponentA>().LifeStyle.Custom<ContextualLifestyle>(),
				Component.For<ComponentB>().LifeStyle.Transient,
				Component.For<ComponentC>().LifeStyle.Transient
				);
			using (new ContainerContext(kernel))
			{
				var c1 = kernel.Resolve<ComponentC>();
				Assert.That(c1.A, Is.SameAs(c1.B.A));
			}
		}

		[Test]
		public void Should_return_different_instances_when_outside_resolution_chain()
		{
			kernel.Register(
				Component.For<ComponentA>().LifeStyle.Custom<ContextualLifestyle>(),
				Component.For<ComponentB>().LifeStyle.Transient,
				Component.For<ComponentC>().LifeStyle.Transient
				);
			var c = kernel.Resolve<ComponentC>();
			var a = kernel.Resolve<ComponentA>();
			Assert.That(c.A, Is.Not.SameAs(a));
		}

		[Test]
		public void Should_work_in_multithreading_scenario_with_explicit_contexts()
		{
			kernel.Register(Component.For<ComponentA>().LifeStyle.Custom(typeof(ContextualLifestyle)));

			object instanceInBackgroundThread = null;
			var countInBackgroundThread = 0;

			object instanceInForegroundThread = null;
			var countInForegroundThread = 0;

			using (new ContainerContext(kernel))
			{
				var autoResetEvent = new AutoResetEvent(false);

				ThreadPool.QueueUserWorkItem(delegate
				{
					using (new ContainerContext(kernel))
					{
						for (int i = 0; i < 5000; i++)
						{
							var a = kernel.Resolve<ComponentA>();
							if (a != instanceInBackgroundThread)
							{
								countInBackgroundThread++;
								instanceInBackgroundThread = a;
							}
						}
					}
					autoResetEvent.Set();
				});

				using (new ContainerContext(kernel))
				{
					for (int i = 0; i < 5000; i++)
					{
						var a = kernel.Resolve<ComponentA>();
						if (a != instanceInForegroundThread)
						{
							countInForegroundThread++;
							instanceInForegroundThread = a;
						}
					}
				}

				autoResetEvent.WaitOne();
			}

			Assert.That(countInBackgroundThread, Is.EqualTo(1));
			Assert.That(countInForegroundThread, Is.EqualTo(1));
			Assert.That(instanceInBackgroundThread, Is.Not.SameAs(instanceInForegroundThread));
		}

		[Test]
		public void Should_work_in_multithreading_scenario_with_implicit_contexts()
		{
			kernel.Register(Component.For<ComponentA>().LifeStyle.Custom(typeof(ContextualLifestyle)));

			object instanceInBackgroundThread = null;
			var countInBackgroundThread = 0;

			object instanceInForegroundThread = null;
			var countInForegroundThread = 0;

			var are = new AutoResetEvent(false);

			ThreadPool.QueueUserWorkItem(delegate
				{
					for (int i = 0; i < 5000; i++)
					{
						var a = kernel.Resolve<ComponentA>();
						if (a != instanceInForegroundThread)
						{
							countInForegroundThread++;
							instanceInForegroundThread = a;
						}
					}
					are.Set();
				});

			for (int i = 0; i < 5000; i++)
			{
				var a = kernel.Resolve<ComponentA>();
				if (a != instanceInBackgroundThread)
				{
					countInBackgroundThread++;
					instanceInBackgroundThread = a;
				}
			}
			are.WaitOne();

			Assert.That(countInBackgroundThread, Is.EqualTo(5000));
			Assert.That(countInForegroundThread, Is.EqualTo(5000));
			Assert.That(instanceInBackgroundThread, Is.Not.SameAs(instanceInForegroundThread));
		}

        [Test]
        public void Should_release_components_when_context_disposed() {
            ComponentDisposable.Disposed = false;
            kernel.Register(Component.For<ComponentDisposable>().LifeStyle.Transient,
                            Component.For<ComponentD>().LifeStyle.Custom<ContextualLifestyle>());
            using (new ContainerContext(kernel)) {
                kernel.Resolve<ComponentD>();
            }
            Assert.IsTrue(ComponentDisposable.Disposed);
        }
	}

	public class ComponentA
	{
	}

	public class ComponentB
	{
		public ComponentA A;

		public ComponentB(ComponentA a)
		{
			A = a;
		}
	}

	public class ComponentC
	{
		public ComponentA A;
		public ComponentB B;

		public ComponentC(ComponentA a, ComponentB b)
		{
			A = a;
			B = b;
		}
	}

    public class ComponentD {
        public ComponentDisposable disposable;

        public ComponentD(ComponentDisposable disposable) {
            this.disposable = disposable;
        }
    }

    public class ComponentDisposable: IDisposable {
        public static bool Disposed;

        public void Dispose() {
            Disposed = true;
        }
    }
}
