using Microsoft.SqlServer.Management.Smo;

namespace StoredProceduresBackup.Dto
{
    public class UserDefinedFunctionWithType
    {
        public UserDefinedFunction Function { get; set; }
        public UserDefinedFunctionType UserDefinedFunctionType { get; set; }

        public UserDefinedFunctionWithType(UserDefinedFunction function, UserDefinedFunctionType type)
        {
            UserDefinedFunctionType = type;
            Function = function;
        }
    }
}