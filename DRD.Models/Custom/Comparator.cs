namespace DRD.Models.Custom
{
    public class Comparator
    {
        public string value1;
        public string value2;
        public decimal number1;
        public decimal number2;

        public Comparator()
        {
        }
        public Comparator(string val1, string val2)
        {
            value1 = val1;
            value2 = val2;
        }
        public Comparator(decimal val1, decimal val2)
        {
            number1 = val1;
            number2 = val2;
        }
    }
}
