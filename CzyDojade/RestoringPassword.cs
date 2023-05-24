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

                // Dodaj tutaj logikę obsługującą zdarzenie kliknięcia przycisku SendButton
                SendButton.Click += delegate
                {
                    string email = EmailEntry.Text; // Pobierz adres e-mail z pola wprowadzania

                    // Sprawdź, czy podano poprawny adres e-mail
                    if (!IsValidEmail(email))
                    {
                        Toast.MakeText(this, "Podaj poprawny adres e-mail.", ToastLength.Short).Show();
                        //DisplayAlert("Błąd", , "OK");
                        return;
                    }

                    // Wygeneruj nowe hasło dla użytkownika (możesz użyć dowolnego mechanizmu do generowania hasła)

                    string newPassword = GenerateNewPassword();

                    // Wyślij e-mail z nowym hasłem do użytkownika

                    string smtpServer = "smtp.poczta.onet.pl"; // Serwer SMTP używany do wysyłania wiadomości e-mail
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

                        Toast.MakeText(this, "E-mail z nowym hasłem został wysłany.", ToastLength.Short).Show();
                        //DisplayAlert("Sukces", , "OK");
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();

                        //DisplayAlert("Błąd", "Wystąpił błąd podczas wysyłania e-maila: " + ex.Message, "OK");
                    }
                };
                //SendButton_Clicked(object sender, EventArgs e)
                //{
                    
                //}
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




