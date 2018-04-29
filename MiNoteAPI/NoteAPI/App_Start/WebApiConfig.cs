using DatabaseInterfaces;
using RabbitCore;
using RabbitMongoService;
using ServiceInterfaces;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using Unity;
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
            container.RegisterType<IObjectBroker, RabbitBroker>
                (new HierarchicalLifetimeManager());
            container.RegisterType<IMessageConsumer, RabbitConsumer>();
            container.RegisterType<IMessagePublisher, RabbitPublisher>();
            //(new InjectionConstructor("localhost", ""));
            //Must use default exchange "" if want to use queueName in routing key.
            //Otherwise must set up bindings for queues.
            container.RegisterType<IAutoMessageConsumerObjectCreator, RabbitAutoConsumerMongoCreator>();
            container.RegisterType<IObjectReaderMessagePublisher, MongoReaderRabbitPublisher>();

            // Set resolver
            config.DependencyResolver = new UnityResolver(container);

            // Edit config
            EditConfiguration(config);
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
