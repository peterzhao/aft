using NUnit.Framework;
using SalsaImporter;
using SalsaImporterTests.Utilities;

namespace SalsaImporterTests.FunctionalTests
{

    [TestFixture]
    [Category("FunctionalTest")]
    class DemoTests
    {
        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Demo;
        }

        [Test]
        public void CreateCustomColumnsForDemo()
        {
            //TestUtils.RemoveAllSalsa("custom_column", false);
            
            TestUtils.EnsureSupporterCustomColumn("cdb_guid", "varchar");
            TestUtils.EnsureSupporterCustomColumn("leadership_code", "varchar");
            TestUtils.EnsureSupporterCustomColumn("membership_status_2", "varchar");
            TestUtils.EnsureSupporterCustomColumn("contact_preferences", "varchar");
            TestUtils.EnsureSupporterCustomColumn("aft_local_number", "varchar");
            TestUtils.EnsureSupporterCustomColumn("aft_local_name", "varchar");
            TestUtils.EnsureSupporterCustomColumn("pub_at", "bool");
            TestUtils.EnsureSupporterCustomColumn("pub_psrp", "bool");
            TestUtils.EnsureSupporterCustomColumn("pub_pe", "bool");
            TestUtils.EnsureSupporterCustomColumn("pub_hed", "bool");
            TestUtils.EnsureSupporterCustomColumn("pub_health", "bool");
            TestUtils.EnsureSupporterCustomColumn("cdb_match_method", "varchar");
            TestUtils.EnsureSupporterCustomColumn("cdb_match_date", "datetime");
            TestUtils.EnsureSupporterCustomColumn("local_job_classification", "varchar");
            TestUtils.EnsureSupporterCustomColumn("most_recent_assesment", "varchar");
            TestUtils.EnsureSupporterCustomColumn("local_job_classification", "varchar");
            TestUtils.EnsureSupporterCustomColumn("salary", "varchar");
            TestUtils.EnsureSupporterCustomColumn("work_unit", "varchar");
            TestUtils.EnsureSupporterCustomColumn("worksite", "varchar");
            TestUtils.EnsureSupporterCustomColumn("worksite_area", "varchar");
            TestUtils.EnsureSupporterCustomColumn("payment_enrolled", "varchar");
            TestUtils.EnsureSupporterCustomColumn("enrollment_type", "varchar");
            TestUtils.EnsureSupporterCustomColumn("local_dues_category", "varchar");
            TestUtils.EnsureSupporterCustomColumn("employer", "varchar");
            TestUtils.EnsureSupporterCustomColumn("non_member_type", "varchar");
            TestUtils.EnsureSupporterCustomColumn("union_status", "varchar");
            TestUtils.EnsureSupporterCustomColumn("building_representative", "bool");
            TestUtils.EnsureSupporterCustomColumn("officer_info", "varchar");
            TestUtils.EnsureSupporterCustomColumn("key_contact", "bool");
            TestUtils.EnsureSupporterCustomColumn("pac_contributor", "bool");
            TestUtils.EnsureSupporterCustomColumn("pac_date", "varchar");
         }
    }
}
