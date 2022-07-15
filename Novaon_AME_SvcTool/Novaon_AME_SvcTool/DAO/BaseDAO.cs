using Novaon_AME_SvcTool.Common;

namespace Novaon_AME_SvcTool.DAO
{
    public class BaseDAO
    {
        protected SqlUtilities sqlUtilities;

        protected BaseDAO()
        {
            sqlUtilities = new SqlUtilities();
        }
    }
}
