using System;
using System.ComponentModel.DataAnnotations;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Aft
{
    public class Supporter: ISyncObject
    {
        private string _uid;
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
        //public int? Receive_Email { get; set; } //tinyint	YES	1	
        public string Email_Preference { get; set; } //	HTML Email	
        public DateTime? Last_Bounce { get; set; }
        //public int? Receive_Phone_Blasts { get; set; } //tinyint	
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
        public DateTime? CustomDateTime0 { get; set; }

        public string uid
        {
            get
            { 
                if(_uid == null)
                {
                    _uid = Id.ToString();
                }
                return _uid;
            }
            set { _uid = value; }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Supporter) obj);
        }

        protected bool Equals(Supporter other)
        {
            return string.Equals(_uid, other._uid) && supporter_KEY == other.supporter_KEY && string.Equals(Title, other.Title)
                && string.Equals(First_Name, other.First_Name) && string.Equals(MI, other.MI) && string.Equals(Last_Name, other.Last_Name)
                && string.Equals(Suffix, other.Suffix) && string.Equals(Email, other.Email)
                && string.Equals(Email_Preference, other.Email_Preference) && string.Equals(Phone, other.Phone)
                && string.Equals(Cell_Phone, other.Cell_Phone) && string.Equals(Phone_Provider, other.Phone_Provider)
                && string.Equals(Work_Phone, other.Work_Phone) && string.Equals(Pager, other.Pager)
                && string.Equals(Home_Fax, other.Home_Fax) && string.Equals(Work_Fax, other.Work_Fax)
                && string.Equals(Street, other.Street) && string.Equals(Street_2, other.Street_2)
                && string.Equals(Street_3, other.Street_3) && string.Equals(City, other.City)
                && string.Equals(State, other.State) && string.Equals(Zip, other.Zip)
                && string.Equals(PRIVATE_Zip_Plus_4, other.PRIVATE_Zip_Plus_4) && string.Equals(County, other.County)
                && string.Equals(District, other.District) && string.Equals(Country, other.Country)
                && Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude)
                && string.Equals(Organization, other.Organization) && string.Equals(Department, other.Department)
                && string.Equals(Occupation, other.Occupation) && string.Equals(Instant_Messenger_Service, other.Instant_Messenger_Service)
                && string.Equals(Instant_Messenger_Name, other.Instant_Messenger_Name) && string.Equals(Web_Page, other.Web_Page)
                && string.Equals(Alternative_Email, other.Alternative_Email) && string.Equals(Other_Data_1, other.Other_Data_1)
                && string.Equals(Other_Data_2, other.Other_Data_2) && string.Equals(Other_Data_3, other.Other_Data_3)
                && string.Equals(Notes, other.Notes) && string.Equals(Source, other.Source)
                && string.Equals(Source_Details, other.Source_Details) && string.Equals(Source_Tracking_Code, other.Source_Tracking_Code)
                && string.Equals(Tracking_Code, other.Tracking_Code)
                && string.Equals(Status, other.Status) && string.Equals(Timezone, other.Timezone)
                && string.Equals(Language_Code, other.Language_Code) && string.Equals(CustomString0, other.CustomString0)
                && string.Equals(CustomString1, other.CustomString1) && string.Equals(CustomString2, other.CustomString2)
                && string.Equals(CustomString3, other.CustomString3) && string.Equals(CustomString4, other.CustomString4)
                && string.Equals(CustomString5, other.CustomString5) && string.Equals(CustomString6, other.CustomString6)
                && string.Equals(CustomString7, other.CustomString7) && string.Equals(CustomString8, other.CustomString8)
                && string.Equals(CustomString9, other.CustomString9) && CustomInteger0 == other.CustomInteger0
                && CustomInteger1 == other.CustomInteger1 && CustomInteger2 == other.CustomInteger2
                && CustomInteger3 == other.CustomInteger3 && CustomInteger4 == other.CustomInteger4
                && CustomBoolean0.Equals(other.CustomBoolean0) && CustomBoolean1.Equals(other.CustomBoolean1)
                && CustomBoolean2.Equals(other.CustomBoolean2) && CustomBoolean3.Equals(other.CustomBoolean3)
                && CustomBoolean4.Equals(other.CustomBoolean4) && CustomBoolean5.Equals(other.CustomBoolean5)
                && CustomBoolean6.Equals(other.CustomBoolean6) && CustomBoolean7.Equals(other.CustomBoolean7)
                && CustomBoolean8.Equals(other.CustomBoolean8) && CustomBoolean9.Equals(other.CustomBoolean9)
                && CustomDateTime0.Equals(other.CustomDateTime0);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (_uid != null ? _uid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ supporter_KEY.GetHashCode();
                hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (First_Name != null ? First_Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MI != null ? MI.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Last_Name != null ? Last_Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Suffix != null ? Suffix.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Email_Preference != null ? Email_Preference.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Phone != null ? Phone.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Cell_Phone != null ? Cell_Phone.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Phone_Provider != null ? Phone_Provider.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Work_Phone != null ? Work_Phone.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Pager != null ? Pager.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Home_Fax != null ? Home_Fax.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Work_Fax != null ? Work_Fax.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Street != null ? Street.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Street_2 != null ? Street_2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Street_3 != null ? Street_3.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (City != null ? City.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (State != null ? State.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Zip != null ? Zip.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PRIVATE_Zip_Plus_4 != null ? PRIVATE_Zip_Plus_4.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (County != null ? County.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (District != null ? District.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Country != null ? Country.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Latitude.GetHashCode();
                hashCode = (hashCode * 397) ^ Longitude.GetHashCode();
                hashCode = (hashCode * 397) ^ (Organization != null ? Organization.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Department != null ? Department.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Occupation != null ? Occupation.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Instant_Messenger_Service != null ? Instant_Messenger_Service.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Instant_Messenger_Name != null ? Instant_Messenger_Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Web_Page != null ? Web_Page.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Alternative_Email != null ? Alternative_Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Other_Data_1 != null ? Other_Data_1.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Other_Data_2 != null ? Other_Data_2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Other_Data_3 != null ? Other_Data_3.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Notes != null ? Notes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Source != null ? Source.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Source_Details != null ? Source_Details.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Source_Tracking_Code != null ? Source_Tracking_Code.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Tracking_Code != null ? Tracking_Code.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Status != null ? Status.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Timezone != null ? Timezone.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Language_Code != null ? Language_Code.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomString0 != null ? CustomString0.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomString1 != null ? CustomString1.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomString2 != null ? CustomString2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomString3 != null ? CustomString3.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomString4 != null ? CustomString4.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomString5 != null ? CustomString5.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomString6 != null ? CustomString6.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomString7 != null ? CustomString7.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomString8 != null ? CustomString8.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CustomString9 != null ? CustomString9.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ CustomInteger0.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomInteger1.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomInteger2.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomInteger3.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomInteger4.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomBoolean0.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomBoolean1.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomBoolean2.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomBoolean3.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomBoolean4.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomBoolean5.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomBoolean6.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomBoolean7.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomBoolean8.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomBoolean9.GetHashCode();
                hashCode = (hashCode * 397) ^ CustomDateTime0.GetHashCode();
                return hashCode;
            }
        }


        [NotMapped]
        public int LocalKey
        {
            get { return Id; }
            set { Id = value; }
        }

        [NotMapped]
        public int? ExternalKey
        {
            get{return supporter_KEY;}
            set { supporter_KEY = value; }
        }

        //Todo: define colunms in local db
        public DateTime? localModifiedDate { get; set; }

        [NotMapped]
        public DateTime? ExternalModifiedDate
        {
            get { return Last_Modified; }
            set { Last_Modified = value; }
        }

        public ISyncObject Clone()
        {
            var cloned = new Supporter();
             foreach (var property in this.GetType().GetProperties())
             {
                 if(!property.CanWrite || !property.CanRead) continue;
                 var value = property.GetValue(this, null);
                 property.SetValue(cloned, value, null);
             }
            return cloned;
        }
    }
}