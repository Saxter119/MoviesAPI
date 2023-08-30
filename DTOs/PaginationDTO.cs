namespace MoviesAPI.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;

        private int pageRecords { get; set; } = 10;

        private readonly int maxRecordForPage = 50;

        public int PageRecords
        {
            get => pageRecords;
            set
            { 
                pageRecords = (value > maxRecordForPage ? maxRecordForPage : value);
            }
        }
    }
}
