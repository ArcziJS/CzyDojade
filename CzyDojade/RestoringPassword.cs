using System;
using Android.App;
using Android.Widget;
using Android.OS;
using System.Net.Mail;
using System.Net;
using Android.Preferences;
using MySqlConnector;
using System.Runtime.InteropServices;

namespace CzyDojade
{
    [Activity(Label = "RestoringPassword")]
    public class RestorePassword
    {

        protected void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.restoring_password);

            string connectionString = "Server=db4free.net;Port=3306;Database=czy_dojade;Uid=czy_dojade;Pwd=czy_dojade;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                Button SendButton = FindViewById<Button>(Resource.Id.SendButton);
                EditText EmailEntry = FindViewById<EditText>(Resource.Id.EmailEntry);

                // Dodaj tutaj logikę obsługującą zdarzenie kliknięcia przycisku SendButton

            }
        }

        private void SendButton_Clicked(object sender, EventArgs e)
            {
                string email = EmailEntry.Text; // Pobierz adres e-mail z pola wprowadzania

                // Sprawdź, czy podano poprawny adres e-mail
                if (!IsValidEmail(email))
                {
                    DisplayAlert("Błąd", "Podaj poprawny adres e-mail.", "OK");
                    return;
                }

                // Wygeneruj nowe hasło dla użytkownika (możesz użyć dowolnego mechanizmu do generowania hasła)

                string newPassword = GenerateNewPassword();

                // Wyślij e-mail z nowym hasłem do użytkownika

                string smtpServer = "smtp.poczta.onet.pll"; // Serwer SMTP używany do wysyłania wiadomości e-mail
                string smtpUsername = "czydojade"; // Nazwa użytkownika SMTP
                string smtpPassword = "CzyDojade123!"; // Hasło SMTP

                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient smtpClient = new SmtpClient(smtpServer);

                    mail.From = new MailAddress(smtpUsername);
                    mail.To.Add(email);
                    mail.Subject = "Przypomnienie hasła";
                    mail.Body = "Twoje nowe hasło: " + newPassword;

                    smtpClient.Port = 587; // Port SMTP
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = true; // Włącz SSL, jeśli wymagane

                    smtpClient.Send(mail);

                    DisplayAlert("Sukces", "E-mail z nowym hasłem został wysłany.", "OK");
                }
                catch (Exception ex)
                {
                    DisplayAlert("Błąd", "Wystąpił błąd podczas wysyłania e-maila: " + ex.Message, "OK");
                }
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


            private string GenerateNewPassword()
            {
                // Implementacja generowania nowego hasła
                // Możesz użyć np. biblioteki Random lub innych mechanizmów do generowania losowych ciągów znaków
                return "newpassword123";
            }
        }
    }




