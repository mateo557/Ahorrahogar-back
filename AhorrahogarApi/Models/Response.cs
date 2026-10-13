namespace AhorrahogarApi.Models
{
   
        public record Response<T>(bool Success, string Message, T? Data = default);
  
}
