namespace ImportXML
{
    public class Patient
    {
        
        
        public string id  { get; set; }

        
        
        public string name { get; set; } // Ten
        public string surname { get; set; } // Ho va ten lot

        
        public int sex { get; set; }

        
        public int birthyear { get; set; }
        public string birthdate { get; set; }

        
        public string mobile { get; set; }
        public string social_id { get; set; } // So CMND, passport, Can cuoc cong dan
        
        public Country country { get; set; }
        
        public string country_code { get; set; } // Ma QUoc Gia: VN hoac VIE
        
        public City city { get; set; }
        
        public string city_id { get; set; } // ID Dia chi Thanh pho
        
        public District district { get; set; }

        
        public string district_id { get; set; } // ID Dia chi Quan
        
        public Ward ward { get; set; }

        
        public string ward_id { get; set; } // ID Dia chi Phuong
        public string address { get; set; } // So nha, Toa nha va ten duong
        public string bhyt_code { get; set; } // Ma so the BHYT 

       
    }
}
