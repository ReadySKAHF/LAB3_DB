using Microsoft.Extensions.Caching.Memory;
using Domain.Models;
using Domain.Data;
using System.Collections.Generic;
using System;

namespace Domain.Service
{
    public class CachedDataService
    {
        private readonly AutoRepairWorkshopContext _context;
        private readonly IMemoryCache _cache;
        private const int RowCount = 20;

        public CachedDataService(AutoRepairWorkshopContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        // Метод для получения автомобилей
        public IEnumerable<Car> GetCars()
        {
            if (!_cache.TryGetValue("Cars", out IEnumerable<Car> cars))
            {
                cars = _context.Cars.Take(RowCount).ToList();
                _cache.Set("Cars", cars, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }
            return cars;
        }

        // Метод для получения ремонтных заказов
        public IEnumerable<RepairOrder> GetRepairOrders()
        {
            if (!_cache.TryGetValue("RepairOrders", out IEnumerable<RepairOrder> repairOrders))
            {
                repairOrders = _context.RepairOrders.Take(RowCount).ToList();
                _cache.Set("RepairOrders", repairOrders, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }
            return repairOrders;
        }

        // Метод для получения механиков
        public IEnumerable<Mechanic> GetMechanics()
        {
            if (!_cache.TryGetValue("Mechanics", out IEnumerable<Mechanic> mechanics))
            {
                mechanics = _context.Mechanics.Take(RowCount).ToList();
                _cache.Set("Mechanics", mechanics, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }
            return mechanics;
        }

        // Метод для получения услуг
        public IEnumerable<Domain.Models.Service> GetServices()
        {
            if (!_cache.TryGetValue("Services", out IEnumerable<Domain.Models.Service> services))
            {
                services = _context.Services.Take(RowCount).ToList();
                _cache.Set("Services", services, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }
            return services;
        }

        // Метод для получения платежей
        public IEnumerable<Payment> GetPayments()
        {
            if (!_cache.TryGetValue("Payments", out IEnumerable<Payment> payments))
            {
                payments = _context.Payments.Take(RowCount).ToList();
                _cache.Set("Payments", payments, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }
            return payments;
        }

        // Метод для получения CarService
        public IEnumerable<CarService> GetCarServices()
        {
            if (!_cache.TryGetValue("CarServices", out IEnumerable<CarService> carServices))
            {
                carServices = _context.CarServices.Take(RowCount).ToList();
                _cache.Set("CarServices", carServices, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }
            return carServices;
        }

        // Метод для получения статусов автомобилей
        public IEnumerable<CarStatus> GetCarStatuses()
        {
            if (!_cache.TryGetValue("CarStatuses", out IEnumerable<CarStatus> carStatuses))
            {
                carStatuses = _context.CarStatuses.Take(RowCount).ToList();
                _cache.Set("CarStatuses", carStatuses, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }
            return carStatuses;
        }

        // Метод для получения владельцев
        public IEnumerable<Owner> GetOwners()
        {
            if (!_cache.TryGetValue("Owners", out IEnumerable<Owner> owners))
            {
                owners = _context.Owners.Take(RowCount).ToList();
                _cache.Set("Owners", owners, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
            }
            return owners;
        }
    }
}
