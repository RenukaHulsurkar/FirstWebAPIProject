#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace FirstWebAPIProject.Models
{
    public class Products
    {
        public int Id { get; set; }

        [Required]
        public string Sku { get; set; } = string.Empty;

        public string Name { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [JsonIgnore]
        public virtual Category? Category { get; set; }
    }

}

