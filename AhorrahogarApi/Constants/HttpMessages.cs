namespace AhorrahogarApi.Constants
{
    public class HttpMessages
    {
        public const string Ok = "Listo, proceso exitoso.";
        public const string Confirm = "Proceso realizado correctamente.";

        // Errores del cliente
        public const string BadRequest = "Revisa los datos ingresados e inténtalo de nuevo.";
        public const string Unauthorized = "Tu sesión expiró. Inicia sesión otra vez.";
        public const string Forbidden = "No tienes permiso para hacer esto.";
        public const string NotFound = "No encontramos lo que buscabas.";
        public const string Conflict = "Ese registro ya existe.";

        // Errores del servidor
        public const string InternalServerError = "Algo falló de nuestro lado. Inténtalo más tarde.";
        public const string ServiceUnavailable = "El servicio no está disponible por ahora.";

        public static string ForStatus(int status) => status switch
        {
            200 => Ok,
            201 => Confirm,
            400 => BadRequest,
            401 => Unauthorized,
            403 => Forbidden,
            404 => NotFound,
            409 => Conflict,
            500 => InternalServerError,
            503 => ServiceUnavailable,
            _ => "Ocurrió un error inesperado."
        };
    }
}
