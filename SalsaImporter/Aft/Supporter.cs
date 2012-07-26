using System;

namespace SalsaImporter.Aft
{
    public class Supporter
    {
        public int Id { get; set; }
        public int? supporter_KEY { get; set; }
        public DateTime? Last_Modified { get; set; }
        public DateTime? Date_Created { get; set; }
        public string Title { get; set; }
        public string First_Name { get; set; }
        public string MI { get; set; }
        public string Last_Name { get; set; }
        public string Suffix { get; set; }
        public string Email { get; set; }
        //public string Password { get; set; }
        public int? Receive_Email { get; set; } //tinyint	YES	1	
        public string Email_Preference { get; set; } //	HTML Email	
        public DateTime? Last_Bounce { get; set; }
        public int? Receive_Phone_Blasts { get; set; } //tinyint	
        public string Phone { get; set; }
        public string Cell_Phone { get; set; }
        public string Phone_Provider { get; set; }
        public string Work_Phone { get; set; }
        public string Pager { get; set; }
        public string Home_Fax { get; set; }
        public string Work_Fax { get; set; }
        public string Street { get; set; }
        public string Street_2 { get; set; }
        public string Street_3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string PRIVATE_Zip_Plus_4 { get; set; }
        public string County { get; set; }
        public string District { get; set; }
        public string Country { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string Organization { get; set; }
        public string Department { get; set; }
        public string Occupation { get; set; }
        public string Instant_Messenger_Service { get; set; }
        public string Instant_Messenger_Name { get; set; }
        public string Web_Page { get; set; }
        public string Alternative_Email { get; set; }
        public string Other_Data_1 { get; set; }
        public string Other_Data_2 { get; set; }
        public string Other_Data_3 { get; set; }
        public string Notes { get; set; }
        public string Source { get; set; }
        public string Source_Details { get; set; }
        public string Source_Tracking_Code { get; set; }
        public string Tracking_Code { get; set; }
        public string Status { get; set; }
        public string uid { get; set; }
        public string Timezone { get; set; }
        public string Language_Code { get; set; }
        public string CustomString0 { get; set; }
        public string CustomString1 { get; set; }
        public string CustomString2 { get; set; }
        public string CustomString3 { get; set; }
        public string CustomString4 { get; set; }
        public string CustomString5 { get; set; }
        public string CustomString6 { get; set; }
        public string CustomString7 { get; set; }
        public string CustomString8 { get; set; }
        public string CustomString9 { get; set; }
        public int? CustomInteger0 { get; set; }       
        public int? CustomInteger1 { get; set; }       
        public int? CustomInteger2 { get; set; }       
        public int? CustomInteger3 { get; set; }       
        public int? CustomInteger4 { get; set; }
        public bool? CustomBoolean0 { get; set; }
        public bool? CustomBoolean1 { get; set; }
        public bool? CustomBoolean2 { get; set; }
        public bool? CustomBoolean3 { get; set; }
        public bool? CustomBoolean4 { get; set; }
        public bool? CustomBoolean5 { get; set; }
        public bool? CustomBoolean6 { get; set; }
        public bool? CustomBoolean7 { get; set; }
        public bool? CustomBoolean8 { get; set; }
        public bool? CustomBoolean9 { get; set; }
    }
}