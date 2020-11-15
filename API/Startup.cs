using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Linq;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            DataContext.DbPath = @"C:\Users\ethan\api.db3";
            using (DataContext db = new())
                db.Database.Migrate();

            ClientModels.SignedMessage.X_KeyLookup = (string fingerprint) =>
            {
                using DataContext db = new();
                if (db.Users.FirstOrDefault(x => x.ExchangeFingerprint == fingerprint) is Models.User user && user != null)
                    return CryptoService.ImportKey(user.ExchangePem);
                return null;
            };
            ClientModels.SignedMessage.X_UserLookup = (string fingerprint) =>
            {
                using DataContext db = new();
                if (db.Users.FirstOrDefault(x => x.ExchangeFingerprint == fingerprint) is Models.User user && user != null)
                    return user.Id;
                return null;
            };

            goto nogen;
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair key = CryptoService.GenerateKeypair(512);
            string keyPem = CryptoService.ExportKeypair(key, CryptoService.KeyType.Public);
            string keyPri = CryptoService.ExportKeypair(key, CryptoService.KeyType.Private);
            var dbg = 5;
            nogen:
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }
#if !DEBUG
            app.UseHttpsRedirection();
#endif
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
