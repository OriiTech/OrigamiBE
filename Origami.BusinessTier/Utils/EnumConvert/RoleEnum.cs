using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Utils.EnumConvert
{
    public enum RoleEnum
    {
        User = 1,
        Sensei = 2,
        Staff = 3
    }

    public static class RoleConstants
    {
        public const string User = "1";
        public const string Sensei = "2";
        public const string Staff = "3";
    }

    public static class RoleEnumExtensions
    {
        public static string ToRoleString(this RoleEnum role)
        {
            return ((int)role).ToString();
        }

        public static RoleEnum? ToRoleEnum(this string roleString)
        {
            if (int.TryParse(roleString, out int roleId))
            {
                return Enum.IsDefined(typeof(RoleEnum), roleId) ? (RoleEnum)roleId : null;
            }
            return null;
        }
    }
}
