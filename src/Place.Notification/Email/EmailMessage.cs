using System;

namespace Place.Notification.Email;

public class EmailMessage
{
    public string To { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }

    public EmailMessage(string to, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            throw new ArgumentNullException(nameof(to));
        }
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentNullException(nameof(subject));
        }
        if (string.IsNullOrWhiteSpace(body))
        {
            throw new ArgumentNullException(nameof(body));
        }

        To = to;
        Subject = subject;
        Body = body;
    }
}
