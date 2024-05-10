namespace PlaceApi.Web.Endpoints.Authentication;

public static class Routes
{
    public static class Register
    {
        public static string Endpoint => "/register";
        public static string Name => "Register";

        public static class OpenApi
        {
            public static string Summary => "Register new user";
            public static string Description =>
                "Register new user with username, a valid email and password. The password should have a least 8 characters with one capital letter, on number and one special character";
        }
    }

    public static class ConfirmEmail
    {
        public static string Endpoint => "/confirmEmail";
        public static string Name => "ConfirmEmail";

        public static class OpenApi
        {
            public static string Summary => "Confirm user email";
            public static string Description =>
                "Confirm user email to validate and activate her account.";
        }
    }
}
