using MinimalAPIPeliculas.Utilities;

namespace MinimalAPIPeliculas.DTOs
{
    public class PaginacionDTO
    {
        private const int paginaValorIncial = 1;
        private const int recordsPorPaginaValorInicial = 10;
        public int Pagina { get; set; } = paginaValorIncial;
        private int recordsPorPagina = recordsPorPaginaValorInicial;
        private readonly int cantidadMaximaRecordsPorPagina = 50;
        public int RecordsPorPagina
        {
            get { return recordsPorPagina; }
            set { recordsPorPagina = (value > cantidadMaximaRecordsPorPagina) ? cantidadMaximaRecordsPorPagina : value;  }
        }

        public static ValueTask<PaginacionDTO> BindAsync(HttpContext context)
        {
            var pagina = context.ExtraerValorODefecto(nameof(Pagina), paginaValorIncial);
            var recordsPorPagina = context.ExtraerValorODefecto(nameof(RecordsPorPagina),
                recordsPorPaginaValorInicial);

            var resultado = new PaginacionDTO
            {
                Pagina = pagina,
                RecordsPorPagina = recordsPorPagina
            };

            return ValueTask.FromResult(resultado);

        }
    }
}
