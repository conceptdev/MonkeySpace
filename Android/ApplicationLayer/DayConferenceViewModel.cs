using Android.Runtime;

namespace MonkeySpace.Core
{
    [Preserve]
    public class DayConferenceViewModel
    {
        public int SortOrder { get; set; }
        public string Section { get; set; }
        public string Day { get; set; }
        public string ConfCode { get; set; }
        public string SessCode { get; set; }

        private string lineOne;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        public string LineOne
        {
            get
            {
                return lineOne;
            }
            set
            {
                if (value != lineOne)
                {
                    lineOne = value;
                }
            }
        }

        private string lineTwo;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        public string LineTwo
        {
            get
            {
                return lineTwo;
            }
            set
            {
                if (value != lineTwo)
                {
                    lineTwo = value;
                }
            }
        }
    }
}