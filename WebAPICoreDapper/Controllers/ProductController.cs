using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAPICoreDapper.Dtos;
using WebAPICoreDapper.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPICoreDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string _connectionStringOracle;
        public ProductController(IConfiguration configuration, IConfiguration configurationOracle)
        {
            _connectionString = configuration.GetConnectionString("SqlDBConnectionString");
            _connectionStringOracle = configurationOracle.GetConnectionString("OracleDbConnectionString");
        }
        // GET: api/<ProductController>
        [HttpGet]
        public async Task<IEnumerable<Product>> Get()
        {
            //using (var conn = new SqlConnection(_connectionStringOracle))
            //{
            //    if (conn.State == System.Data.ConnectionState.Closed)
            //        conn.Open();
            //    var result = await conn.QueryAsync<Donvi>("select donvi_id from linhnvu.ds_9ttvt", null, null, null, System.Data.CommandType.Text);
            //    return result;
            //}
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var result = await conn.QueryAsync<Product>("Get_Product_All", null, null, null, System.Data.CommandType.StoredProcedure);
                return result;
            }
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<Product> Get(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameter = new DynamicParameters();
                parameter.Add("@id",id);
                var result = await conn.QueryAsync<Product>("Get_Product_ById", parameter, null, null, System.Data.CommandType.StoredProcedure);
                return result.Single();
            }
        }

        // GET api/paging
        [HttpGet("paging")]
        public async Task<PagedResult> GetPaging(string keyword, int categoryId, int pageIndex, int pageSize)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                var parameter = new DynamicParameters();
                parameter.Add("@keyword", keyword);
                parameter.Add("@categoryId", categoryId);
                parameter.Add("@pageIndex", pageIndex);
                parameter.Add("@pageSize", pageSize);
                parameter.Add("@totalRow", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                var result = await conn.QueryAsync<Product>("Get_Product_AllPaging", parameter, null, null, System.Data.CommandType.StoredProcedure);
                
                int totaRow = parameter.Get<int>("@totalRow");

                var PagedResult =  new PagedResult()
                {
                    Items = result.ToList(),
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRow = totaRow
                };

                return PagedResult;
            }
        }
        // POST api/<ProductController>
        [HttpPost]
        public async Task<int> Post([FromBody] Product product)
        {
            int newId = 0;
            using (var conn = new SqlConnection(_connectionString))
            {                
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@name",product.Name);
                parameters.Add("@sku", product.Sku);
                parameters.Add("@price", product.Price);
                parameters.Add("@imageurl", product.ImageUrl);
                parameters.Add("@isactive", product.IsActive);
                parameters.Add("@id", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);
                var result = await conn.ExecuteAsync("Create_Product", parameters, null, null, System.Data.CommandType.StoredProcedure);

                newId = parameters.Get<int>("@id");
            }
            return newId;
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public async void Put(int id, [FromBody] Product product)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                parameters.Add("@name", product.Name);
                parameters.Add("@sku", product.Sku);
                parameters.Add("@price", product.Price);
                parameters.Add("@imageurl", product.ImageUrl);
                parameters.Add("@isactive", product.IsActive);
                await conn.ExecuteAsync("Update_Product", parameters, null, null, System.Data.CommandType.StoredProcedure);

            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async void Delete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id);
                await conn.ExecuteAsync("Delete_Product_ById", parameters, null, null, System.Data.CommandType.StoredProcedure);
            }
        }
    }
}
