using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterService.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedCountryStateLanguageData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert Country Codes first
            migrationBuilder.Sql(@"
                INSERT INTO Countries (Name, Code, IsActive, CreatedAt) VALUES
                ('India', 'IN', 1, GETDATE()),
                ('United States', 'US', 1, GETDATE()),
                ('United Kingdom', 'GB', 1, GETDATE()),
                ('Canada', 'CA', 1, GETDATE()),
                ('Australia', 'AU', 1, GETDATE()),
                ('Germany', 'DE', 1, GETDATE()),
                ('France', 'FR', 1, GETDATE()),
                ('Japan', 'JP', 1, GETDATE()),
                ('China', 'CN', 1, GETDATE()),
                ('Singapore', 'SG', 1, GETDATE()),
                ('United Arab Emirates', 'AE', 1, GETDATE()),
                ('Saudi Arabia', 'SA', 1, GETDATE()),
                ('Malaysia', 'MY', 1, GETDATE()),
                ('Thailand', 'TH', 1, GETDATE()),
                ('Indonesia', 'ID', 1, GETDATE()),
                ('Philippines', 'PH', 1, GETDATE()),
                ('Sri Lanka', 'LK', 1, GETDATE()),
                ('Bangladesh', 'BD', 1, GETDATE()),
                ('Nepal', 'NP', 1, GETDATE()),
                ('Pakistan', 'PK', 1, GETDATE())
            ");

            // Insert Indian States with country code
            migrationBuilder.Sql(@"
                INSERT INTO States (Name, Code, CountryCode, IsActive, CreatedAt) VALUES
                ('Andhra Pradesh', 'AP', 'IN', 1, GETDATE()),
                ('Arunachal Pradesh', 'AR', 'IN', 1, GETDATE()),
                ('Assam', 'AS', 'IN', 1, GETDATE()),
                ('Bihar', 'BR', 'IN', 1, GETDATE()),
                ('Chhattisgarh', 'CG', 'IN', 1, GETDATE()),
                ('Goa', 'GA', 'IN', 1, GETDATE()),
                ('Gujarat', 'GJ', 'IN', 1, GETDATE()),
                ('Haryana', 'HR', 'IN', 1, GETDATE()),
                ('Himachal Pradesh', 'HP', 'IN', 1, GETDATE()),
                ('Jharkhand', 'JH', 'IN', 1, GETDATE()),
                ('Karnataka', 'KA', 'IN', 1, GETDATE()),
                ('Kerala', 'KL', 'IN', 1, GETDATE()),
                ('Madhya Pradesh', 'MP', 'IN', 1, GETDATE()),
                ('Maharashtra', 'MH', 'IN', 1, GETDATE()),
                ('Manipur', 'MN', 'IN', 1, GETDATE()),
                ('Meghalaya', 'ML', 'IN', 1, GETDATE()),
                ('Mizoram', 'MZ', 'IN', 1, GETDATE()),
                ('Nagaland', 'NL', 'IN', 1, GETDATE()),
                ('Odisha', 'OD', 'IN', 1, GETDATE()),
                ('Punjab', 'PB', 'IN', 1, GETDATE()),
                ('Rajasthan', 'RJ', 'IN', 1, GETDATE()),
                ('Sikkim', 'SK', 'IN', 1, GETDATE()),
                ('Tamil Nadu', 'TN', 'IN', 1, GETDATE()),
                ('Telangana', 'TS', 'IN', 1, GETDATE()),
                ('Tripura', 'TR', 'IN', 1, GETDATE()),
                ('Uttar Pradesh', 'UP', 'IN', 1, GETDATE()),
                ('Uttarakhand', 'UK', 'IN', 1, GETDATE()),
                ('West Bengal', 'WB', 'IN', 1, GETDATE()),
                ('Delhi', 'DL', 'IN', 1, GETDATE()),
                ('Jammu & Kashmir', 'JK', 'IN', 1, GETDATE()),
                ('Ladakh', 'LA', 'IN', 1, GETDATE()),
                ('Puducherry', 'PY', 'IN', 1, GETDATE()),
                ('Chandigarh', 'CH', 'IN', 1, GETDATE()),
                ('Andaman & Nicobar', 'AN', 'IN', 1, GETDATE()),
                ('Dadra & Nagar Haveli', 'DN', 'IN', 1, GETDATE()),
                ('Daman & Diu', 'DD', 'IN', 1, GETDATE()),
                ('Lakshadweep', 'LD', 'IN', 1, GETDATE())
            ");

            // Insert Languages with codes
            migrationBuilder.Sql(@"
                INSERT INTO Languages (Name, Code, IsActive, CreatedAt) VALUES
                ('Hindi', 'HI', 1, GETDATE()),
                ('English', 'EN', 1, GETDATE()),
                ('Bengali', 'BN', 1, GETDATE()),
                ('Telugu', 'TE', 1, GETDATE()),
                ('Marathi', 'MR', 1, GETDATE()),
                ('Tamil', 'TA', 1, GETDATE()),
                ('Urdu', 'UR', 1, GETDATE()),
                ('Gujarati', 'GU', 1, GETDATE()),
                ('Kannada', 'KN', 1, GETDATE()),
                ('Odia', 'OR', 1, GETDATE()),
                ('Malayalam', 'ML', 1, GETDATE()),
                ('Punjabi', 'PA', 1, GETDATE()),
                ('Assamese', 'AS', 1, GETDATE()),
                ('Maithili', 'MI', 1, GETDATE()),
                ('Sanskrit', 'SA', 1, GETDATE()),
                ('Kashmiri', 'KS', 1, GETDATE()),
                ('Sindhi', 'SD', 1, GETDATE()),
                ('Konkani', 'KK', 1, GETDATE()),
                ('Manipuri', 'MN', 1, GETDATE()),
                ('Nepali', 'NE', 1, GETDATE()),
                ('Dogri', 'DG', 1, GETDATE()),
                ('Bodo', 'BO', 1, GETDATE()),
                ('Santali', 'ST', 1, GETDATE()),
                ('Marwari', 'MW', 1, GETDATE()),
                ('Awadhi', 'AW', 1, GETDATE()),
                ('Haryanvi', 'HY', 1, GETDATE()),
                ('Bhojpuri', 'BP', 1, GETDATE()),
                ('Chhattisgarhi', 'CT', 1, GETDATE()),
                ('Magahi', 'MG', 1, GETDATE()),
                ('Rajasthani', 'RJ', 1, GETDATE()),
                ('Kumaoni', 'KU', 1, GETDATE()),
                ('Garhwali', 'GW', 1, GETDATE()),
                ('Tulu', 'TU', 1, GETDATE()),
                ('Gondi', 'GO', 1, GETDATE())
            ");

            // Insert State-Language mappings for major Indian languages
            migrationBuilder.Sql(@"
                -- Hindi is spoken in most states
                INSERT INTO StateLanguages (StateId, LanguageId, IsActive, CreatedAt)
                SELECT s.Id, l.Id, 1, GETDATE()
                FROM States s
                JOIN Languages l ON l.Code = 'HI'
                WHERE s.CountryCode = 'IN';

                -- English is official in many states
                INSERT INTO StateLanguages (StateId, LanguageId, IsActive, CreatedAt)
                SELECT s.Id, l.Id, 1, GETDATE()
                FROM States s
                JOIN Languages l ON l.Code = 'EN'
                WHERE s.CountryCode = 'IN';

                -- Regional languages
                INSERT INTO StateLanguages (StateId, LanguageId, IsActive, CreatedAt)
                SELECT s.Id, l.Id, 1, GETDATE()
                FROM States s
                JOIN Languages l ON l.Code = 'BN'
                WHERE s.Code IN ('WB', 'TR');

                INSERT INTO StateLanguages (StateId, LanguageId, IsActive, CreatedAt)
                SELECT s.Id, l.Id, 1, GETDATE()
                FROM States s
                JOIN Languages l ON l.Code = 'TE'
                WHERE s.Code IN ('AP', 'TS');

                INSERT INTO StateLanguages (StateId, LanguageId, IsActive, CreatedAt)
                SELECT s.Id, l.Id, 1, GETDATE()
                FROM States s
                JOIN Languages l ON l.Code = 'MR'
                WHERE s.Code = 'MH';

                INSERT INTO StateLanguages (StateId, LanguageId, IsActive, CreatedAt)
                SELECT s.Id, l.Id, 1, GETDATE()
                FROM States s
                JOIN Languages l ON l.Code = 'TA'
                WHERE s.Code IN ('TN', 'PY');

                INSERT INTO StateLanguages (StateId, LanguageId, IsActive, CreatedAt)
                SELECT s.Id, l.Id, 1, GETDATE()
                FROM States s
                JOIN Languages l ON l.Code = 'GU'
                WHERE s.Code = 'GJ';

                INSERT INTO StateLanguages (StateId, LanguageId, IsActive, CreatedAt)
                SELECT s.Id, l.Id, 1, GETDATE()
                FROM States s
                JOIN Languages l ON l.Code = 'KN'
                WHERE s.Code = 'KA';

                INSERT INTO StateLanguages (StateId, LanguageId, IsActive, CreatedAt)
                SELECT s.Id, l.Id, 1, GETDATE()
                FROM States s
                JOIN Languages l ON l.Code = 'ML'
                WHERE s.Code = 'KL';

                INSERT INTO StateLanguages (StateId, LanguageId, IsActive, CreatedAt)
                SELECT s.Id, l.Id, 1, GETDATE()
                FROM States s
                JOIN Languages l ON l.Code = 'PA'
                WHERE s.Code IN ('PB', 'CH');

                INSERT INTO StateLanguages (StateId, LanguageId, IsActive, CreatedAt)
                SELECT s.Id, l.Id, 1, GETDATE()
                FROM States s
                JOIN Languages l ON l.Code = 'AS'
                WHERE s.Code = 'AS';

                INSERT INTO StateLanguages (StateId, LanguageId, IsActive, CreatedAt)
                SELECT s.Id, l.Id, 1, GETDATE()
                FROM States s
                JOIN Languages l ON l.Code = 'OR'
                WHERE s.Code = 'OD';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM StateLanguages WHERE StateId IN (SELECT Id FROM States WHERE CountryCode = 'IN')");
            migrationBuilder.Sql("DELETE FROM States WHERE CountryCode = 'IN'");
            migrationBuilder.Sql("DELETE FROM Languages WHERE Code IN ('HI', 'EN', 'BN', 'TE', 'MR', 'TA', 'UR', 'GU', 'KN', 'OR', 'ML', 'PA', 'AS', 'MI', 'SA', 'KS', 'SD', 'KK', 'MN', 'NE', 'DG', 'BO', 'ST', 'MW', 'AW', 'HY', 'BP', 'CT', 'MG', 'RJ', 'KU', 'GW', 'TU', 'GO')");
            migrationBuilder.Sql("DELETE FROM Countries WHERE Code IN ('IN', 'US', 'GB', 'CA', 'AU', 'DE', 'FR', 'JP', 'CN', 'SG', 'AE', 'SA', 'MY', 'TH', 'ID', 'PH', 'LK', 'BD', 'NP', 'PK')");
        }
    }
}
