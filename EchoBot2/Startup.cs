using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using EchoBot2.Bots;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;

namespace EchoBot2
{
    public class Startup
    {
        private readonly IHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            services.AddSingleton<IStorage, MemoryStorage>();
            services.AddSingleton<UserState>();
            services.AddSingleton<ConversationState>();

            ComponentRegistration.Add(new DialogsComponentRegistration());
            ComponentRegistration.Add(new AdaptiveComponentRegistration());
            ComponentRegistration.Add(new DeclarativeComponentRegistration());
            ComponentRegistration.Add(new LanguageGenerationComponentRegistration());

            var resourceExplorer = new ResourceExplorer().LoadProject(_env.ContentRootPath);
            services.AddSingleton(resourceExplorer);

            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            services.AddTransient<IBot, EchoBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
