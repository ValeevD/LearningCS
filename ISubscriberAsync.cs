using System.Threading.Tasks;

public interface ISubscriberAsync : ISubscriber
{
    new Task Do();
}
