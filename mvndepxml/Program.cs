using System;

namespace mvndepxml
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1) throw new ArgumentException("please pass in result file of running 'mvn dependency:tree -Dverbose -DoutputFile=%res_file%");
           GenerateXml(args[0]);
        }

        static void GenerateXml(string fileName)
        {
            var root = Node.Load(fileName);
            root.Print(n =>
            {
                for(var i = 0; i < n.Depth; ++i) Console.Write('-');
                Console.WriteLine(n.Content);
            });
        }

    }
}
