using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("Users")]  // Anotação para definir o nome da tabela no banco de dados
    public class User
    {
        [Key]  // Anotação para definir a chave primária
        public int Id { get; set; }

        [Required]  // Anotação para garantir que o campo seja obrigatório
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }
    }
}
