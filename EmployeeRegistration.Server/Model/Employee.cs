using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeeRegistration.Server.Model
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(30)]
        public string EmployeeName { get; set; } = string.Empty;

        [Required]
        public int Age { get; set; }

        [Required]
        [StringLength(10)]
        public string MobileNum { get; set; } = string.Empty;

        [StringLength(6)]
        public string Pincode { get; set; } = string.Empty;

        public DateTime? DOB { get; set; }

        [Required]
        [StringLength(250)]
        public string AddressLine1 { get; set; } = string.Empty;

        [StringLength(250)]
        public string? AddressLine2 { get; set; }

        [Required]
        public int StateId { get; set; }

        [Required]
        public int CountryId { get; set; }

        // Navigation properties
        [ForeignKey("StateId")]
        public virtual State? State { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country? Country { get; set; }
    }

    public class State
    {
        [Key]
        public int StateId { get; set; }

        [Required]
        [StringLength(100)]
        public string StateName { get; set; } = string.Empty;

        [Required]
        public int CountryId { get; set; }

        [ForeignKey("CountryId")]
        [JsonIgnore]
        public virtual Country? Country { get; set; }

        [JsonIgnore] // Make sure Employees are ignored to prevent cycle
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }

    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        [Required]
        [StringLength(100)]
        public string CountryName { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<State> States { get; set; } = new List<State>();

        [JsonIgnore]
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }

}
