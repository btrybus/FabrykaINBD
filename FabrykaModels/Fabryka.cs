using System.ComponentModel.DataAnnotations;

namespace FabrykaModels
{
    public class Hala
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public required string Nazwa { get; set; }

        [MaxLength(200)]
        public string? Adres { get; set; }

        public ICollection<Maszyna>? Maszyny { get; set; }

    }

    public class Maszyna
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public required string Nazwa { get; set; }

        public System.DateTime? DataUruchomienia { get; set; }

        public int HalaId { get; set; }
        public Hala? Hala { get; set; }

        public ICollection<Operator>? Operatorzy { get; set; }


    }

    public class Operator
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public required string Nazwisko { get; set; }

        [MaxLength(50)]
        public required string Imie { get; set; }

        public double? Placa { get; set; }

        public ICollection<Maszyna>? Maszyny { get; set; }

    }
}
