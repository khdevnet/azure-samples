using AsyncHttpAzureFunc.LongTaskProcessor;
using Automatonymous;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncHttpAzureFunc.Data
{
    public class MessageProcessSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public DateTime Timestamp { get; set; }

        // If using Optimistic concurrency, this property is required
        public byte[] RowVersion { get; set; }
    }

    public class MessageProcessStateMap : SagaClassMap<MessageProcessSagaState>
    {
        protected override void Configure(EntityTypeBuilder<MessageProcessSagaState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);
            // If using Optimistic concurrency, otherwise remove this property
            entity.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}