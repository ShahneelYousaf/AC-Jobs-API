using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_Jobs_API.Models
{
    public enum ApiStatusCode
    {
        // 100 series for failures
        RecordAlreadyExist = 100,
        RecordNotFound = 101,
        InvalidId = 102,
        SomethingWentWrong=103,
        ImageUploadError = 104,

        // 200 series for success
        RecordSavedSuccessfully = 200,
        RecordUpdatedSuccessfully = 201,
        RecordFoundSuccessfully = 202,
        RecordDeletedSuccessfully = 203,  
    }
}
