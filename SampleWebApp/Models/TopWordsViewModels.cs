using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SampleWebApp.Models
{
    public class TopWordsViewModels
    {        
        public string Word { get; set; }
        public int Weigth { get; set; }

        public string GetTagClass()
        {
            //Function to give different style
            int result = Weigth;
            while (result > 50) result = result / 50;
            if (result <= 1)
                return "tag1";
            if (result <= 4)
                return "tag2";
            if (result <= 8)
                return "tag3";
            if (result <= 12)
                return "tag4";
            if (result <= 18)
                return "tag5";
            if (result <= 30)
                return "tag6";
            return result <= 50 ? "tag7" : "";
        }

    }    
}
