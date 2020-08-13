using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Connections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageQueueLongRunnigTasks.Services.SignalR
{
    public class PersistentConnectionLifeTimeManager
    {
        private readonly ConnectionList _connectionList = new ConnectionList();

        public void OnConnectedAsync(ConnectionContext connection)
        {
            _connectionList.Add(connection);
        }

        public void OnDisconnectedAsync(ConnectionContext connection)
        {
            _connectionList.Remove(connection);
        }

        public async Task SendToAllAsync<T>(T data)
        {
            foreach (var connection in _connectionList)
            {
                var dataJson = JsonConvert.SerializeObject(data);
                var bytes = Encoding.UTF8.GetBytes(dataJson);
                await connection.Transport.Output.WriteAsync(bytes);
            }
        }

        public Task InvokeConnectionAsync(string connectionId, object data)
        {
            throw new NotImplementedException();
        }

        public Task InvokeGroupAsync(string groupName, object data)
        {
            throw new NotImplementedException();
        }

        public Task InvokeUserAsync(string userId, object data)
        {
            throw new NotImplementedException();
        }

        public void AddGroupAsync(ConnectionContext connection, string groupName)
        {
            var groups = (HashSet<string>)connection.Items["groups"];
            lock (groups)
            {
                groups.Add(groupName);
            }
        }

        public void RemoveGroupAsync(ConnectionContext connection, string groupName)
        {
            var groups = (HashSet<string>)connection.Items["groups"];
            if (groups != null)
            {
                lock (groups)
                {
                    groups.Remove(groupName);
                }
            }
        }
    }
}
