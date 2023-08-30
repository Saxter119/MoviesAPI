namespace MoviesAPI.DTOs.AccountsDTOs
{
    public class UserToken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
