using System;
using Maintenance.Grits.Entities;

namespace Maintenance.Grits.Utils
{
    public class Helper
    {
        public static string GetModeSymbol(GritsMode mode)
        {
            switch (mode)
            {
                case GritsMode.BBU:
                    return "B";
                case GritsMode.Enterprise:
                    return "E";
                case GritsMode.Both:
                    return "A";
                default: //case GritsMode.Disabled:
                    return "-";
            }
        }

        public static GritsMode GetModeEnum(string mode)
        {
            switch (mode)
            {
                case "B":
                    return GritsMode.BBU;
                case "E":
                    return GritsMode.Enterprise;
                case "A":
                    return GritsMode.Both;
                case "-":
                    return GritsMode.Disabled;
                default:
                    throw new InvalidOperationException("Invalid Grits Mode string: " + mode);
            }
        } 
    }
}