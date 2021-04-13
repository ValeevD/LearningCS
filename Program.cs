using System;
using System.Threading.Tasks;

using System.Collections.Generic;
using System.Reflection;

public class SomeClass : ISubscriber
{
    void ISubscriber.Do()
    {
        Console.WriteLine("SomeClass");
        EventBus.UnsubscribeAll(this);
    }
}

public class SomeClassAsync : ISubscriberAsync
{
    async Task ISubscriberAsync.Do()
    {
        Task tsk = new Task(DoSomeLoop);

        tsk.Start();
        //await Task.Run(() => DoSomeLoop());
        await tsk;
    }

    void ISubscriber.Do()
    {
    }

    public void DoSomeLoop(){
        int k = 0;

        for(int i = 0; i < 100000000; ++i){
            if(i % 1000000 == 0){
                k++;
                Console.WriteLine("-----Inner loop: " + k);
            }
        }
    }
}

public class Program
{
    static void Main(string[] args)
    {
        SomeClassAsync someClassAsync = new SomeClassAsync();

        EventBus.Subscribe<ISubscriberAsync>(someClassAsync);

        Task[] tasks = EventBus.RaiseEventAsync<ISubscriberAsync>((subscr) => { return subscr.Do();});

        while(true){
            int k = 0;
            bool endLoop = false;

            for(int i = 0; i < 100000000; ++i){
                if(Task.WhenAll(tasks).IsCompleted){
                    endLoop = true;
                    break;
                }

                if(i % 300000 == 0){
                    k++;
                    Console.WriteLine("-----Outter loop: " + k);
                }
            }

            if(endLoop)
                break;
        }
    }

}
