using Postgrest.Attributes;
using Postgrest.Models;
using System.Collections.Generic;


public class puntos : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("nombre_usuario")]
    public string NombreUsuario { get; set; }

    [Column("puntos")]
    public int Puntos { get; set; }

    /*[Column("trivia_id")]
    public int TriviaId { get; set; }*/
}
