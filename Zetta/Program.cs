using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Zetta.Datos;

/*
 * Este m?todo inicializa una nueva instancia de la WebApplicationBuilder clase con valores predeterminados preconfigurados.
 * Se utiliza para configurar y construir una aplicaci?n web ASP.NET Core MVC. args es un par?metro opcional que se utiliza
 * para pasar argumentos de l?nea de comandos a la aplicaci?n.
 */
var builder = WebApplication.CreateBuilder(args);
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

/*
 * Esta extensi?n se utiliza para registrar los controladores y las vistas en una aplicaci?n web ASP.NET Core MVC.
 * Los controladores procesan las solicitudes HTTP y las vistas generan la salida HTML que se env?a al cliente.
 * Esta extensi?n tambi?n configura la integraci?n del marco de trabajo MVC con otras caracter?sticas de la plataforma
 * ASP.NET Core, como la inyecci?n de dependencias.
 */
// Agregar servicios al contenedor.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(
                                builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders().AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

/*
 * Esta extensi?n se utiliza para determinar si la aplicaci?n se est? ejecutando en un entorno de desarrollo o no.
 * Esto puede ser ?til para habilitar caracter?sticas espec?ficas de la depuraci?n o la visualizaci?n de mensajes de error
 * detallados solo en entornos de desarrollo.
 */
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    /*
     * Esta extensi?n se utiliza para manejar las excepciones que se producen durante la ejecuci?n de una aplicaci?n web
     * ASP.NET Core MVC. En caso de que se produzca una excepci?n, esta extensi?n redirigir? la solicitud a la ruta
     * especificada, en este caso, "/Home/Error", que corresponde al controlador "Home" y al m?todo "Error".
     */
    app.UseExceptionHandler("/Home/Error");

    /*
        *  Esta extensi?n se utiliza para habilitar la pol?tica de seguridad de transporte estricto (HSTS) en una aplicaci?n
        *  web ASP.NET Core MVC. HSTS es una pol?tica de seguridad que obliga a los navegadores web a acceder a la aplicaci?n
        *  solo a trav?s de una conexi?n HTTPS segura. Esto ayuda a proteger la privacidad y la integridad de la informaci?n
        *  que se transmite entre el cliente y el servidor.
        */
    // El valor predeterminado de HSTS es de 30 d?as. Es posible que desee cambiar esto para escenarios de producci?n, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

/*
 * Esta extensi?n se utiliza para redirigir todas las solicitudes HTTP a HTTPS en una aplicaci?n web ASP.NET Core MVC.
 * En otras palabras, si un usuario ingresa a una URL utilizando HTTP, esta extensi?n redirigir? autom?ticamente la
 * solicitud a la versi?n HTTPS de la misma URL.
 */
app.UseHttpsRedirection();

/*
 * Esta extensi?n se utiliza para habilitar el acceso a archivos est?ticos, como im?genes, hojas de estilo y archivos 
 * JavaScript, en una aplicaci?n web ASP.NET Core MVC. Permite que la aplicaci?n sirva estos archivos al cliente sin 
 * procesamiento adicional.
 */
app.UseStaticFiles();

/*
 * Esta extensi?n se utiliza para habilitar la funcionalidad de enrutamiento en una aplicaci?n web ASP.NET Core MVC.
 * El enrutamiento es un mecanismo que permite que la aplicaci?n responda a diferentes solicitudes HTTP en funci?n de
 * la ruta o URL solicitada.
 */
app.UseRouting();

/*
 * Esta extensi?n se utiliza para habilitar la autenticaci?n y la autorizaci?n en una aplicaci?n web ASP.NET Core MVC.
 * Permite que la aplicaci?n proteja sus recursos y funcionalidades mediante la autenticaci?n y la autorizaci?n de los
 * usuarios que acceden a ella.
 */
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();


/*
 *  Esta extensi?n se utiliza para definir las rutas de controlador en una aplicaci?n web ASP.NET Core MVC. Define c?mo
 *  se deben asignar las solicitudes HTTP a los m?todos del controlador correspondiente. En la l?nea de c?digo proporcionada,
 *  la ruta predeterminada se define como "{controller=Home}/{action=Index}/{id?}", lo que significa que si la ruta no se 
 *  especifica, la solicitud se enviar? al controlador "Home" y se llamar? al m?todo "Index" con un par?metro opcional "id".
 */
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();