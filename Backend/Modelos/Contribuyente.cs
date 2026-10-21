
namespace Backend.Modelos
{
    public class Contribuyente
    {
        public string Nit { get; set; }
        public string Nombre { get; set; }

        public Contribuyente(string nit, string nombre)
        {
            Nit = nit;
            Nombre = nombre;
        }
    }
}
