using System;
using Autofac;
using Ninject;
using Ninject.Modules;
using Ninject.Parameters;

namespace Container.Resolve
{
    class Program
    {
        static void Main(string[] args)
        {
            Ninject();

            Autofac();
        }

        private static void Autofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TreeWithNode>().AsSelf();
            builder.RegisterType<TreeWithNodeWithoutDependency>().AsSelf();
            builder.RegisterType<Node>().AsSelf();
            builder.RegisterType<NodeWithoutDependency>().AsSelf();

            var container = builder.Build();

            var dependency = new Dependency(1);
            var treeWithNode = container.Resolve<TreeWithNode>(TypedParameter.From(dependency));
            var treeWithNodeWithoutDependency = container.Resolve<TreeWithNodeWithoutDependency>(TypedParameter.From(dependency));

            Console.WriteLine(treeWithNode.ToString());
            Console.WriteLine(treeWithNodeWithoutDependency.ToString());

            Console.ReadLine();
        }

        private static void Ninject()
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
