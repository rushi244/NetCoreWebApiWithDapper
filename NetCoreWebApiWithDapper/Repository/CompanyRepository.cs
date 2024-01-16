using Dapper;
using NetCoreWebApiWithDapper.Contex;
using NetCoreWebApiWithDapper.Contracts;
using NetCoreWebApiWithDapper.Dto;
using NetCoreWebApiWithDapper.Entities;
using System.Data;

namespace NetCoreWebApiWithDapper.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext _dapperContext;

        public CompanyRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<Company> CreateCompany(CompanyDto company)
        {
            var query = "INSERT INTO Companies(Name, Address, Country) VALUES (@Name, @Address, @Country)" +
                "SELECT CAST(SCOPE_IDENTITY() AS int)";

            var parameters = new DynamicParameters();
            parameters.Add("@Name", company.Name, DbType.String);
            parameters.Add("@Address", company.Address, DbType.String);
            parameters.Add("@Country", company.Country, DbType.String);
            using (var connection = _dapperContext.CreateConnection())
            {
                // await connection.ExecuteAsync(query, parameters);
                var id = await connection.QuerySingleAsync<int>(query, parameters);
                var createdCompany = new Company
                {
                    Id = id,
                    Name = company.Name,
                    Address = company.Address,
                    Country = company.Country
                };
                return createdCompany;
            }
        }

        public async Task CreateMultipleCompanies(List<CompanyDto> companies)
        {
            
                var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country)";

                using (var connection = _dapperContext.CreateConnection())
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (var company in companies)
                        {
                            var parameters = new DynamicParameters();

                            parameters.Add("@Name", company.Name, DbType.String);
                            parameters.Add("@Address", company.Address, DbType.String);
                            parameters.Add("@Country", company.Country, DbType.String);

                            await connection.ExecuteAsync(query, parameters, transaction: transaction);
                        }
                        transaction.Commit();
                    }
                }
            
        }

        public async Task DeleteCompany(int id)
        {
            var query = "DELETE FROM Companies WHERE Id=@Id";

            using (var connection = _dapperContext.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
        }

        public async Task<IEnumerable<Company>> GetCompanies()
        {
            var query = "SELECT Id,Name,Address,Country FROM Companies";

            using (var connection = _dapperContext.CreateConnection())
            {
                var companies = await connection.QueryAsync<Company>(query);
                return companies.ToList();
            }
        }

        public async Task<Company> GetCompany(int id)
        {
            var query = "SELECT Id,Name,Address,Country FROM Companies WHERE Id=@Id";

            using (var connection = _dapperContext.CreateConnection())
            {
                var company = await connection.QuerySingleOrDefaultAsync<Company>(query, new { id });
                return company;
            }
        }

        public async Task<Company> GetCompanyByEmpId(int id)
        {
            var procedurName = "ShowCompanyByEmpId";
            var parameters = new DynamicParameters();

            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);

            using (var connection = _dapperContext.CreateConnection())
            {
                var company = await connection.QueryFirstOrDefaultAsync<Company>
                     (procedurName, parameters, commandType: CommandType.StoredProcedure);
                return company;

            }
        }

        public async Task<Company> GetMultipleResults(int id)
        {
            var query = "SELECT * FROM Companies WHERE Id=@Id;" +
                "SELECT * FROM Employees WHERE CompanyId=@Id";

            using (var connection = _dapperContext.CreateConnection())
            using (var multi = await connection.QueryMultipleAsync(query, new { id }))
            {
                var company = await multi.ReadSingleOrDefaultAsync<Company>();
                if (company is not null)
                {
                    company.Employee = (await multi.ReadAsync<Employee>()).ToList();

                }
                return company;
            }
        }

        public async Task<List<Company>> MultipleMapping()
        {
            var query = "SELECT * FROM Companies c JOIN Employees e ON e.CompanyId=c.ID";

            using (var connection = _dapperContext.CreateConnection())
            {
                var companyDict = new Dictionary<int, Company>();
                var companies = await connection.QueryAsync<Company, Employee, Company>(
                    query, (company, employee) =>
                    {
                        if (!companyDict.TryGetValue(company.Id, out var currentCompany))
                        {
                            currentCompany = company;
                            companyDict.Add(currentCompany.Id, currentCompany);
                        }
                        currentCompany.Employee.Add(employee);
                        return currentCompany;
                    }
                    );
                return companies.Distinct().ToList();
            }
        }

        public async Task UpdateCompany(int id, CompanyDto company)
        {
            var query = "UPDATE Companies SET Name=@Name,Address=@Address,Country=@Country WHERE Id=@Id";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);
            parameters.Add("@Name", company.Name, DbType.String);
            parameters.Add("@Address", company.Address, DbType.String);
            parameters.Add("@Country", company.Country, DbType.String);

            using (var connection = _dapperContext.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }

        }
 
    }
}
