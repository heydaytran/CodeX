using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

public static class ValidatePhoneNumber
{
    public static bool IsValidPhoneNumber(string input)
    {
        var phoneUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();

        try
        {
            var parsed = phoneUtil.Parse(input, null);  // Parse the input string into a standardized phone number object
            return phoneUtil.IsValidNumber(parsed); // Check if the parsed phone number is valid according to international rules
        }
        catch
        {
            return false; // if parsing fails ( lack of (+) , contain string character)--> return false
        }
    }
}

