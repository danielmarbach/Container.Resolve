using System;
using Autofac;
using Autofac.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;
using StructureMap.Pipeline;

namespace Container.Resolve
{
    class Program
    {
        static void Main(string[] args)
        {
            NinjectSample();

            // not supported https://github.com/autofac/Autofac/issues/478#issuecomment-33536784
            // AutofacSample();

            StructureMapSample();

            // not supported https://github.com/castleproject/Windsor/blob/master/docs/passing-arguments.md#composition-root---containerresolve
            // WindsorSample();
        }

        private static void WindsorSample()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<TreeWithNode>().LifestyleTransient());
            container.Register(Component.For<TreeWithNodeWithoutDependency>().LifestyleTransient());
            container.Register(Component.For<Node>().LifestyleTransient());
            container.Register(Component.For<NodeWithoutDependency>().LifestyleTransient());

            var dependency = new Dependency(1);
            var treeWithNode = container.Resolve<TreeWithNode>(new Arguments(new { dependency }));
            var treeWithNodeWithoutDependency = container.Resolve<TreeWithNodeWithoutDependency>(new Arguments(new { dependency }));

            Console.WriteLine(treeWithNode.ToString());
            Console.WriteLine(treeWithNodeWithoutDependency.ToString());
        }

        private static void StructureMapSample()
        {
            var container = new StructureMap.Container(_ =>
            {
                _.For<TreeWithNode>().Transient();
                _.For<TreeWithNodeWithoutDependency>().Transient();
                _.For<Node>().Transient();
                _.For<NodeWithoutDependency>().Transient();
            });

            var dependency = new Dependency(1);
            var args = new ExplicitArguments();
            args.Set<Dependency>(dependency);
            var treeWithNode = container.GetInstance<TreeWithNode>(args);
            var treeWithNodeWithoutDependency = container.GetInstance<TreeWithNodeWithoutDependency>(args);

            Console.WriteLine(treeWithNode.ToString());
            Console.WriteLine(treeWithNodeWithoutDependency.ToString());

            Console.ReadLine();
        }

        private static void AutofacSample()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TreeWithNode>();
            builder.RegisterType<TreeWithNodeWithoutDependency>();
            builder.RegisterType<Node>();
            builder.RegisterType<NodeWithoutDependency>();

            var container = builder.Build();
            
            var dependency = new Dependency(1);
            var treeWithNode = container.Resolve<TreeWithNode>(TypedParameter.From(dependency));
            var treeWithNodeWithoutDependency = container.Resolve<TreeWithNodeWithoutDependency>(TypedParameter.From(dependency));

            Console.WriteLine(treeWithNode.ToString());
            Console.WriteLine(treeWithNodeWithoutDependency.ToString());

            Console.ReadLine();
        }

        private static void NinjectSample()
        {
            var kernel = new StandardKernel();
            kernel.Load(new Module());

            var dependency = new Dependency(1);
            var treeWithNode = kernel.Get<TreeWithNode>(new TypeMatchingConstructorArgument(typeof(Dependency), (c, t) => dependency, true));
            var treeWithNodeWithoutDependency = kernel.Get<TreeWithNodeWithoutDependency>(new TypeMatchingConstructorArgument(typeof(Dependency), (c, t) => dependency, true));
            Console.WriteLine(treeWithNode.ToString());
            Console.WriteLine(treeWithNodeWithoutDependency.ToString());
        }
    }

    class Module : NinjectModule
    {
        public override void Load()
        {
            this.Bind<TreeWithNode>().ToSelf();
            this.Bind<TreeWithNodeWithoutDependency>().ToSelf();
            this.Bind<Node>().ToSelf();
            this.Bind<NodeWithoutDependency>().ToSelf();
        }
    }

    public class TreeWithNode {
        private readonly Node node;

        public TreeWithNode(Node node)
        {
            this.node = node;
        }

        public override string ToString() {
            return node.ToString();
        }
    }

    public class TreeWithNodeWithoutDependency {
        private readonly NodeWithoutDependency node;

        public TreeWithNodeWithoutDependency(NodeWithoutDependency node)
        {
            this.node = node;
        }

        public override string ToString() {
            return node.ToString();
        }
    }

    public class Node
    {
        private readonly Dependency dependency;

        public Node(Dependency dependency) {
            this.dependency = dependency;
        }

        public override string ToString() {
            return dependency.ToString();
        }
    }

    public class NodeWithoutDependency
    {

    }

    public class Dependency {
        private readonly int id;

        public Dependency(int id) {
            this.id = id;
        }

        public override string ToString() {
            return id.ToString();
        }
    }
}
