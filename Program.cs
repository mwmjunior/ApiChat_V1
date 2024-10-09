using WebChat.DataService;
using WebChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Problema de CORS
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("reactApp", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

// Adicionar InMemoryDb ao container de serviços antes de construir o app
builder.Services.AddSingleton<InMemoryDb>();

// SignalR suporte
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Mapear controladores da API
app.MapControllers();

// Mapear hubs do SignalR
app.MapHub<ChatHub>("/Chat");

app.UseCors("reactApp");

app.Run();
