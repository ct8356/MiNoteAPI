using DatabaseInterfaces;
using MongoClient;
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
            // Keys
            var rabbit = "rabbit";
            var mongo = "mongo";

            // Rabbit Core
            container.RegisterType<IMessagePublisher, RabbitPublisher>
                (new InjectionConstructor("localhost", "", "NoteAPI"));
            // Use default exchange "" to use queueName as routing key.

            // Rabbit Client
            container.RegisterType<IMessageConsumer, RabbitConsumer>();
            container.RegisterType<IObjectCreator, RabbitCreator>(rabbit);
            container.RegisterType<IObjectReader , RabbitReader >(rabbit);
            container.RegisterType<IObjectUpdater, RabbitUpdater>(rabbit);
            container.RegisterType<IObjectDeleter, RabbitDeleter>(rabbit);
            container.RegisterType<IObjectBroker , RabbitBroker >(
                new HierarchicalLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<IObjectCreator>(rabbit),
                    new ResolvedParameter<IObjectReader >(rabbit),
                    new ResolvedParameter<IObjectUpdater>(rabbit),
                    new ResolvedParameter<IObjectDeleter>(rabbit)
                )
            ); //need to name it as rabbit somehow, so controller knows to use THIS one!
            //Maybe I can just Register the controller here myself?

            // Rabbit Service
            container.RegisterType<IAutoMessageConsumer, RabbitAutoConsumer>();

            // Mongo Client
            container.RegisterType<IObjectCreator, MongoCreator>(mongo);
            container.RegisterType<IObjectReader , MongoReader >(mongo);
            //container.RegisterType<IObjectUpdater, MongoUpdater>(mongo);
            //container.RegisterType<IObjectDeleter, MongoDeleter>(mongo);
            container.RegisterType<IObjectBroker , MongoBroker >(
                mongo,
                new InjectionConstructor(
                    new ResolvedParameter<IObjectCreator>(mongo),
                    new ResolvedParameter<IObjectReader >(mongo),
                    new ResolvedParameter<IObjectUpdater>(mongo),
                    new ResolvedParameter<IObjectDeleter>(mongo)
                )
            );

            // Rabbit Mongo Service
            container.RegisterType<IAutoMessageConsumerObjectCreator, RabbitAutoConsumerMongoCreator>();
            container.RegisterType<IObjectReaderMessagePublisher, MongoReaderRabbitPublisher>();
            //AH! Possible that the above, should autoConsume, then read, then publish!
            //container.RegisterType<IAutoConsumerEntryUpdater, RabbitMongoUpdater>();
            //container.RegisterType<IAutoConsumerEntryDeleter, RabbitMongoDeleter>();
            container.RegisterType<IService, Service>(
                new InjectionConstructor(
                    new ResolvedParameter<IAutoMessageConsumerObjectCreator>(),
                    new ResolvedParameter<IObjectReaderMessagePublisher>()
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
