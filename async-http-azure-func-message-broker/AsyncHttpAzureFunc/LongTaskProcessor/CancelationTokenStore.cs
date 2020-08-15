using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncHttpAzureFunc.LongTaskProcessor
{
    public class CancelationTokenStore
    {
        private readonly ConcurrentDictionary<Guid, CancellationTokenSource> tokens = new ConcurrentDictionary<Guid, CancellationTokenSource>();

        public CancellationTokenSourceScope CreateScope(Guid id)
        {
            return new CancellationTokenSourceScope(id, this);
        }

        public CancellationTokenSource Add(Guid id)
        {
            var tokenSource = new CancellationTokenSource();
            tokens.TryAdd(id, tokenSource);
            return tokenSource;
        }

        public void Cancel(Guid id)
        {
            if (tokens.TryRemove(id, out var tokenSource))
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
        }

        public void Remove(Guid id)
        {
            if (tokens.TryRemove(id, out var tokenSource))
            {
                tokenSource.Dispose();
            }
        }

        public class CancellationTokenSourceScope : IDisposable
        {
            private readonly Guid id;
            private readonly CancelationTokenStore store;
            public CancellationToken Token { get; }

            public CancellationTokenSourceScope(Guid id, CancelationTokenStore store)
            {
                this.id = id;
                this.store = store;
                Token = store.Add(id).Token;
            }

            public void Dispose()
            {
                store.Remove(id);
            }
        }
    }


}
