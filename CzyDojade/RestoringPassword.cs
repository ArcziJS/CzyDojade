using Android.App;
using Android.OS;
using Android.Widget;
using MySqlConnector;
using System;
using System.Net;
using System.Net.Mail;
using Xamarin.Essentials;

#pragma warning disable CS0618

namespace CzyDojade
{
    [Activity(Label = "RestoringPassword")]
    public class RestorePassword : Activity
    {
        EditText EmailEntry;
        Button SendButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.restoring_password);

            string connectionString = "Server=db4free.net;Port=3306;Database=czy_dojade;Uid=czy_dojade;Pwd=czy_dojade;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                SendButton = FindViewById<Button>(Resource.Id.SendButton);
                EmailEntry = FindViewById<EditText>(Resource.Id.EmailEntry);

                SendButton.Click += ResetPasswordButton_Clicked;
            }
        }

        private void ResetPasswordButton_Clicked(object sender, EventArgs e)
        {
            if (!IsValidEmail(EmailEntry.Text))
            {
                Toast.MakeText(this, "Podaj poprawny adres e-mail.", ToastLength.Short).Show();
                return;
            }

            string resetToken = GenerateResetToken();
            SaveResetToken(resetToken);
            SendResetEmail(EmailEntry.Text, resetToken);

            Toast.MakeText(this, "Wiadomość e-mail z linkiem resetującym hasło została wysłana.", ToastLength.Short).Show();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateResetToken()
        {
            string resetToken = Guid.NewGuid().ToString();
            return resetToken;
        }

        private void SaveResetToken(string resetToken)
        {
            // Zapisz resetToken w lokalnej bazie danych, na przykład w SharedPreferences lub App Properties
            Preferences.Set("ResetToken", resetToken);
        }

        private void SendResetEmail(string emailAddress, string resetToken)
        {
            // Wysyłanie wiadomości e-mail z linkiem resetującym hasło
            // Możesz skorzystać z usługi zewnętrznej do wysyłania wiadomości e-mail lub użyć wbudowanych możliwości Xamarin

            string smtpServer = "smtp.poczta.onet.pl"; // Serwer SMTP używany do wysyłania wiadomości e-mail
            string smtpUsername = "czydojade"; // Nazwa użytkownika SMTP
            string smtpPassword = "CzyDojade123!"; // Hasło SMTP

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient(smtpServer);

                mail.From = new MailAddress(smtpUsername);
                mail.To.Add(emailAddress);
                mail.Subject = "Resetowanie hasła";
                mail.Body = $"Kliknij poniższy link, aby zresetować hasło: {resetToken}";

                smtpClient.Port = 465; // Port SMTP
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true; // Włącz SSL, jeśli wymagane

                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Wystąpił błąd podczas wysyłania wiadomości e-mail: " + ex.Message, ToastLength.Short).Show();
            }
        }
    }
}
