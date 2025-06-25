using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validator
{
    public static class FileUploadValidator
    {
        public static bool HasImageExtension(this string? source)
        {
            if (source == null)
            {
                return false;
            }

            return (source.EndsWith(".png")
                || source.EndsWith(".jpg")
                || source.EndsWith(".jpeg")
                || source.EndsWith(".jpeg 2000")
                || source.EndsWith(".pdf"));
        }
    }
}
