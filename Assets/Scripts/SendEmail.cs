using UnityEngine;
using UnityEngine.UI;
using System.Net.Mail;
using System.Collections;  
public class SendEmail : MonoBehaviour
{
    public GameObject CanvaEmail;
    public GameObject ErrorPanel;
    public GameObject SuccessPanel;
    public InputField nombreField;
    public InputField emailField;
    public InputField messageField;

    public void Enviar()
    {
        try
        {
            string nombre = nombreField.text;
            string email = emailField.text;
            string asunto = emailField.text;

            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("joe.chang.9696@gmail.com");
            mail.To.Add(email);
            mail.Subject = "More information about AR_Application";
            mail.Body = "Nombre: " + nombre + "\nEmail: " + email + "\nAsunto: " + asunto;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.Normal;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            string sCuentaCorreo = "joe.chang.9696@gmail.com";
            string sPassword = "yrxyswtjekdqqlee";
            smtp.Credentials = new System.Net.NetworkCredential(sCuentaCorreo, sPassword);

            smtp.Send(mail);

        }
        catch
        {
            ErrorPanel.SetActive(true);
            StartCoroutine(HideErrorPanelAfterDelay());
        }

        SuccessPanel.SetActive(true);
        StartCoroutine(HidesuccesPanelAfterDelay());
    }

    private IEnumerator HideErrorPanelAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        ErrorPanel.SetActive(false);
    }

    private IEnumerator HidesuccesPanelAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        SuccessPanel.SetActive(false); 
        CanvaEmail.SetActive(false);
    }
    public void btnEnviar()
    {
        Enviar();
    }
}
