using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Stuff.Communication
{
    public interface IAutoSebscriber
    {
        void SubscribeAsync(params Type[] reciversTypes);
    }

    public class AutoSubscriber : IAutoSebscriber
    {
        private readonly IReciverDispathcer _dispathcer;
        private readonly IReceiverBus _bus;

        public AutoSubscriber(IReciverDispathcer dispathcer, IReceiverBus bus)
        {
            _dispathcer = dispathcer;
            _bus = bus;
        }

        public void SubscribeAsync(params Type[] reciversTypes)
        {
            var subscriptionInfos = GetSubscriptionInfos(reciversTypes, typeof(IReciveveAsync<>));

            Type SubscriberDelegate(Type messageType) => typeof(Func<,>).MakeGenericType(messageType, typeof(Task));


            foreach (var kv in subscriptionInfos)
            {
                foreach (var subscriptionInfo in kv.Value)
                {
                    var dispatchMethod = _dispathcer.GetType()
                                  .GetMethod("DispatchAsync")// todo ....
                                  .MakeGenericMethod(subscriptionInfo.MessageType, subscriptionInfo.ConcreteType);

                    var busMethod = typeof(IReceiverBus).GetMethods()
                        .Single(m => m.Name == "SubscribeAsync").MakeGenericMethod(subscriptionInfo.MessageType);

                    var dispatchDelegate = Delegate.CreateDelegate(SubscriberDelegate(subscriptionInfo.MessageType), _dispathcer, dispatchMethod);

                    busMethod.Invoke(_bus, new object[] { dispatchDelegate });
                }
            }
        }

        private static IEnumerable<KeyValuePair<Type, AutoSubscriberReceiverInfo[]>> GetSubscriptionInfos(IEnumerable<Type> types, Type interfaceType)
        {
            foreach (var concreteType in types.Where(t => t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract))
            {
                var subscriptionInfos = concreteType.GetInterfaces()
                    .Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == interfaceType && !i.GetGenericArguments()[0].IsGenericParameter)
                    .Select(i => new AutoSubscriberReceiverInfo(concreteType, i.GetGenericArguments()[0]))
                    .ToArray();

                if (subscriptionInfos.Any())
                    yield return new KeyValuePair<Type, AutoSubscriberReceiverInfo[]>(concreteType, subscriptionInfos);
            }
        }

        private class AutoSubscriberReceiverInfo
        {
            public Type ConcreteType { get; }
            public Type MessageType { get; }

            public AutoSubscriberReceiverInfo(Type concreteType, Type messageType)
            {
                ConcreteType = concreteType;
                MessageType = messageType;
            }
        }
    }
}
