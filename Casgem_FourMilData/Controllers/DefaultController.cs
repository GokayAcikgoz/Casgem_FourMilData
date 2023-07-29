using Casgem_FourMilData.DAL.DTOs;
using Casgem_FourMilData.DAL.Entities;
using Casgem_FourMilData.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Casgem_FourMilData.Controllers
{
    public class DefaultController : Controller
    {
        private readonly string _connectionString = "Server = (localdb)\\MSSQLLocalDB; initial catalog = CARPLATES; integrated security = true";

        public async Task<IActionResult> Index()
        {
            await using var connection = new SqlConnection(_connectionString);

            var brandMax = (await connection.QueryAsync<BrandResullt>("SELECT TOP 1 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count DESC")).FirstOrDefault();
            var brandMin = (await connection.QueryAsync<BrandResullt>("SELECT TOP 1 BRAND, COUNT(*) AS count FROM PLATES GROUP BY BRAND ORDER BY count ASC")).FirstOrDefault();

            var plateMax = (await connection.QueryAsync<PlateResult>("SELECT TOP 1 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count DESC")).FirstOrDefault();
            var plateMin = (await connection.QueryAsync<PlateResult>("SELECT TOP 1 SUBSTRING(PLATE, 1, 2) AS plate, COUNT(*) AS count FROM PLATES GROUP BY SUBSTRING(PLATE, 1, 2) ORDER BY count ASC")).FirstOrDefault();

            var shiftType = (await connection.QueryAsync<ShiftTypeResult>("SELECT TOP 1 SHIFTTYPE, COUNT(*) AS count FROM PLATES GROUP BY SHIFTTYPE ORDER BY count DESC")).FirstOrDefault();

            var fuelType = (await connection.QueryAsync<FuelResult>("SELECT TOP 1 FUEL, COUNT(*) AS count FROM PLATES GROUP BY FUEL ORDER BY count DESC")).FirstOrDefault();

            ViewData["brandMax"] = brandMax.BRAND;
            ViewData["countMax"] = brandMax.COUNT;

            ViewData["brandMin"] = brandMin.BRAND;
            ViewData["countMin"] = brandMin.COUNT;

            ViewData["plateMax"] = plateMax.PLATE;
            ViewData["countMax"] = plateMax.COUNT;

            ViewData["plateMin"] = plateMin.PLATE;
            ViewData["countMin"] = plateMin.COUNT;

            ViewData["shiftType"] = shiftType.SHIFTTYPE;
            ViewData["shiftTypeCount"] = shiftType.COUNT;

            ViewData["fuelType"] = fuelType.FUEL;
            ViewData["fuelTypeCount"] = fuelType.COUNT;
            return View();
        }



        public async Task<IActionResult> Search(string keyword)
        {
            
            string query = @"
            SELECT TOP 10000 BRAND, SUBSTRING(PLATE, 1, 2) AS PlatePrefix, SHIFTTYPE, FUEL
            FROM PLATES
            WHERE BRAND LIKE '%' + @Keyword + '%'
               OR PLATE LIKE '%' + @Keyword + '%'
               OR SHIFTTYPE LIKE '%' + @Keyword + '%'
               OR FUEL LIKE '%' + @Keyword + '%'
        ";

            await using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // Sorguyu çalıştırın ve sonuçları alın
            var searchResults = await connection.QueryAsync<SearchResult>(query, new { Keyword = keyword });

            // Sonuçları JSON formatında döndürün
            return Json(searchResults);

        }

    }
}
