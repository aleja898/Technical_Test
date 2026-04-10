using TechnicalTest.Data;

namespace TechnicalTest.Backend.Data
{
    public class SeedDb
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;

        public SeedDb(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public async Task SeedAsync()
        {
            Console.WriteLine("=== INICIO DEL SEED DE BASE DE DATOS ===");
            

            var databaseExists = await _context.Database.CanConnectAsync();
            Console.WriteLine($"Base de datos existe: {databaseExists}");

            var resetDatabase = Environment.GetEnvironmentVariable("RESET_DB")?.ToLower() == "true";
            Console.WriteLine($"Variable RESET_DB: {Environment.GetEnvironmentVariable("RESET_DB")}");
            Console.WriteLine($"resetDatabase: {resetDatabase}");
            
            if (resetDatabase)
            {
                Console.WriteLine("RESET_DB=true - Eliminando base de datos...");

                // Esta linea BORRA POR COMPLETO LA BASE DE DATOS.
                // A menos que se vaya a usar, siempre hay que VERIFICAR que esta linea este COMENTADA.
                // DANGER ZONE /////////////////////////////////////
                //await _context.Database.EnsureDeletedAsync();
                Console.WriteLine("Base de datos eliminada (EnsureDeletedAsync)");
                
                databaseExists = false;
            }
            else
            {
                Console.WriteLine("RESET_DB=false o no definida - Manteniendo base de datos existente");
                Console.WriteLine("Para eliminar la base de datos, establece RESET_DB=true");
            }

            await _context.Database.EnsureCreatedAsync();
            Console.WriteLine("Base de datos y tablas creadas (EnsureCreatedAsync)");

            var entityTypes = _context.Model.GetEntityTypes().Select(t => t.ClrType.Name).ToList();
            Console.WriteLine($"Entidades configuradas: {string.Join(", ", entityTypes)}");

            Console.WriteLine("=== FIN DEL SEED DE BASE DE DATOS ===");
        }
    }
}
