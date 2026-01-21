using Microsoft.Data.SqlClient;
using System.Data;

var connectionString = "Server=localhost;Database=RankUpMasterDB;Trusted_Connection=true;TrustServerCertificate=true;";

using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();

Console.WriteLine("Adding Hindi names to existing states...");

// Get Hindi language ID
var getHindiLanguageCmd = new SqlCommand("SELECT Id FROM Languages WHERE Code = 'HI'", connection);
var hindiLanguageId = (int?)await getHindiLanguageCmd.ExecuteScalarAsync();

if (hindiLanguageId.HasValue)
{
    // Hindi names for Indian states
    var stateNames = new Dictionary<string, string>
    {
        {"AN", "अंडमान और निकोबार"},
        {"AP", "आंध्र प्रदेश"},
        {"AR", "अरुणाचल प्रदेश"},
        {"AS", "असम"},
        {"BR", "बिहार"},
        {"CH", "चंडीगढ़"},
        {"CG", "छत्तीसगढ़"},
        {"DN", "दादरा और नगर हवेली"},
        {"DD", "दमन और दीउ"},
        {"DL", "दिल्ली"},
        {"GA", "गोवा"},
        {"GJ", "गुजरात"},
        {"HR", "हरियाणा"},
        {"HP", "हिमाचल प्रदेश"},
        {"JK", "जम्मू और कश्मीर"},
        {"JH", "झारखंड"},
        {"KA", "कर्नाटक"},
        {"KL", "केरल"},
        {"LA", "लद्दाख"},
        {"LD", "लक्षद्वीप"},
        {"MP", "मध्य प्रदेश"},
        {"MH", "महाराष्ट्र"},
        {"MN", "मणिपुर"},
        {"ML", "मेघालय"},
        {"MZ", "मिजोरम"},
        {"NL", "नागालैंड"},
        {"OD", "ओडिशा"},
        {"PB", "पंजाब"},
        {"PY", "पुडुचेरी"},
        {"RJ", "राजस्थान"},
        {"SK", "सिक्किम"},
        {"TN", "तमिलनाडु"},
        {"TR", "त्रिपुरा"},
        {"TS", "तेलंगाना"},
        {"UP", "उत्तर प्रदेश"},
        {"UK", "उत्तराखंड"},
        {"WB", "पश्चिम बंगाल"}
    };

    int addedCount = 0;
    foreach (var stateName in stateNames)
    {
        // Check if state exists and doesn't already have Hindi name
        var checkCmd = new SqlCommand(@"
            SELECT COUNT(*) FROM States s 
            WHERE s.Code = @Code AND s.CountryCode = 'IN'
            AND NOT EXISTS (
                SELECT 1 FROM StateLanguages sl 
                WHERE sl.StateId = s.Id AND sl.LanguageId = @LanguageId
            )", connection);
        
        checkCmd.Parameters.AddWithValue("@Code", stateName.Key);
        checkCmd.Parameters.AddWithValue("@LanguageId", hindiLanguageId.Value);
        
        var exists = (int)await checkCmd.ExecuteScalarAsync();
        
        if (exists > 0)
        {
            // Get state ID and insert Hindi name
            var getStateCmd = new SqlCommand("SELECT Id FROM States WHERE Code = @Code AND CountryCode = 'IN'", connection);
            getStateCmd.Parameters.AddWithValue("@Code", stateName.Key);
            var stateId = (int)await getStateCmd.ExecuteScalarAsync();
            
            var insertCmd = new SqlCommand(@"
                INSERT INTO StateLanguages (StateId, LanguageId, Name, CreatedAt, IsActive)
                VALUES (@StateId, @LanguageId, @Name, GETDATE(), 1)", connection);
            
            insertCmd.Parameters.AddWithValue("@StateId", stateId);
            insertCmd.Parameters.AddWithValue("@LanguageId", hindiLanguageId.Value);
            insertCmd.Parameters.AddWithValue("@Name", stateName.Value);
            
            await insertCmd.ExecuteNonQueryAsync();
            addedCount++;
        }
    }
    
    Console.WriteLine($"Added Hindi names for {addedCount} states");
}
else
{
    Console.WriteLine("Hindi language not found in database");
}

Console.WriteLine("Database update completed!");
