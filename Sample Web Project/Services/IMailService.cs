using Sample_Web_Project.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample_Web_Project.Services
{
    public interface IMailService
    {
        Task SendMailAsync(MailRequest mailRequest);
    }
}
