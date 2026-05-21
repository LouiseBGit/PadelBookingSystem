
using Microsoft.EntityFrameworkCore;
using System;
using PadelBooking.Core.Data;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Repositories;
using PadelBooking.Core.Services;

namespace PadelBooking.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //databas, EntityFramework core 
            builder.Services.AddDbContext<AppDbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            //controllers, api
            builder.Services.AddControllers();

            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //dependency injection - när IBookingRepository efterfrågas skapas och skickas BookingRepository automatiskt
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

            //dependency injection - när ICustomerService efterfrågas skapas och skickas CustomerService automatiskt
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
