using ChatSample.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncHttpAzureFunc.LongTaskProcessor
{
    public class LongTaskHandler : ILongTaskHandler, IDisposable
    {
        private readonly Subject<IMessage> _subject;
        private readonly Dictionary<string, IDisposable> _subscribers;

        public LongTaskHandler()
        {
            _subject = new Subject<IMessage>();
            _subscribers = new Dictionary<string, IDisposable>();
        }

        public void Publish(IMessage eventMessage)
        {
            _subject.OnNext(eventMessage);
        }

        public void Subscribe(string subscriberName, Action<IMessage> action)
        {
            if (!_subscribers.ContainsKey(subscriberName))
            {
                _subscribers.Add(subscriberName, _subject.Subscribe(action));
            }
        }

        public void Subscribe(string subscriberName, Func<IMessage, bool> predicate, Action<IMessage> action)
        {
            if (!_subscribers.ContainsKey(subscriberName))
            {
                _subscribers.Add(subscriberName, _subject.Where(predicate).Subscribe(action));
            }
        }

        public void Dispose()
        {
            if (_subject != null)
            {
                _subject.Dispose();
            }

            foreach (var subscriber in _subscribers)
            {
                subscriber.Value.Dispose();
            }
        }
    }

    public interface ILongTaskHandler
    {
        void Publish(IMessage eventMessage);
        void Subscribe(string subscriberName, Action<IMessage> action);
        void Subscribe(string subscriberName, Func<IMessage, bool> predicate, Action<IMessage> action);
    }
}
