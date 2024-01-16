﻿namespace NetCoreWebApiWithDapper.Entities
{
    public class Company
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }

        public List<Employee> Employee { get; set; }=new List<Employee>();
    }
}
