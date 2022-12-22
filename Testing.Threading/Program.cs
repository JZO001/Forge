using Forge.Threading;
using Forge.Threading.Tasking;

namespace Testing.Threading
{

    internal class Program
    {

        static void Main(string[] args)
        {
            Action action0 = new Action(ActionEmpty);
            action0.BeginInvoke(new ReturnCallback(ActionCallback0), action0);

            Action<string> action1 = new Action<string>(ActionP1);
            action1.BeginInvoke("data", new ReturnCallback(ActionCallback1), action1);

            Func<int> fn0 = new Func<int>(FuncEmpty);
            fn0.BeginInvoke<int>(FuncCallback, fn0);

            Func<string, int> fn1 = new Func<string, int>(FuncP1);
            fn1.BeginInvoke<string, int>("data", FuncCallback, fn0);

            Console.ReadKey();
        }

        #region Actions

        static void ActionEmpty() 
        {
            Console.WriteLine("Running....");
            Thread.Sleep(1000);
            Console.WriteLine("Completed");
        }

        static void ActionP1(string data)
        {
            Console.WriteLine("Running....");
            Console.WriteLine(data);
            Thread.Sleep(1000);
            Console.WriteLine("Completed");
        }

        static void ActionCallback0(ITaskResult result)
        {
            Action action = (Action)result.AsyncState;
            action?.EndInvoke(result);
            Console.WriteLine("Callback called");
        }

        static void ActionCallback1(ITaskResult result)
        {
            Action<string> action = (Action<string>)result.AsyncState;
            action?.EndInvoke(result);
            Console.WriteLine("Callback called");
        }

        #endregion

        #region Functions

        static int FuncEmpty()
        {
            Console.WriteLine("Func running....");
            Thread.Sleep(1000);
            Console.WriteLine("Func completed");
            return 100;
        }

        static int FuncP1(string data)
        {
            Console.WriteLine($"Func running...., data: {data}");
            Thread.Sleep(1000);
            Console.WriteLine("Func completed");
            return 100;
        }

        static void FuncCallback(ITaskResult result)
        {
            Func<int> func = (Func<int>)result.AsyncState;
            int? data = func?.EndInvoke(result);
            Console.WriteLine($"Func callback called, data: {data}");
        }

        #endregion

    }

}