namespace MoviesAPI.DTOs
{
    public class MovieFilterDTO
    {
        public int PageRecords { get; set; } = 10;
        public int Page { get; set; } = 1;
        public PaginationDTO Paginate
        {
            get { return new PaginationDTO() { Page = Page, PageRecords = PageRecords }; }
        }//el get es el valor por defecto, el
         //set es el valor que se le asignara cuando se use
         //la propiedad. En este ejemplo estoy devolviendole un
         //valor con los valores de mis propiedades de
         //clase sustituyendo las que tiene utilizando el set.
         //aqui el set pues no quiero que le asigne un valor desde
         //los parametros.

        public string Name { get; set; }
        public bool OnCinemas{ get; set; }
        public bool NextMovies { get; set; }
        public int GenderId { get; set; }
        public string OrderField { get; set; }
        public bool OrderAsc { get; set; } = true;
    }
}
