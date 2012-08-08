using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

namespace SalsaImporter.Aft
{
    public class Supporter: ISyncObject
    {
        public int Id { get; set; }
        public int? ExternalId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ModifiedDate { get; set; }

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
        public string Chapter_Key { get; set; }
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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Supporter) obj);
        }

        protected bool Equals(Supporter other)
        {
            return StringUtility.EqualsIncludingNullEmpty(Title, other.Title)
                && StringUtility.EqualsIncludingNullEmpty(First_Name, other.First_Name) && StringUtility.EqualsIncludingNullEmpty(MI, other.MI) && StringUtility.EqualsIncludingNullEmpty(Last_Name, other.Last_Name)
                && StringUtility.EqualsIncludingNullEmpty(Suffix, other.Suffix) && StringUtility.EqualsIncludingNullEmpty(Email, other.Email)
                && StringUtility.EqualsIncludingNullEmpty(Email_Preference, other.Email_Preference) && StringUtility.EqualsIncludingNullEmpty(Phone, other.Phone)
                && StringUtility.EqualsIncludingNullEmpty(Cell_Phone, other.Cell_Phone) && StringUtility.EqualsIncludingNullEmpty(Phone_Provider, other.Phone_Provider)
                && StringUtility.EqualsIncludingNullEmpty(Work_Phone, other.Work_Phone) && StringUtility.EqualsIncludingNullEmpty(Pager, other.Pager)
                && StringUtility.EqualsIncludingNullEmpty(Home_Fax, other.Home_Fax) && StringUtility.EqualsIncludingNullEmpty(Work_Fax, other.Work_Fax)
                && StringUtility.EqualsIncludingNullEmpty(Street, other.Street) && StringUtility.EqualsIncludingNullEmpty(Street_2, other.Street_2)
                && StringUtility.EqualsIncludingNullEmpty(Street_3, other.Street_3) && StringUtility.EqualsIncludingNullEmpty(City, other.City)
                && StringUtility.EqualsIncludingNullEmpty(State, other.State) && StringUtility.EqualsIncludingNullEmpty(Zip, other.Zip)
                && StringUtility.EqualsIncludingNullEmpty(PRIVATE_Zip_Plus_4, other.PRIVATE_Zip_Plus_4) && StringUtility.EqualsIncludingNullEmpty(County, other.County)
                && StringUtility.EqualsIncludingNullEmpty(District, other.District) && StringUtility.EqualsIncludingNullEmpty(Country, other.Country)
                && Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude)
                && StringUtility.EqualsIncludingNullEmpty(Organization, other.Organization) && StringUtility.EqualsIncludingNullEmpty(Department, other.Department)
                && StringUtility.EqualsIncludingNullEmpty(Occupation, other.Occupation) && StringUtility.EqualsIncludingNullEmpty(Instant_Messenger_Service, other.Instant_Messenger_Service)
                && StringUtility.EqualsIncludingNullEmpty(Instant_Messenger_Name, other.Instant_Messenger_Name) && StringUtility.EqualsIncludingNullEmpty(Web_Page, other.Web_Page)
                && StringUtility.EqualsIncludingNullEmpty(Alternative_Email, other.Alternative_Email) && StringUtility.EqualsIncludingNullEmpty(Other_Data_1, other.Other_Data_1)
                && StringUtility.EqualsIncludingNullEmpty(Other_Data_2, other.Other_Data_2) && StringUtility.EqualsIncludingNullEmpty(Other_Data_3, other.Other_Data_3)
                && StringUtility.EqualsIncludingNullEmpty(Notes, other.Notes) 
//                && StringUtility.EqualsIncludingNullEmpty(Source, other.Source)
//                && StringUtility.EqualsIncludingNullEmpty(Source_Details, other.Source_Details) && StringUtility.EqualsIncludingNullEmpty(Source_Tracking_Code, other.Source_Tracking_Code)
                && StringUtility.EqualsIncludingNullEmpty(Tracking_Code, other.Tracking_Code)
                && StringUtility.EqualsIncludingNullEmpty(Status, other.Status) && StringUtility.EqualsIncludingNullEmpty(Timezone, other.Timezone)
                && StringUtility.EqualsIncludingNullEmpty(Language_Code, other.Language_Code) && StringUtility.EqualsIncludingNullEmpty(CustomString0, other.CustomString0)
                && StringUtility.EqualsIncludingNullEmpty(CustomString1, other.CustomString1) && StringUtility.EqualsIncludingNullEmpty(CustomString2, other.CustomString2)
                && StringUtility.EqualsIncludingNullEmpty(CustomString3, other.CustomString3) && StringUtility.EqualsIncludingNullEmpty(CustomString4, other.CustomString4)
                && StringUtility.EqualsIncludingNullEmpty(CustomString5, other.CustomString5) && StringUtility.EqualsIncludingNullEmpty(CustomString6, other.CustomString6)
                && StringUtility.EqualsIncludingNullEmpty(CustomString7, other.CustomString7) && StringUtility.EqualsIncludingNullEmpty(CustomString8, other.CustomString8)
                && StringUtility.EqualsIncludingNullEmpty(CustomString9, other.CustomString9) && CustomInteger0 == other.CustomInteger0
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
                int hashCode = (Title != null ? Title.GetHashCode() : 0);
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
                //hashCode = (hashCode * 397) ^ (Source != null ? Source.GetHashCode() : 0);
                //hashCode = (hashCode * 397) ^ (Source_Details != null ? Source_Details.GetHashCode() : 0);
                //hashCode = (hashCode * 397) ^ (Source_Tracking_Code != null ? Source_Tracking_Code.GetHashCode() : 0);
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

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var property in this.GetType().GetProperties())
            {
                if ( !property.CanRead) continue;
                var value = property.GetValue(this, null);
                if(value != null)
                    builder.AppendFormat(" {0}:{1}", property.Name, value);
            }
            return builder.ToString();
        }

     
    }
}