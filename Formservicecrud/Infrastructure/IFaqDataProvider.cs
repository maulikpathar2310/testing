using Innovagic.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using innovegic.Models.ViewModel;

namespace innovegic.Infrastructure.DataProvider
{
    interface IFaqDataProvider
    {
        string SaveFaqData(FaqViewModel cfd);
        FaqViewModel GetFaqSavedData();
        Faq GetFaqById(Guid FaqId);
        string EditFaq(Faq obj);
        Guid DeleteFaq(Guid Id);
    }
}
