using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreWebApiWithDapper.Contracts;
using NetCoreWebApiWithDapper.Dto;
using NetCoreWebApiWithDapper.Entities;

namespace NetCoreWebApiWithDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetCompanies() 
        {
        var companies =await _companyRepository.GetCompanies();
            return Ok(companies);
        
        }
        [HttpGet("{id}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var company=await _companyRepository.GetCompany(id);
            if (company is null)
            {
                return NotFound();
            }
            return Ok(company);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyDto company)
        {
            var createdCompany=await _companyRepository.CreateCompany(company);
            return CreatedAtRoute("CompanyById", new { id =createdCompany.Id},createdCompany);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyDto company)
        {
            var dbcompany=await _companyRepository.GetCompany(id);
            if(dbcompany is null)
                return NotFound();

            await _companyRepository.UpdateCompany(id, company);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var dbcompany = await _companyRepository.GetCompany(id);
            if (dbcompany is null)
                return NotFound();

            await _companyRepository.DeleteCompany(id);
            return NoContent();
        }

        [HttpGet("ByEmployeeId/{id}")]
        public async Task<IActionResult> GetCompanyByEmpId(int id)
        {
            var company = await _companyRepository.GetCompanyByEmpId(id);
            if (company is null)
            {
                return NotFound();
            }
            return Ok(company);
        }
        [HttpGet("{id}/MultipleResults")]
        public async  Task<IActionResult> GetMultipleResult(int id)
        {
            var company= await _companyRepository.GetMultipleResults(id);
            if (company is null)
                return NotFound();
            return Ok(company);
        }

        [HttpGet("MultipleMappings")]
        public async Task<IActionResult> GetMultipleMappings()
        {
            var companies = await _companyRepository.MultipleMapping();
            
            return Ok(companies);
        }
    }
}
