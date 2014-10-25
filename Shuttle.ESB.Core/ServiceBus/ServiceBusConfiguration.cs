using System;
using System.Collections.Generic;
using Shuttle.Core.Infrastructure;

namespace Shuttle.ESB.Core
{
	public class ServiceBusConfiguration : IServiceBusConfiguration
	{
		private static ServiceBusSection _serviceBusSection;

		private readonly List<IEncryptionAlgorithm> _encryptionAlgorithms = new List<IEncryptionAlgorithm>();
		private readonly List<ICompressionAlgorithm> _compressionAlgorithms = new List<ICompressionAlgorithm>();

		private ISerializer _serializer;
		private ISubscriptionManager _subscriptionManager;

		public ServiceBusConfiguration()
		{
			WorkerAvailabilityManager = new WorkerAvailabilityManager();
			Modules = new ModuleCollection();
			TransactionScope = new TransactionScopeConfiguration();
			QueueManager = new QueueManager();
			Serializer = new DefaultSerializer();
			MessageHandlerFactory = new DefaultMessageHandlerFactory();
			MessageRouteProvider = new DefaultMessageRouteProvider();
			PipelineFactory = new DefaultPipelineFactory();
			TransactionScopeFactory = new DefaultTransactionScopeFactory();
			Policy = new DefaultServiceBusPolicy();
			ThreadActivityFactory = new DefaultThreadActivityFactory();
		}

		public static ServiceBusSection ServiceBusSection
		{
			get
			{
				return _serviceBusSection ?? (_serviceBusSection = ShuttleConfigurationSection.Open<ServiceBusSection>());
			}
		}

		public ISerializer Serializer
		{
			get { return _serializer; }
			set
			{
				Guard.AgainstNull(value, "serializer");

				if (value.Equals(Serializer))
				{
					return;
				}

				_serializer = value;
			}
		}

		public IInboxQueueConfiguration Inbox { get; set; }
		public IControlInboxQueueConfiguration ControlInbox { get; set; }
		public IOutboxQueueConfiguration Outbox { get; set; }
		public IWorkerConfiguration Worker { get; set; }
		public ITransactionScopeConfiguration TransactionScope { get; set; }

		public IQueueManager QueueManager { get; set; }
		public IIdempotenceService IdempotenceService { get; set; }

		public ModuleCollection Modules { get; private set; }

		public IMessageHandlerFactory MessageHandlerFactory { get; set; }
		public IMessageRouteProvider MessageRouteProvider { get; set; }
		public IServiceBusPolicy Policy { get; set; }
		public IThreadActivityFactory ThreadActivityFactory { get; set; }
		public DeferredMessageProcessor DeferredMessageProcessor { get; set; }

		public bool HasIdempotenceService
		{
			get { return IdempotenceService != null; }
		}

		public bool HasSubscriptionManager
		{
			get { return _subscriptionManager != null; }
		}

		public ISubscriptionManager SubscriptionManager
		{
			get
			{
				if (!HasSubscriptionManager)
				{
					throw new SubscriptionManagerException(ESBResources.NoSubscriptionManager);
				}

				return _subscriptionManager;
			}
			set { _subscriptionManager = value; }
		}

		public bool HasDeferredQueue
		{
			get { return HasInbox && Inbox.DeferredQueue != null; }
		}

		public bool HasInbox
		{
			get { return Inbox != null; }
		}

		public bool HasOutbox
		{
			get { return Outbox != null; }
		}

		public bool HasControlInbox
		{
			get { return ControlInbox != null; }
		}

		public bool HasServiceBusSection
		{
			get { return ServiceBusSection != null; }
		}

		public bool RemoveMessagesNotHandled { get; set; }
		public string EncryptionAlgorithm { get; set; }
		public string CompressionAlgorithm { get; set; }

		public IWorkerAvailabilityManager WorkerAvailabilityManager { get; private set; }

		public IPipelineFactory PipelineFactory { get; set; }

		public ITransactionScopeFactory TransactionScopeFactory { get; set; }

		public IServiceBus StartServiceBus()
		{
			return new ServiceBus(this);
		}

		public IEncryptionAlgorithm FindEncryptionAlgorithm(string name)
		{
			return
				_encryptionAlgorithms.Find(algorithm => algorithm.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
		}

		public void AddEncryptionAlgorithm(IEncryptionAlgorithm algorithm)
		{
			Guard.AgainstNull(algorithm, "algorithm");

			_encryptionAlgorithms.Add(algorithm);
		}

		public ICompressionAlgorithm FindCompressionAlgorithm(string name)
		{
			return
				_compressionAlgorithms.Find(algorithm => algorithm.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
		}

		public void AddCompressionAlgorithm(ICompressionAlgorithm algorithm)
		{
			Guard.AgainstNull(algorithm, "algorithm");

			_compressionAlgorithms.Add(algorithm);
		}

		public bool IsWorker
		{
			get { return Worker != null; }
		}
	}
}