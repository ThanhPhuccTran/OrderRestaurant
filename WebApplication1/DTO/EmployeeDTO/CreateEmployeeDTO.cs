﻿namespace OrderRestaurant.DTO.EmployeeDTO
{
    public class CreateEmployeeDTO
    {
        public string EmployeeName { get; set; }
        public string Phone { get; set; }
        public string? Image { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
