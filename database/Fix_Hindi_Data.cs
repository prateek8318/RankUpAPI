using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "Server=(local);Database=RankUp_QuestionDB;Trusted_Connection=true;";
        
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            
            // Clean existing Hindi data
            var deleteCmd = new SqlCommand("DELETE FROM dbo.QuestionTranslations WHERE LanguageCode = 'hi'", connection);
            await deleteCmd.ExecuteNonQueryAsync();
            
            // Insert proper Hindi data
            var insertCmd = new SqlCommand(@"
                INSERT INTO dbo.QuestionTranslations (QuestionId, LanguageCode, QuestionText, OptionA, OptionB, OptionC, OptionD, Explanation, CreatedAt)
                VALUES (@QuestionId, @LanguageCode, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD, @Explanation, @CreatedAt)", connection);
            
            // Question 17 - Hindi translation
            insertCmd.Parameters.Clear();
            insertCmd.Parameters.AddWithValue("@QuestionId", 17);
            insertCmd.Parameters.AddWithValue("@LanguageCode", "hi");
            insertCmd.Parameters.AddWithValue("@QuestionText", "भारतीय संविधान में नीति निदेशक सिद्धांत भाग IV में निहित हैं। ये देश के शासन में मौलिक हैं।");
            insertCmd.Parameters.AddWithValue("@OptionA", "केवल सर्वोच्च न्यायालय");
            insertCmd.Parameters.AddWithValue("@OptionB", "सर्वोच्च न्यायालय, उच्च न्यायालय और अधीनस्थ न्यायालय");
            insertCmd.Parameters.AddWithValue("@OptionC", "केवल उच्च न्यायालय");
            insertCmd.Parameters.AddWithValue("@OptionD", "केवल अधीनस्थ न्यायालय");
            insertCmd.Parameters.AddWithValue("@Explanation", "नीति निदेशक सिद्धांत गैर-न्यायिक हैं, लेकिन शासन में मौलिक हैं।");
            insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            await insertCmd.ExecuteNonQueryAsync();
            
            // Question 18 - Hindi translation
            insertCmd.Parameters.Clear();
            insertCmd.Parameters.AddWithValue("@QuestionId", 18);
            insertCmd.Parameters.AddWithValue("@LanguageCode", "hi");
            insertCmd.Parameters.AddWithValue("@QuestionText", "भारतीय संसदीय प्रणाली ब्रिटिश मॉडल पर आधारित है।");
            insertCmd.Parameters.AddWithValue("@OptionA", "राष्ट्रपति प्रधानमंत्री की नियुक्ति करता है।");
            insertCmd.Parameters.AddWithValue("@OptionB", "मंत्री परिषद लोकसभा के प्रति जिम्मेदार है।");
            insertCmd.Parameters.AddWithValue("@OptionC", "प्रधानमंत्री सरकार का प्रमुख है।");
            insertCmd.Parameters.AddWithValue("@OptionD", "राष्ट्रपति प्रधानमंत्री को बर्खास्त कर सकता है।");
            insertCmd.Parameters.AddWithValue("@Explanation", "संसदीय प्रणाली में राष्ट्रपति मंत्री परिषद की सलाह पर कार्य करता है।");
            insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            await insertCmd.ExecuteNonQueryAsync();
            
            // Question 19 - Simple Hindi question
            insertCmd.Parameters.Clear();
            insertCmd.Parameters.AddWithValue("@QuestionId", 19);
            insertCmd.Parameters.AddWithValue("@LanguageCode", "hi");
            insertCmd.Parameters.AddWithValue("@QuestionText", "भारत की राजधानी क्या है?");
            insertCmd.Parameters.AddWithValue("@OptionA", "मुंबई");
            insertCmd.Parameters.AddWithValue("@OptionB", "दिल्ली");
            insertCmd.Parameters.AddWithValue("@OptionC", "कोलकाता");
            insertCmd.Parameters.AddWithValue("@OptionD", "चेन्नई");
            insertCmd.Parameters.AddWithValue("@Explanation", "दिल्ली भारत की राजधानी है।");
            insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            await insertCmd.ExecuteNonQueryAsync();
            
            Console.WriteLine("Hindi data inserted successfully!");
            
            // Verify the data
            var verifyCmd = new SqlCommand("SELECT QuestionId, LanguageCode, QuestionText FROM dbo.QuestionTranslations WHERE LanguageCode = 'hi' ORDER BY QuestionId", connection);
            using (var reader = await verifyCmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"QuestionId: {reader["QuestionId"]}, Language: {reader["LanguageCode"]}, Text: {reader["QuestionText"]}");
                }
            }
        }
    }
}
