namespace PlaceApi.Web.Endpoints.Authentication;

public static class Routes
{
    public static class Register
    {
        public static string Endpoint => "/auth/register";
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
        public static string Endpoint => "/auth/confirm-email";
        public static string Name => "ConfirmEmail";

        public static class OpenApi
        {
            public static string Summary => "Confirm user email";
            public static string Description =>
                "Confirm user email to validate and activate her account.";
        }
    }

    public class Login
    {
        public static string Endpoint => "/auth/login";
        public static string Name => "Login";

        public static class OpenApi
        {
            public static string Summary => "Login user";
            public static string Description =>
                "Authenticate registered user with email and password and generate bearer token";
        }
    }

    public class ForgotPassword
    {
        public static string Endpoint => "/auth/forgot-password";
        public static string Name => "ForgotPassword";

        public static class OpenApi
        {
            public static string Summary => "Send unique token to reset password";
            public static string Description => "Send a token to a user to reset his password";
        }
    }

    public class ResetPassword
    {
        public static string Endpoint => "/auth/reset-password";
        public static string Name => "ResetPassword";

        public static class OpenApi
        {
            public static string Summary => "Reset user password";
            public static string Description => "Reset user password";
        }
    }

    public class ResendConfirmationEmail
    {
        public static string Endpoint => "/auth/resend-confirmation-email";
        public static string Name => "ResendConfirmationEmail";

        public static class OpenApi
        {
            public static string Summary => "Resend confirmation email";
            public static string Description =>
                "Manually resend confirmation email for registered user.";
        }
    }
}
