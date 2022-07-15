using DS.BusinessObject.API;
using System.Web.Http;

namespace Invoice.API.Controllers
{
    public class BaseApiController : ApiController
    {
        protected PartnerOAuthBO partnerObject = PartnerOAuthBO.Current.CurrentPartner();

        public BaseApiController()
        {

        }

        public void CalTaskNumber(int totalRow, ref int numberPerTask, ref int taskNumDefault)
        {
            if (totalRow <= taskNumDefault)
            {
                taskNumDefault = 1;
                numberPerTask = totalRow;
            }
            else
            {
                numberPerTask = totalRow / taskNumDefault;
                if (numberPerTask % taskNumDefault != 0)
                    numberPerTask++;
            }
        }
    }
}
