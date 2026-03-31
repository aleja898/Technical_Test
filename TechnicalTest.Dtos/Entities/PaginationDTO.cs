using System;

namespace TechnicalTest.Dtos.Entities
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        public int RecordsNumber { get; set; } = 10;
        public string? Filter { get; set; }
    }
}
