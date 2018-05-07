using DatabaseInterfaces;
using RabbitCore;
using RabbitMongoService;
using ServiceInterfaces;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace NoteAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Create container
            var container = new UnityContainer();

            // Register types
            RegisterTypes(container);

            // Set resolver
            config.DependencyResolver = new UnityResolver(container);

            // Edit config
            EditConfiguration(config);
        }

        private static void RegisterTypes(UnityContainer container)
        {
            // NOTE: should try and do most of this by NAMING convention!
            // If you are registering all types like this, better to use POOR man's Dependency Injection!
            // Keys
            var rabbit = "rabbit";
            var mongo = "mongo";
            var client = "client";
            var service = "service";
            var creator = "creator";
            var reader = "reader";
            var updater = "updater";
            var deleter = "deleter";
            // Other
            var hostName = "localhost";
            var exchangeName = "";
            // Use default exchange "" to use queueName as routing key.

            // Rabbit Client     
            var noteApiQueueName = "NoteApi";
            var createNoteQueueName = "CreateNote";
            var readNotesQueueName = "ReadNotes";
            var updateNoteQueueName = "UpdateNote";
            var deleteNoteQueueName = "DeleteNote";
            container.RegisterType<IMessageConsumer, MessageConsumer>(new InjectionConstructor(hostName, noteApiQueueName));

            container.RegisterType<IMessagePublisher, MessagePublisher>(creator, new InjectionConstructor(hostName, exchangeName, createNoteQueueName));
            container.RegisterType<IMessagePublisher, MessagePublisher>(reader, new InjectionConstructor(hostName, exchangeName, readNotesQueueName));
            container.RegisterType<IMessagePublisher, MessagePublisher>(updater, new InjectionConstructor(hostName, exchangeName, updateNoteQueueName));
            container.RegisterType<IMessagePublisher, MessagePublisher>(deleter, new InjectionConstructor(hostName, exchangeName, deleteNoteQueueName));

            container.RegisterType<IEntryCreator, RabbitCore.EntryCreator >(rabbit, new InjectionConstructor(new ResolvedParameter<IMessagePublisher>(creator)));
            container.RegisterType<IEntryReader , RabbitCore.EntryReader >(rabbit, new InjectionConstructor(new ResolvedParameter<IMessageConsumer>(), new ResolvedParameter<IMessagePublisher>(reader)));
            container.RegisterType<IEntryUpdater, RabbitCore.EntryUpdater>(rabbit, new InjectionConstructor(new ResolvedParameter<IMessagePublisher>(updater)));
            container.RegisterType<IEntryDeleter, RabbitCore.EntryDeleter>(rabbit, new InjectionConstructor(new ResolvedParameter<IMessagePublisher>(deleter)));
            container.RegisterType<IRabbitEntryBroker, RabbitCore.EntryBroker>(
                new HierarchicalLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<IEntryCreator>(rabbit),
                    new ResolvedParameter<IEntryReader>(rabbit),
                    new ResolvedParameter<IEntryUpdater>(rabbit),
                    new ResolvedParameter<IEntryDeleter>(rabbit)
                )
            );

            // Mongo Client
            var databaseName = "Test";
            var collectionName = "Objects";
            container.RegisterType<IEntryCreator, MongoClient.EntryCreator>(
                mongo, 
                new InjectionConstructor(new ResolvedParameter<IEntryReader>(mongo), databaseName, collectionName));
            container.RegisterType<IEntryReader , MongoClient.EntryReader >(mongo, new InjectionConstructor(databaseName, collectionName));
            container.RegisterType<IEntryUpdater, MongoClient.EntryUpdater>(mongo, new InjectionConstructor(databaseName, collectionName));
            container.RegisterType<IEntryDeleter, MongoClient.EntryDeleter>(mongo, new InjectionConstructor(databaseName, collectionName));
            container.RegisterType<IMongoEntryBroker, MongoClient.EntryBroker>(
                new InjectionConstructor(
                    new ResolvedParameter<IEntryCreator>(mongo),
                    new ResolvedParameter<IEntryReader >(mongo),
                    new ResolvedParameter<IEntryUpdater>(mongo),
                    new ResolvedParameter<IEntryDeleter>(mongo)
                )
            );

            // Rabbit Mongo Service
            container.RegisterType<IMessagePublisher, MessagePublisher>(service, new InjectionConstructor(hostName, exchangeName, noteApiQueueName));
            container.RegisterType<IAutoMessageConsumer, AutoMessageConsumer>(creator, new InjectionConstructor(hostName, createNoteQueueName));
            container.RegisterType<IAutoMessageConsumer, AutoMessageConsumer>(reader, new InjectionConstructor(hostName, readNotesQueueName));
            container.RegisterType<IAutoMessageConsumer, AutoMessageConsumer>(updater, new InjectionConstructor(hostName, updateNoteQueueName));
            container.RegisterType<IAutoMessageConsumer, AutoMessageConsumer>(deleter, new InjectionConstructor(hostName, deleteNoteQueueName));

            container.RegisterType<IAutoConsumerCreator, AutoConsumerCreator>(
                new InjectionConstructor(
                    new ResolvedParameter<IAutoMessageConsumer>(creator), 
                    new ResolvedParameter<IEntryCreator>(mongo)
                )
            );
            container.RegisterType<IAutoConsumerReader , AutoConsumerReader >(
                 new InjectionConstructor(
                    new ResolvedParameter<IAutoMessageConsumer>(reader),
                    new ResolvedParameter<IEntryReader>(mongo),
                    new ResolvedParameter<IMessagePublisher>(service)
                )
            );
            container.RegisterType<IAutoConsumerUpdater, AutoConsumerUpdater>(
                new InjectionConstructor(
                    new ResolvedParameter<IAutoMessageConsumer>(updater),
                    new ResolvedParameter<IEntryUpdater>(mongo)
                )
            );
            container.RegisterType<IAutoConsumerDeleter, AutoConsumerDeleter>(
                new InjectionConstructor(
                    new ResolvedParameter<IAutoMessageConsumer>(deleter),
                    new ResolvedParameter<IEntryDeleter>(mongo)
                )
            );
            container.RegisterType<IService, CrudService>(
                new InjectionConstructor(
                    new ResolvedParameter<IAutoConsumerCreator>(),
                    new ResolvedParameter<IAutoConsumerReader >(),
                    new ResolvedParameter<IAutoConsumerUpdater>(),
                    new ResolvedParameter<IAutoConsumerDeleter>()
                )
            );
        }

        private static void EditConfiguration(HttpConfiguration config)
        {
            // Format as JSON
            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));
            //doh, this was all I needed to show json in browser.
            //did not need curl!

            // Allow CORS
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            //This allows your react app to access this API.

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

    }
}
