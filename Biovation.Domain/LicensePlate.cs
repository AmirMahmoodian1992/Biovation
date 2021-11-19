using DataAccessLayerCore.Attributes;
using System;
using System.Text.RegularExpressions;

namespace Biovation.Domain
{
    public class LicensePlate
    {
        [Id]
        public int EntityId { get; set; }

        public string LicensePlateNumber
        {
            get => _licensePlateNumber;
            set
            {
                FormatPlate(value);
                _licensePlateNumber = value;
            }
        }

        private string _licensePlateNumber;
        public bool IsActive { get; set; }
        //public DateTime StartDate { get; set; }

        public int FirstPart { get; private set; }
        public string SecondPart { get; set; }
        public int ThirdPart { get; private set; }
        public int FourthPart { get; private set; }

        [OneToOne]
        public Vehicle Vehicle { get; set; }
        public DateTime StartDate { get; set; }


        public DateTime EndDate { get; set; }


        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }


        //public const string PlatePattern = @"[۰-۹][۰-۹][آ-ی][۰-۹][۰-۹][۰-۹][۰-۹][۰-۹]";

        private void FormatPlate(string licensePlateNumber)
        {
            try
            {
                //var regexDetect = Regex.Match(licensePlateNumber, PlatePattern);
                //if (!regexDetect.Success) return;
                //for (var i = 48; i < 58; i++)
                licensePlateNumber = ToEnglishNumber(licensePlateNumber);

                FirstPart = Convert.ToInt32(licensePlateNumber.Substring(6, 2));
                SecondPart = licensePlateNumber.Substring(2, 1);
                ThirdPart = Convert.ToInt32(licensePlateNumber.Substring(3, 3));
                FourthPart = Convert.ToInt32(licensePlateNumber.Substring(0, 2));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void FillLicensePlateNumber(int firstPart, string secondPart, int thirdPart, int fourthPart)
        {
            var licensePlateNumber = fourthPart + secondPart + thirdPart + firstPart;

            licensePlateNumber = ToEnglishNumber(licensePlateNumber);

            FirstPart = firstPart;
            SecondPart = secondPart;
            ThirdPart = thirdPart;
            FourthPart = fourthPart;

            LicensePlateNumber = licensePlateNumber;
        }
        public static string ToEnglishNumber(string input)
        {
            if (input == null) return "";
            string EnglishNumbers = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsDigit(input[i]))
                {
                    EnglishNumbers += char.GetNumericValue(input, i);
                }
                else
                {
                    EnglishNumbers += input[i].ToString();
                }
            }
            return EnglishNumbers;
        }
        //public bool ValidateLicensePlateFormat()
        //{
        //    var regexDetect = Regex.Match(LicensePlateNumber, PlatePattern);
        //    return regexDetect.Success;
        //}
    }
}
