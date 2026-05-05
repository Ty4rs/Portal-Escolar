using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PortalEscolar.Data;
using PortalEscolar.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<PortalescolarContext>(options =>
    
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor(); //Dizendo que os meus Servicos podem acessar os meus cookies, para pegar o id do usuario logado, por exemplo.
builder.Services.AddScoped<IUsuarioService, UsuarioService>(); //Adicionando um servico que eu criei
builder.Services.AddScoped<IMateriaService, MateriaService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{ //Adicionando o Cookie como forma de autenticação
    options.LoginPath = "/Usuario/Login"; options.AccessDeniedPath = "/Painel";  //Redirecionando caso o usuario acesse uma página não permitida, por estar deslogado ou não ter acesso.
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Painel}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
